using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OTILib.Models;
using OTILib.Sockets;
using OTILib.Events;
using OTILib.Handlers;
using System.Net;
using System.Windows.Interop;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using talk2.Models;
using talk2.Services;
using System.Windows.Input;
using talk2.Commands;
using talk2.Views;
using System.Windows;

namespace talk2.ViewModels
{
    public class RoomViewModel : ObservableObject
    {
        private IUserService _userService;
        private IChatService _chatService;

        private ChatClient _client;
        private ClientHandler? _clientHandler;
        private int _roomNo;

        public int RoomNo { get => _roomNo; }
        public RoomViewModel(int roomNo, IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
            _msgs = new ObservableCollection<string>();
            _roomNo = roomNo;

            InviteCommand = new RelayCommand<object>(Invite);
            RoomUserCommand = new RelayCommand<object>(RoomUser);
            LeaveCommand = new RelayCommand<object>(Leave);

            _client = new ChatClient(IPAddress.Parse(_userService.Me.Ip), _userService.Me.Port);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();

            _chats = new ObservableCollection<Chat>();
            var chats = _chatService.SelectChats(_roomNo).Reverse<Chat>();
            foreach (var chat in chats)
            {
                switch (chat.ChatFg)
                {
                    case "A": chat.Align = chat.UsrNo == _userService.Me.UsrNo ? "Right" : "Left"; break;
                    case "B": 
                    case "C": chat.Align = "Center"; break;
                }
                _chats.Add(chat);
            }
        }

        private string _msg;
        public string Msg
        {
            get => _msg;
            set => SetProperty(ref _msg, value);
        }

        private ObservableCollection<string> _msgs;
        public ObservableCollection<string> Msgs
        {
            get => _msgs;
            set => SetProperty(ref _msgs, value);
        }

        public ObservableCollection<Chat> _chats;
        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }

        #region Command
        public ICommand InviteCommand { get; set; }
        private void Invite(object _)
        {
            var userPopupView = new UserPopupView();
            userPopupView.DataContext = new UserPopupViewModel(userPopupView, _userService);
            if (userPopupView.ShowDialog() == true)
            {
                var userList = ((UserPopupViewModel)userPopupView.DataContext).SelectedList;
                string msg = _chatService.Invite(_roomNo, userList);

                _clientHandler?.Send(new ChatHub
                {
                    RoomId = _roomNo,
                    UsrNo = _userService.Me.UsrNo,
                    Message = msg,
                    State = ChatState.Invite,
                });
            }
        }
        public ICommand RoomUserCommand { get; set; }
        private void RoomUser(object _)
        {
            var roomUserPopupView = new RoomUserPopupView();
            roomUserPopupView.DataContext = new RoomUserPopupViewModel(_roomNo, roomUserPopupView, _chatService);
            roomUserPopupView.ShowDialog();
        }
        public ICommand LeaveCommand { get; set; }
        private void Leave(object _)
        {
            // TODO confirm창 띄워서 ok일때만 돌게
            // room-user연결 끊기
            string msg =_chatService.Leave(_roomNo, _userService.Me.UsrNo);

            // 채팅방에 나갔다고 알리기
            _clientHandler?.Send(new ChatHub
            {
                RoomId = _roomNo,
                UsrNo = _userService.Me.UsrNo,
                Message = msg,
                State = ChatState.Leave
            });

            // 창 찾아서 닫기
            var roomWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(p => p.Tag is not null && Convert.ToInt16(p.Tag) == _roomNo);
            roomWin.Close();
        }
        #endregion Command

        #region socket
        private async void Connect()
        {
            await _client.ConnectAsync(new ConnectionDetails
            {
                RoomId = _roomNo,
                UsrNo = _userService.Me.UsrNo,
                UsrNm = _userService.Me.UsrNm,
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
            _chatService.InsertChat(_roomNo, _userService.Me.UsrNo, "A", Msg);
            _clientHandler?.Send(new ChatHub
            {
                RoomId = _roomNo,
                UsrNo = _userService.Me.UsrNo,
                Message = Msg,
            });
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
                case ChatState.Invite:
                    _chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = "Center",
                    });
                    break;
                case ChatState.Leave:
                    _chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = "Center",
                    });
                    break;
                default:
                    _chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = hub.UsrNo == _userService.Me.UsrNo ? "Right" : "Left",
                    });
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
