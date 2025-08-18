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
using System.IO;
using talkLib.Util;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Data;
using OTILib.Util;

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
            
            DownloadCommand = new RelayCommand<Chat>(Download);

            _client = new ChatClient(IPAddress.Parse(_userService.Me.Ip), _userService.Me.Port);
            _client.Connected += Connected;
            _client.Disconnected += Disconnected;
            _client.Received += Received;
            _client.RunningStateChanged += RunningStateChanged;

            Connect();

            // Room = _chatService.getRoom(_roomNo);
            // Title = Room.Title;
            // TitleReadonly = false;

            // 않읽은 채팅 읽기
            // _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
            // ReloadChatList();

            // 이전채팅(10개) 불러오기
            // ReloadChats();
        }

        public async void InitAsync()
        {
            Room = await _chatService.getRoom(_roomNo);
            Title = Room.Title;
            TitleReadonly = false;

            // 않읽은 채팅 읽기
            // _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
            await _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
            await ReloadChatList();

            // 이전채팅(10개) 불러오기
            await ReloadChats();
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

        public ObservableCollection<Chat> Chats { get; } = new();

        public bool IsSave { get; set; } = true;

        private async Task ReloadChats()
        {
            var chats = await _chatService.SelectChats(_roomNo);
            chats.Reverse<Chat>();
            foreach (var chat in chats)
            {
                switch (chat.ChatFg)
                {
                    case "A": chat.Align = chat.UsrNo == _userService.Me.UsrNo ? "Right" : "Left"; break;
                    case "B":
                    case "C":
                    case "D": chat.Align = "Center"; break;
                    case "E": 
                        chat.Align = chat.UsrNo == _userService.Me.UsrNo ? "Right" : "Left";
                        chat.Image = ImageUtil.IsImage(chat.chat) ? new BitmapImage(new Uri("http://localhost:8686/file/" + chat.FileNo)) : null;
                        chat.isImage = ImageUtil.IsImage(chat.chat) ? "Visible" : "Collapsed";
                        chat.isFile = "Visible";
                        break;

                }
                Chats.Add(chat);
            }
        }

        private async Task ReloadChatList()
        {
            // 채팅방목록 새로고침
            ChatViewModel chatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
            await chatViewModel.Reload();
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
        private async void Validate(object? sender, EventArgs e)
        {
            var userList = ((UserPopupViewModel)_userPopupView.DataContext).SelectedList;
            var usrNoList = userList.Select(x => x.UsrNo).ToList();

            // 중복 초대 체크
            // bool IsThereSomeoneinRoom = _chatService.IsThereSomeoneinRoom(_roomNo, userList);
            bool IsThereSomeoneinRoom = await _chatService.IsThereSomeoneinRoom(_roomNo, usrNoList);
            if (IsThereSomeoneinRoom)
            {
                MessageBox.Show("이미 있는사람을 추가했는뎁쇼?");
                return;
            }

            string invitedUsers = string.Join(",", userList.Select(u => u.UsrNm));
            string msg = await _chatService.Invite(_roomNo, usrNoList, invitedUsers);

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
            var vm = new RoomAllChatViewModel(_roomNo, roomAllChatView, _userService, _chatService);
            roomAllChatView.DataContext = vm;
            vm.InitAsync();
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

        public ICommand DownloadCommand { get; set; }
        private void Download(Chat chat)
        {
            var ext = chat.chat.IndexOf(".") == -1 ? "*" : chat.chat.Substring(chat.chat.LastIndexOf(".") + 1);
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                FileName = chat.chat + ("*".Equals(ext) ? "" : "." + ext),
                Filter = $"{("*".Equals(ext) ? "모든" : ext)} 파일 (*.{ext})|*.{ext}",
                // Filter = "모든 파일 (*.*)|*.*",
            };
            bool? isOk = saveFileDialog.ShowDialog();

            if ((bool)isOk)
            {
                if (chat.FileNo > 0)
                {
                    WebClient wc = new WebClient();
                    wc.DownloadFile("http://localhost:8686/file/" + chat.FileNo, saveFileDialog.FileName);
                }
                else
                {
                FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create);
                file.Write(chat.FileBuffer, 0, chat.FileBuffer.Length);
                file.Close();
                }
            }
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
            if (IsSave)
            {
                _chatService.InsertChat(_roomNo, _userService.Me.UsrNo, "A", Msg);
            }
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
                    Chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = "Center",
                    });
                    break;
                case ChatState.Leave:
                    Chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = "Center",
                    });
                    break;
                case ChatState.File:
                    _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
                    Chats.Add(new Chat()
                    {
                        UsrNo = hub.UsrNo,
                        chat = hub.Message,
                        Align = hub.UsrNo == _userService.Me.UsrNo ? "Right" : "Left",
                        FileBuffer = hub.Data2,
                        isFile = hub.Data2 is not null ? "Visible" : "Collapsed",
                        isImage = ImageUtil.IsImage(hub.Data2) ? "Visible" : "Collapsed",
                        Image = ImageUtil.IsImage(hub.Data2) ? byteToBitmapImage(hub.Data2) : null,
                    });
                    break;
                default:
                    _chatService.ReadChat(_roomNo, _userService.Me.UsrNo);
                    Chats.Add(new Chat()
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

        public void Send(string[] files)
        {
            foreach (var file in files)
            {
                // byte[] buffer = null;
                FileInfo fi = new FileInfo(file);
                Models.File f = new Models.File()
                {
                    FilePath = $"D:/temp/{DateUtil.now("yyyyMMdd")}/",
                    FileName = $"OTI{DateUtil.now("yyyyMMddHHmmddfffffff")}_{NumberUtil.random(1,999)}",
                    FileExt = fi.Extension,
                    OriginName = fi.Name,
                    Buffer = System.IO.File.ReadAllBytes(file),
                };
                // byte[] buffer = new byte[fs.Length];
                // fs.Read(buffer, 0, (int)fs.Length);
                if (IsSave)
                {
                    _chatService.InsertChat(_roomNo, _userService.Me.UsrNo, "E", f);
                }

                _clientHandler?.Send(new ChatHub
                {
                    RoomId = _roomNo,
                    UsrNo = _userService.Me.UsrNo,
                    Message = f.OriginName,
                    State = ChatState.File,
                    Data = new Dictionary<int, object>
                    {
                        {1, f.FilePath },
                        {2, f.FileName },
                    },
                    Data2 = f.Buffer,
                });
            }
            Msg = "";
        }

        public static BitmapImage byteToBitmapImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
