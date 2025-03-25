using CommunityToolkit.Mvvm.ComponentModel;
using OTILib.Handlers;
using OTILib.Models;
using OTILib.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using talk2.Commands;
using talk2.Models;
using talk2.Services;

namespace talk2.ViewModels
{
    class UserViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;

        private ChatClient _client;
        private ClientHandler? _clientHandler;
        private int _roomNo = 0;

        private readonly IUserService _userService;
        private List<string> _userList = new List<string>();

        public UserViewModel(IUserService userService)
        {
            _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;

            _userService = userService;

            LogoutCommand = new RelayCommand<object>(GoToLogout);
            GotoUserCommand = new RelayCommand<object>(GotoUser);
            GotoChatCommand = new RelayCommand<object>(GotoChat);
            GotoSettingCommand = new RelayCommand<object>(GotoSetting);

            _client = new ChatClient(IPAddress.Parse("127.0.0.1"), 8080);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();

            Init();
        }

        private void Init()
        {
            Debug.WriteLine("user init");
            UserList = _userService.getUserList();
        }

        public List<string> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged();
            }
        }

        private void GoToLogout(object _)
        {
            _mainViewModel.changeViewModel(NaviType.LoginView);
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
        public ICommand GotoUserCommand { get; set; }
        public ICommand GotoChatCommand { get; set; }
        public ICommand GotoSettingCommand { get; set; }

        private string _msg;
        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);

        }
        #region socket
        private async void Connect()
        {
            await _client.ConnectAsync(new ConnectionDetails
            {
                RoomId = _roomNo,
                UsrNo = 1,// _loginService.UserInfo.UsrNo,
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
                UsrNo = 1, // _loginService.UserInfo.UsrNo,
                Message = Msg,
            });
            // _chatService.InsertChat(_chatId, Msg);
            Msg = "";
        }

        private void Received(object? sender, OTILib.Events.ChatEventArgs e)
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
                case ChatState.Connect: break;
                case ChatState.Disconnect: break;
                /* case ChatState.Invite:
                    _chats.Add(new Chat()
                    {
                        UsrNo = 0,
                        chat = $"{hub.inviter}님이 {hub.invitee}를 초대했습니다.",
                        Align = "Center",
                    });
                    break; */
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
