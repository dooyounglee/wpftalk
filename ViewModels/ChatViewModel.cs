using OTILib.Models;
using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Views;
using talkLib.Util;

namespace talk2.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatViewModel(IUserService userService, IChatService chatService)
        {
            _chatService = chatService;
            _userService = userService;

            ChatCommand = new RelayCommand<int>(Chat);
            ToCreateRoom = new RelayCommand<object>(CreateRoom);
        }

        public async void InitAsync()
        {
            Debug.WriteLine("chat init");
            var chats = await _chatService.getChatList(_userService.Me.UsrNo);
            ChatList.Clear();
            foreach (var chat in chats)
            {
                ChatList.Add(chat);
            }
        }

        public ObservableCollection<Room> ChatList { get; } = new();

        public async Task Reload()
        {
            // ChatList = new List<Room>();
            // ChatList = await _chatService.getChatList(_userService.Me.UsrNo);
            // ChatList = new List<Room>();
            // ChatList = await _chatService.getChatList(_userService.Me.UsrNo);
        }

        public async Task Reload(int roomNo, string msg)
        {
            for (int i=0; i < ChatList.Count; i++)
            {
                var chat = ChatList[i];
                if (chat.RoomNo == roomNo)
                {
                    Room removed = ChatList[i];
                    ChatList.RemoveAt(i);
                    var roomWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(p => p.Tag is not null && Convert.ToInt16(p.Tag) == roomNo);
                    if (roomWin is null)
                    {
                        removed.CntUnread++;
                    }
                    removed.Chat = msg;
                    removed.RgtDtm = DateUtil.now("yyyyMMddHHmmss");
                    ChatList.Insert(0, removed);
                    break;
                }
            }
        }
        public async Task Reload_Create(int roomNo, int usrNo, string title, string msg)
        {
            ChatList.Insert(0, new Room()
            {
                RoomNo = roomNo,
                Title = title,
                CntUnread = usrNo == _userService.Me.UsrNo ? 0 : 1,
                Chat = msg,
                RgtDtm = DateUtil.now("yyyyMMddHHmmss"),
            });
        }

        private void Chat(int roomNo)
        {
            var roomWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(p => p.Tag is not null && Convert.ToInt16(p.Tag) == roomNo);
            if (roomWin is not null)
            {
                roomWin.Activate();
            }
            else
            {
                var roomView = new RoomView();
                AutomationProperties.SetAutomationId(roomView, $"RoomNo_{roomNo}");
                roomView.Tag = roomNo;
                var vm = new RoomViewModel(roomNo, _userService, _chatService);
                roomView.DataContext = vm;
                vm.InitAsync();
                roomView.Show();

                // unreadcnt=0
                for (int i = 0; i < ChatList.Count; i++)
                {
                    var chat = ChatList[i];
                    if (chat.RoomNo == roomNo)
                    {
                        if (chat.CntUnread > 0)
                        {
                            Room removed = ChatList[i];
                            ChatList.RemoveAt(i);
                            removed.CntUnread = 0;
                            ChatList.Insert(i, removed);
                        }
                        break;
                    }
                }
            }
        }

        private Window _userPopupView;
        private async void CreateRoom(object _)
        {
            _userPopupView = new UserPopupView();
            var vm = new UserPopupViewModel(_userPopupView, _userService);
            _userPopupView.DataContext = vm;
            await vm.InitAsync();
            vm.Validate += Validate;
            _userPopupView.ShowDialog();
            // if (userPopupView.ShowDialog() == true)
            // {
            //     var userList = ((UserPopupViewModel)userPopupView.DataContext).SelectedList;
            // 
            //     // 1:1방을 만들때는 이미 있는지 확인
            //     if (userList.Count == 1)
            //     {
            //         bool hasRoom = _chatService.CountRoomWithMe(userList[0].UsrNo) > 0 ? true : false;
            //         if (hasRoom)
            //         {
            //             MessageBox.Show("이미 있는 방인뎁쇼?");
            //             return;
            //         }
            //     }
            // 
            //     // 새로운방번호
            //     var newRoomNo = _chatService.CreateRoom(userList);
            //     if (newRoomNo > 0)
            //     {
            //         var _clientHandler = ((UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!).getClientHandler();
            //         _clientHandler?.Send(new ChatHub
            //         {
            //             RoomId = 0,
            //             State = ChatState.ChatReload,
            //         });
            //     }
            // }
        }
        private async void Validate(object? sender, EventArgs e)
        {
            var userList = ((UserPopupViewModel)_userPopupView.DataContext).SelectedList;

            // 본인을 선택했다면 제외(api보낼때 추가해서 보냄)
            userList = userList.Where(u => u.UsrNo != _userService.Me.UsrNo).ToList();

            // 1:1방을 만들때는 이미 있는지 확인
            if (userList.Count == 1)
            {
                bool hasRoom = await _chatService.CountRoomWithMe(userList[0].UsrNo) > 0 ? true : false;
                if (hasRoom)
                {
                    MessageBox.Show("이미 있는 방인뎁쇼?");
                    return;
                }
            }

            // 새로운방번호
            var newRoom = await _chatService.CreateRoom(userList);
            if (newRoom.RoomNo > 0)
            {
                var _clientHandler = ((UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!).getClientHandler();
                _clientHandler?.Send(new ChatHub
                {
                    RoomId = newRoom.RoomNo,
                    UsrNo = _userService.Me.UsrNo,
                    Message = newRoom.Chat,
                    Title = newRoom.Title,
                    State = ChatState.Create,
                });

                // 새로운방 열기
                var roomView = new RoomView();
                roomView.Tag = newRoom.RoomNo;
                var vm = new RoomViewModel(newRoom.RoomNo, _userService, _chatService);
                roomView.DataContext = vm;
                vm.InitAsync();
                roomView.Show();
            }

            _userPopupView.DialogResult = true;
        }

        public ICommand ChatCommand { get; set; }
        public ICommand ToCreateRoom { get; set; }
    }
}
