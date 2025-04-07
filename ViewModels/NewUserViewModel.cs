using OTILib.Util;
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
    public class NewUserViewModel
    {
        private readonly IUserService _userService;

        private User _me = new User();
        public User Me { get => _me; }

        public NewUserViewModel(IUserService userService)
        {
            _userService = userService;
            SaveCommand = new RelayCommand<object>(DoSave);
        }

        public ICommand SaveCommand { get; set; }
        private void DoSave(object _)
        {
            Me.Password1 = "1";
            _userService.save(Me);
        }
    }
}
