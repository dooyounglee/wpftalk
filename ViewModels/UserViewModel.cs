using CommunityToolkit.Mvvm.ComponentModel;
using OTILib.Handlers;
using OTILib.Models;
using OTILib.Sockets;
using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Util;
using talk2.Views;
using talkLib.Util;

namespace talk2.ViewModels
{
    class UserViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;

        private ChatClient _client;
        private ClientHandler? _clientHandler;
        private int _roomNo = 0;

        private readonly IUserService _userService;
        private User _me;
        private List<User> _userList = new List<User>();

        public UserViewModel(IUserService userService)
        {
            _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;

            _userService = userService;
        }

        public async void InitAsync()
        {
            LogoutCommand = new RelayCommand<object>(GoToLogout);
            CreateUserCommand = new RelayCommand<object>(GoToCreateUser);
            UserInfoCommand = new RelayCommand<int>(GoToUserInfo);
            UserContextmenuCommand = new RelayCommand<object>(GoToUserContextmenu);
            GotoUserCommand = new RelayCommand<object>(GotoUser);
            GotoChatCommand = new RelayCommand<object>(GotoChat);
            GotoSettingCommand = new RelayCommand<object>(GotoSetting);

            _client = new ChatClient(IPAddress.Parse("127.0.0.1"), 8080);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();

            _me = _userService.Me;
            UserList = await _userService.getUserList();
            UserUtil.setUsers(UserList); // cache에 users 담기
            UserList = UserList.Where(u => u.UsrNo != _me.UsrNo).ToList();
            SelectedConnState = "Online"; // 로그인 시, 첫 상태는 Online(비동기로 가져오느라, Socket Connect랑 순서가 꼬임)
        }

        public List<User> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged(nameof(UserList));
            }
        }

        public User Me { get => _me; }

        public ObservableCollection<string> ConnStateItems
        {
            get => new ObservableCollection<string>
            {
                "Offline", "Online", "Busy", "AFK"
            };
        }
        private string _selectedConnState;
        public string SelectedConnState
        {
            get { return _selectedConnState; }
            set
            {
                _selectedConnState = value;
                _clientHandler?.Send(new ChatHub
                {
                    RoomId = 0,
                    UsrNo = _userService.Me.UsrNo,
                    State = ChatState.StateChange,
                    connState = "Offline".Equals(_selectedConnState) ? ConnState.Offline : "Online".Equals(_selectedConnState) ? ConnState.Online : "Busy".Equals(_selectedConnState) ? ConnState.Busy : ConnState.AFK
                });
            }
        }

        private void GoToLogout(object _)
        {
            _userService.logout();
            Disconnected();
            _client = null;
            _clientHandler = null;
            _userList.Clear();
            _mainViewModel.changeViewModel(NaviType.LoginView);

            ProfileUtil.Clear();
            UserUtil.Clear();
        }

        private void GoToCreateUser(object _)
        {
            var newUserView = new NewUserView();
            newUserView.DataContext = new NewUserViewModel(newUserView, _userService);
            newUserView.ShowDialog();
        }

        private void GoToUserInfo(int usrNo)
        {
            var userInfoView = new NewUserView();
            var vm = new NewUserViewModel(userInfoView, _userService);
            userInfoView.DataContext = vm;
            vm.InitAsync(usrNo);
            // userInfoView.DataContext = new UserInfoViewModel(userInfoView, _userService, usrNo);
            userInfoView.ShowDialog();
        }

        private void GoToUserContextmenu(object _)
        {
            OtiLogger.log1("usercontextmenu");
        }

        private void GotoUser(object _)
        {
            _mainViewModel.changeViewModel(NaviType.UserView);
        }

        private void GotoChat(object _)
        {
            _mainViewModel.changeViewModel(NaviType.ChatView);
        }

        private void GotoSetting(object _)
        {
            _mainViewModel.changeViewModel(NaviType.SettingView);
        }

        public ICommand LogoutCommand { get; set; }
        public ICommand CreateUserCommand { get; set; }
        public ICommand UserInfoCommand { get; set; }
        public ICommand UserContextmenuCommand { get; set; }
        public ICommand GotoUserCommand { get; set; }
        public ICommand GotoChatCommand { get; set; }
        public ICommand GotoSettingCommand { get; set; }

        private string _msg;
        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);

        }

        public void SendReloadProfile()
        {
            _clientHandler?.Send(new ChatHub
            {
                RoomId = 0,
                UsrNo = _userService.Me.UsrNo,
                State = ChatState.ProfileReload,
            });
        }
        
        #region socket
        private async void Connect()
        {
            await _client.ConnectAsync(new ConnectionDetails
            {
                RoomId = _roomNo,
                UsrNo = _userService.Me.UsrNo,
            });
        }

        private void Connected(object? sender, OTILib.Events.ChatEventArgs e)
        {
            _clientHandler = e.ClientHandler;
        }

        public void Disconnected()
        {
            _client.Close();
        }

        private void Disconnected(object? sender, OTILib.Events.ChatEventArgs e)
        {
            _clientHandler = null;
            // _msgs.Add("서버의 연결이 끊겼습니다.");
        }

        private void RunningStateChanged(bool isRunning)
        {
            // btnConnect.Enabled = !isRunning;
            // btnStop.Enabled = isRunning;
        }

        public void SendMsg()
        {
            _clientHandler?.Send(new ChatHub
            {
                RoomId = _roomNo,
                UsrNo = _userService.Me.UsrNo,
                Message = Msg,
            });
            // _chatService.InsertChat(_chatId, Msg);
            Msg = "";
        }

        public ClientHandler getClientHandler()
        {
            return _clientHandler;
        }

        private async void Received(object? sender, OTILib.Events.ChatEventArgs e)
        {
            ChatHub hub = e.Hub;
            // string message = hub.State switch
            // {
            //     ChatState.Connect => $"{hub.UsrId}님이 접속하였습니다.",
            //     ChatState.Disconnect => $"{hub.UsrId}님이 종료하였습니다.",
            //     _ => $"{hub.UsrId}: {hub.Message}"
            // };

            switch (hub.State)
            {
                case ChatState.Connect:
                case ChatState.StateChange:
                    // 이미 접속한 유저는 새로운 접속유저만 받으면 되는데,
                    // 새로 접속한 유저는 이미 접속한 유저들 모두를 알아야 함
                    // 그래서 한명만 보낼수없고 전체 접속상황을 봐야 함
                    // hub.Data1 = _roomManager.ClientStates();
                    Dictionary<int, ConnState> p_connMap = JsonUtil.StringToObject<Dictionary<int, ConnState>>(hub.Data1);
                    List<User> p_userList = new List<User>();
                    for (var i=0; i<_userList.Count; i++)
                    {
                        if (p_connMap.ContainsKey(_userList[i].UsrNo))
                        {
                            _userList[i].ConnState = p_connMap[_userList[i].UsrNo];
                        }
                        else
                        {
                            _userList[i].ConnState = ConnState.Offline;
                        }
                        p_userList.Add(_userList[i]);
                    }
                    UserList = new List<User>();
                    UserList = p_userList;
                    break;
                case ChatState.Disconnect:
                    // 나갈사람은 더이상 받든말든 상관없음
                    // 대신 이미 접속한 유저는 나간사람을 알아야 함. 한명만 알아도 됌
                    // new ChatHub{RoomId = 0,UsrNo = e.Hub.UsrNo,State = ChatState.Disconnect,}
                    for (var i = 0; i < _userList.Count; i++)
                    {
                        if (_userList[i].UsrNo == hub.UsrNo)
                        {
                            _userList[i].ConnState = ConnState.Offline;
                            p_userList = _userList;
                            UserList = new List<User>();
                            UserList = p_userList;
                            break;
                        }
                    }
                    break;
                /* case ChatState.Invite:
                    _chats.Add(new Chat()
                    {
                        UsrNo = 0,
                        chat = $"{hub.inviter}님이 {hub.invitee}를 초대했습니다.",
                        Align = "Center",
                    });
                    break; */
                case ChatState.Invite:
                // _chats.Add(new Chat()
                // {
                //     UsrNo = 0,
                //     chat = $"{hub.inviter}님이 {hub.invitee}를 초대했습니다.",
                //     Align = "Center",
                // });
                // break;
                case ChatState.Leave:
                case ChatState.File:
                case ChatState.Chat: // 서버 전파 받고 채팅목록 최신화
                    ChatHub Data1 = ChatHub.Parse(hub.Data1);
                    // ChatViewModel chatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
                    // chatViewModel.Reload(data.RoomId, data.Message);
                    ChatViewModel chatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
                    chatViewModel.Reload(Data1.RoomId, Data1.Message);
                    break;
                case ChatState.Create:
                    Data1 = ChatHub.Parse(hub.Data1);
                    // ChatViewModel chatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
                    // chatViewModel.Reload(data.RoomId, data.Message);
                    chatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
                    chatViewModel.Reload_Create(Data1.RoomId, Data1.Title, Data1.Message);
                    break;
                case ChatState.Profile: // 서버 전파 받고 프로필사진 cache 최신화
                    var users = await _userService.getUserList();
                    UserUtil.setUsers(users);
                    ProfileUtil.clearProfileImage(hub.UsrNo);
                    break;
                default:
                    // User me = _loginService.UserInfo;
                    /* _chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = hub.UsrNo == me.UsrNo ? "Right" : "Left",
                    }); */
                    break;
            }

            //ChatsViewModel ContentViewModel = ShellViewModel.ServiceProvider.GetService(typeof(ChatsViewModel)) as ChatsViewModel;
            //ContentViewModel.LoadRoomList();

            //UserRepository ur = new UserRepository();
            //ur.GetUserList();
        }
        #endregion socket
    }
}
