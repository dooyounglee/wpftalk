using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        private void Init()
        {
            Debug.WriteLine("chat init");
            ChatList = _chatService.getChatList(_userService.Me.UsrNo);
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

        public void Reload()
        {
            ChatList = new List<Room>();
            ChatList = _chatService.getChatList(_userService.Me.UsrNo);
            ChatList = new List<Room>();
            ChatList = _chatService.getChatList(_userService.Me.UsrNo);
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
                roomView.DataContext = new RoomViewModel(roomNo, _userService, _chatService);
                roomView.Show();
            }
        }

        private void CreateRoom(object _)
        {
            var userPopupView = new UserPopupView();
            userPopupView.DataContext = new UserPopupViewModel(userPopupView, _userService);
            var result = userPopupView.ShowDialog();
            // ((UserPopupViewModel)userPopupView.DataContext).SelectedList 에 담겨있음
        }

        public ICommand GotoUserCommand { get; set; }
        public ICommand GotoChatCommand { get; set; }
        public ICommand GotoSettingCommand { get; set; }
        public ICommand ChatCommand { get; set; }
        public ICommand ToCreateRoom { get; set; }
    }
}
