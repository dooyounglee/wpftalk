using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using talk2.Models;
using talk2.Services;
using talk2.Views;

namespace talk2.ViewModels
{
    public class RoomUserPopupViewModel
    {
        private readonly IChatService _chatService;
        private readonly Window _roomUserPopupView;
        private readonly int _roomNo;

        public RoomUserPopupViewModel(int roomNo, Window roomUserPopupView, IChatService chatService)
        {
            _chatService = chatService;
            _roomUserPopupView = roomUserPopupView;
            _roomNo = roomNo;

            _userList = _chatService.RoomUserList(_roomNo);
        }

        private List<User> _userList;
        public List<User> UserList { get => _userList; }
    }
}
