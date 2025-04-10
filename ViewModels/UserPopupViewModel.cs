using System;
using System.Collections.Generic;
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
    public class UserPopupViewModel
    {
        private readonly IUserService _userService;
        private readonly Window _userPopupView;

        public UserPopupViewModel(Window userPopupView, IUserService userService)
        {
            _userService = userService;
            _userPopupView = userPopupView;

            CloseCommand = new RelayCommand<object>(Close);

            _userList = _userService.getUserList();
        }

        private List<User> _userList;
        public List<User> UserList { get => _userList; }

        public List<User> _selectedList = new List<User>();
        public List<User> SelectedList { get => _selectedList; }

        public ICommand CloseCommand { get; set; }
        private void Close(object _)
        {
            _userPopupView.Close();
        }
    }
}
