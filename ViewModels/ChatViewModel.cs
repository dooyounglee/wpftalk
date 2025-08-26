using OTILib.Models;
using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Views;

namespace talk2.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private List<Room> _chatList = new List<Room>();

        public ChatViewModel(IUserService userService, IChatService chatService)
        {
            _chatService = chatService;
            _userService = userService;

            GotoUserCommand = new RelayCommand<object>(GotoUser);
            GotoChatCommand = new RelayCommand<object>(GotoChat);
            GotoSettingCommand = new RelayCommand<object>(GotoSetting);
            ChatCommand = new RelayCommand<int>(Chat);
            ToCreateRoom = new RelayCommand<object>(CreateRoom);

            Init();
        }

        private async void Init()
        {
            Debug.WriteLine("chat init");
            ChatList = await _chatService.getChatList(_userService.Me.UsrNo);
        }

        public List<Room> ChatList
        {
            get => _chatList;
            set
            {
                _chatList = value;
                OnPropertyChanged();
            }
        }

        public async Task Reload()
        {
            // ChatList = new List<Room>();
            ChatList = await _chatService.getChatList(_userService.Me.UsrNo);
            // ChatList = new List<Room>();
            // ChatList = await _chatService.getChatList(_userService.Me.UsrNo);
        }

        private void GotoUser(object _)
        {
            var _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;
            _mainViewModel.changeViewModel(NaviType.UserView);
        }

        private void GotoChat(object _)
        {
            var _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;
            _mainViewModel.changeViewModel(NaviType.ChatView);
        }

        private void GotoSetting(object _)
        {
            var _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;
            _mainViewModel.changeViewModel(NaviType.SettingView);
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
                roomView.Tag = roomNo;
                var vm = new RoomViewModel(roomNo, _userService, _chatService);
                roomView.DataContext = vm;
                vm.InitAsync();
                roomView.Show();
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
            var newRoomNo = await _chatService.CreateRoom(userList);
            if (newRoomNo > 0)
            {
                var _clientHandler = ((UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!).getClientHandler();
                _clientHandler?.Send(new ChatHub
                {
                    RoomId = 0,
                    State = ChatState.ChatReload,
                });
            }

            _userPopupView.DialogResult = true;
        }

        public ICommand GotoUserCommand { get; set; }
        public ICommand GotoChatCommand { get; set; }
        public ICommand GotoSettingCommand { get; set; }
        public ICommand ChatCommand { get; set; }
        public ICommand ToCreateRoom { get; set; }
    }
}
