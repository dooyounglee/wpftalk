using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using talk2.Commands;
using talk2.Models;
using talk2.Services;

namespace talk2.ViewModels
{
    public class UserPopupViewModel
    {
        private readonly IUserService _userService;
        public UserPopupViewModel(IUserService userService)
        {
            _userService = userService;

            _userList = _userService.getUserList();
        }

        private List<User> _userList;
        public List<User> UserList { get => _userList; }
    }
}
