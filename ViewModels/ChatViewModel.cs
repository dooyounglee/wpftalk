using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Debug.WriteLine("chat");
            Debug.WriteLine(roomNo);
            var roomView = new RoomView();
            roomView.DataContext = new RoomViewModel(roomNo, _userService, _chatService);
            roomView.Show();
        }

        public ICommand GotoUserCommand { get; set; }
        public ICommand GotoChatCommand { get; set; }
        public ICommand GotoSettingCommand { get; set; }
        public ICommand ChatCommand { get; set; }
    }
}
