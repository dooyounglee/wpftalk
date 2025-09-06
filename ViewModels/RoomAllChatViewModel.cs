using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Util;
using talkLib.Util;

namespace talk2.ViewModels
{
    public partial class RoomAllChatViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IChatService _chatService;

        private Window _roomAllChatView;
        private int _roomNo;
        [ObservableProperty]
        public int page = 1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PageCnt))]
        public int totalCnt;
        public int PageCnt => (int)Math.Ceiling(totalCnt / 10d);

        public RoomAllChatViewModel(int roomNo, Window roomAllChatView, IUserService userService, IChatService chatService)
        {
            _userService = userService;
            _chatService = chatService;

            _roomNo = roomNo;
            _roomAllChatView = roomAllChatView;
        }

        public async Task InitAsync()
        {
            await LoadChatsAsync();
        }

        public ObservableCollection<Chat> Chats { get; } = new();
        private async Task LoadChatsAsync()
        {
            // 전체페이지 목록만들기
            TotalCnt = await _chatService.CountChats(_roomNo);

            // 채팅목록 뿌리기
            var chats = await _chatService.SelectChats(_roomNo, Page);
            ChatUtil.Chats(Chats, chats, _userService.Me.UsrNo);
        }

        #region Command
        [RelayCommand]
        private async Task Prev()
        {
            if (Page >= PageCnt) return;
            Page++;
            await LoadChatsAsync();
        }
        [RelayCommand]
        private async Task Next()
        {
            if (Page - 1 <= 0) return;
            Page--;
            await LoadChatsAsync();
        }
        #endregion Command
    }
}
