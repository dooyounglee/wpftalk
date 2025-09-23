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
using talk2.Items;
using talk2.Models;
using talk2.Services;
using talk2.Settings;
using talk2.Util;
using talk2.Views;
using talkLib.Util;

namespace talk2.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private ChatClient _client;
        private ClientHandler? _clientHandler;
        private int _roomNo = 0;

        private readonly IUserService _userService;
        private readonly IDivService _divService;
        private User _me;
        private List<User> _userList = new List<User>();

        public UserViewModel(IUserService userService, IDivService divService)
        {
            _userService = userService;
            _divService = divService;
        }

        public async void InitAsync()
        {
            LogoutCommand = new RelayCommand<object>(GoToLogout);
            CreateUserCommand = new RelayCommand<object>(GoToCreateUser);
            UserInfoCommand = new RelayCommand<int>(GoToUserInfo);
            UserContextmenuCommand = new RelayCommand<object>(GoToUserContextmenu);

            _client = new ChatClient(IPAddress.Parse("127.0.0.1"), 8080);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();

            // 모든사람들
            _allUsers = await _userService.getUserList();
            UserUtil.setUsers(_allUsers); // cache에 users 담기

            // 나
            _me = _userService.Me;
            _me.ProfileImage = await ProfileUtil.GetProfileImageAsync(_me.UsrNo);
            _me.ConnState = SelectedMyConnState;
            MeList.Clear();MeList.Add(_me);

            // 다른사람들
            var users = _allUsers.Where(u => u.UsrNo > 0 && u.UsrNo != _me.UsrNo ).ToList();
            foreach (var user in users)
            {
                user.ProfileImage = await ProfileUtil.GetProfileImageAsync(user.UsrNo);
            };
            UserList = users;
            SelectedMyConnState = ConnState.Online; // 로그인 시, 첫 상태는 Online(비동기로 가져오느라, Socket Connect랑 순서가 꼬임)
        }

        [ObservableProperty]
        private ObservableCollection<User> meList = new();
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

        public ObservableCollection<ConnStateItem> MyConnStateItems { get; } = new ObservableCollection<ConnStateItem>
        {
            new ConnStateItem { State = ConnState.Offline, Display = "오프라인" },
            new ConnStateItem { State = ConnState.Online, Display = "온라인" },
            new ConnStateItem { State = ConnState.Busy, Display = "바쁨" },
            new ConnStateItem { State = ConnState.AFK, Display = "A.F.K" },
        };
        [ObservableProperty] private ConnState selectedMyConnState;
        partial void OnSelectedMyConnStateChanged(ConnState value)
        {
            _clientHandler?.Send(new ChatHub
            {
                RoomId = 0,
                UsrNo = _userService.Me.UsrNo,
                State = ChatState.StateChange,
                connState = value,
            });
        }

        #region 검색, filter
        private List<User> _allUsers = new();
        [ObservableProperty] public ConnState searchConnState = ConnState.Null;
        partial void OnSearchConnStateChanged(ConnState value) { FilterUsers(); }
        [ObservableProperty] public int searchDiv = 0;
        partial void OnSearchDivChanged(int value) { FilterUsers(); }
        [ObservableProperty] public string searchUsrNm = "";
        partial void OnSearchUsrNmChanged(string value) { FilterUsers(); }
        [ObservableProperty] private List<ConnStateItem> searchConnStateItems = new()
        {
            new ConnStateItem { State = ConnState.Null, Display = "전체" },
            new ConnStateItem { State = ConnState.Offline, Display = "오프라인" },
            new ConnStateItem { State = ConnState.Online, Display = "온라인" },
            new ConnStateItem { State = ConnState.Busy, Display = "바쁨" },
            new ConnStateItem { State = ConnState.AFK, Display = "A.F.K" },
        };
        [ObservableProperty] private List<DivItem> searchDivItems = new()
        {
            new DivItem { DivNo = 0, DivNm = "전체" },
            new DivItem { DivNo = 1, DivNm = "전략사업1 Div." },
            new DivItem { DivNo = 2, DivNm = "전략사업2 Div." },
            new DivItem { DivNo = 3, DivNm = "전략사업3 Div." },
            new DivItem { DivNo = 4, DivNm = "공공사업2 Div." },
        };

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private async Task ReloadUser()
        {
            _allUsers = await _userService.getUserList();
            UserUtil.setUsers(_allUsers); // cache에 users 담기

            ProfileUtil.Clear();
            foreach (var user in _allUsers)
            {
                user.ProfileImage = await ProfileUtil.GetProfileImageAsync(user.UsrNo);
            };

            // 내정보 최신화
            _me.ProfileImage = await ProfileUtil.GetProfileImageAsync(_me.UsrNo);
            MeList.Clear(); MeList.Add(_me);

            // 소켓서버에 접속한 유저 상태를 받아오기 위해서
            _clientHandler?.Send(new ChatHub
            {
                RoomId = 0,
                UsrNo = _userService.Me.UsrNo,
                State = ChatState.StateChange,
                connState = SelectedMyConnState,
            });
        }
        private void FilterUsers()
        {
            var filteredUsers = _allUsers.Where(u => u.UsrNo > 0 && u.UsrNo != _me.UsrNo);
            filteredUsers = SearchUsrNm.Trim().Length == 0 ? filteredUsers : filteredUsers.Where(u => u.UsrNm.Contains(SearchUsrNm));
            filteredUsers = SearchDiv == 0 ? filteredUsers : filteredUsers.Where(u => u.DivNo == SearchDiv);
            filteredUsers = SearchConnState == ConnState.Null ? filteredUsers : filteredUsers.Where(u => u.ConnState == SearchConnState);
            UserList = filteredUsers.ToList();

        }
        #endregion 검색, filter

        private void GoToLogout(object _)
        {
            _userService.logout();
            Disconnected();
            _client = null;
            _clientHandler = null;

            // 자동로그인 해제
            var settings = LoginSettingsManager.Load();
            settings.IsAutoLogin = false;
            LoginSettingsManager.Save(settings);

            _userList.Clear();
            ((MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))).changeViewModel(NaviType.LoginView);
            var loginVM = (LoginViewModel)App.Current.Services.GetService(typeof(LoginViewModel));
            loginVM.Init();

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

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private async Task Div()
        {
            var divView = new DivView();
            var vm = new DivViewModel(_divService);
            divView.DataContext = vm;
            await vm.InitAsync();
            divView.ShowDialog();
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
                    for (var i=0; i<_allUsers.Count; i++)
                    {
                        if (p_connMap.ContainsKey(_allUsers[i].UsrNo))
                        {
                            _allUsers[i].ConnState = p_connMap[_allUsers[i].UsrNo];
                        }
                        else
                        {
                            _allUsers[i].ConnState = ConnState.Offline;
                        }
                        p_userList.Add(_allUsers[i]);
                    }
                    UserList = new List<User>();
                    UserList = p_userList;

                    FilterUsers();
                    break;
                case ChatState.Disconnect:
                    // 나갈사람은 더이상 받든말든 상관없음
                    // 대신 이미 접속한 유저는 나간사람을 알아야 함. 한명만 알아도 됌
                    // new ChatHub{RoomId = 0,UsrNo = e.Hub.UsrNo,State = ChatState.Disconnect,}
                    for (var i = 0; i < _allUsers.Count; i++)
                    {
                        if (_allUsers[i].UsrNo == hub.UsrNo)
                        {
                            _allUsers[i].ConnState = ConnState.Offline;
                            p_userList = _allUsers;
                            UserList = new List<User>();
                            UserList = p_userList;
                            break;
                        }
                    }

                    FilterUsers();
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
                    chatViewModel.Reload_Create(Data1.RoomId, Data1.UsrNo, Data1.Title, Data1.Message);
                    break;
                case ChatState.ProfileReload: // 서버 전파 받고 프로필사진 cache 최신화
                    var users = await _userService.getUserList();
                    UserUtil.setUsers(users);
                    ProfileUtil.clearProfileImage(hub.UsrNo);

                    ReloadUser();
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
