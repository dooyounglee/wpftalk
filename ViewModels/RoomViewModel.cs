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

        public event EventHandler<object>? validate;

        private ChatClient _client;
        private ClientHandler? _clientHandler;
        private int _roomNo;
        private Room Room { get; set; }
        private Window _roomWindow;
        private FlashWindow _flashWindow;

        public int RoomNo { get => _roomNo; }
        public RoomViewModel(int roomNo, IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;
            _msgs = new ObservableCollection<string>();
            _roomNo = roomNo;
            _roomWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(p => p.Tag is not null && Convert.ToInt16(p.Tag) == _roomNo);
            _flashWindow = new FlashWindow(_roomWindow);

            InviteCommand = new RelayCommand<object>(Invite);
            RoomUserCommand = new RelayCommand<object>(RoomUser);
            LeaveCommand = new RelayCommand<object>(Leave);
            AllChatCommand = new RelayCommand<object>(AllChat);
            SaveTitleCommand = new RelayCommand<object>(SaveTitle);
            EditTitleCommand = new RelayCommand<object>(EditTitle);
            CancleTitleCommand = new RelayCommand<object>(CancleTitle);

            _client = new ChatClient(IPAddress.Parse(_userService.Me.Ip), _userService.Me.Port);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();

            Room = _chatService.getChat(_roomNo);
            Title = Room.Title;
            TitleReadonly = false;

            // 않읽은 채팅 읽기
            _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
            ReloadChatList();

            // 이전채팅(10개) 불러오기
            ReloadChats();
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

        private void ReloadChats()
        {
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

        private void ReloadChatList()
        {
            // 채팅방목록 새로고침
            ChatViewModel chatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
            chatViewModel.Reload();
        }

        #region Command
        private Window _userPopupView;
        public ICommand InviteCommand { get; set; }
        private void Invite(object _)
        {
            _userPopupView = new UserPopupView();
            _userPopupView.DataContext = new UserPopupViewModel(_userPopupView, _userService);
            ((UserPopupViewModel)_userPopupView.DataContext).Validate += Validate;
            _userPopupView.ShowDialog();
            // if (userPopupView.ShowDialog() == true)
            // {
            //     var userList = ((UserPopupViewModel)userPopupView.DataContext).SelectedList;
            // 
            //     // 중복 초대 체크
            //     bool IsThereSomeoneinRoom = _chatService.IsThereSomeoneinRoom(_roomNo, userList);
            //     if (IsThereSomeoneinRoom)
            //     {
            //         MessageBox.Show("이미 있는사람을 추가했는뎁쇼?");
            //         return;
            //     }
            // 
            //     string msg = _chatService.Invite(_roomNo, userList);
            //     
            //     _clientHandler?.Send(new ChatHub
            //     {
            //         RoomId = _roomNo,
            //         UsrNo = _userService.Me.UsrNo,
            //         Message = msg,
            //         State = ChatState.Invite,
            //     });
            // }
        }
        private void Validate(object? sender, EventArgs e)
        {
            var userList = ((UserPopupViewModel)_userPopupView.DataContext).SelectedList;

            // 중복 초대 체크
            bool IsThereSomeoneinRoom = _chatService.IsThereSomeoneinRoom(_roomNo, userList);
            if (IsThereSomeoneinRoom)
            {
                MessageBox.Show("이미 있는사람을 추가했는뎁쇼?");
                return;
            }

            string msg = _chatService.Invite(_roomNo, userList);

            _clientHandler?.Send(new ChatHub
            {
                RoomId = _roomNo,
                UsrNo = _userService.Me.UsrNo,
                Message = msg,
                State = ChatState.Invite,
            });

            _userPopupView.DialogResult = true;
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
        public ICommand AllChatCommand { get; set; }
        private void AllChat(object _)
        {
            Window roomAllChatView = new RoomAllChatView();
            roomAllChatView.DataContext = new RoomAllChatViewModel(_roomNo, roomAllChatView, _userService, _chatService);
            roomAllChatView.ShowDialog();
        }

        private bool _titleReadonly;
        public bool TitleReadonly
        {
            get => _titleReadonly;
            set => SetProperty(ref _titleReadonly, value);
        }
        public string Title { get; set; }
        public ICommand SaveTitleCommand { get; set; }
        private void SaveTitle(object _)
        {
            _chatService.EditTitle(_roomNo, _userService.Me.UsrNo, Title);
            TitleReadonly = false;

            // 채팅방목록 새로고침
            ReloadChatList();
        }
        public ICommand EditTitleCommand { get; set; }
        private void EditTitle(object _)
        {
            TitleReadonly = true;
        }
        public ICommand CancleTitleCommand { get; set; }
        private void CancleTitle(object _)
        {
            TitleReadonly = false;
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
                    _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
                    _chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = hub.UsrNo == _userService.Me.UsrNo ? "Right" : "Left",
                    });

                    // 일단 무조건 깜빡이도록 하지만, 활성화 상태면 안깜빡일꺼고, 비활성화 상태면 깜빡일꺼임
                    _flashWindow.FlashApplicationWindow();

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
