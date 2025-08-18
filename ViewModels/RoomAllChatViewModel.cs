using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using talk2.Commands;
using talk2.Models;
using talk2.Services;

namespace talk2.ViewModels
{
    public class RoomAllChatViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IChatService _chatService;

        private Window _roomAllChatView;
        private int _roomNo;
        private int _page = 1;
        public int Page
        {
            get => _page;
            set => SetProperty(ref _page, value);
        }
        private int _totalCnt = 0;
        public int TotalCnt { get => (int)Math.Ceiling(_totalCnt/10d); }

        public RoomAllChatViewModel(int roomNo, Window roomAllChatView, IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;

            _roomNo = roomNo;
            _roomAllChatView = roomAllChatView;

            PrevCommand = new RelayCommand<object>(Prev);
            NextCommand = new RelayCommand<object>(Next);

            SelectChats();
        }

        public ObservableCollection<Chat> _chats;
        public ObservableCollection<Chat> Chats
        {
            get => _chats;
            set => SetProperty(ref _chats, value);
        }
        private async void SelectChats()
        {
            // 전체페이지 목록만들기
            _totalCnt = _chatService.CountChats(_roomNo);

            // 채팅목록 뿌리기
            var chats = await _chatService.SelectChats(_roomNo, _page);
            chats.Reverse<Chat>();

            Chats = new ObservableCollection<Chat>();
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

        #region Command
        public ICommand PrevCommand { get; set; }
        private void Prev(object _)
        {
            if (_page >= TotalCnt) return;
            Page++;
            SelectChats();
        }
        public ICommand NextCommand { get; set; }
        private void Next(object _)
        {
            if (_page - 1 <= 0) return;
            Page--;
            SelectChats();
        }
        #endregion Command
    }
}
