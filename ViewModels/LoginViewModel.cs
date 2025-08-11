using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
// using talk2.Services;

namespace talk2.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;

            LoginCommand = new RelayCommand<object>(DoLogin);
        }

        private async void DoLogin(object _)
        {
            User user = await _userService.login(Id, Pw);
            if (user is null) return;

            var _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;
            _mainViewModel.changeViewModel(NaviType.UserView);
            GoToUser(_);
        }

        private void GoToUser(object _)
        {
            var _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;
            _mainViewModel.changeViewModel(NaviType.UserView);
        }

        private string _id = "";
        public string Id { get => _id; set { _id = value; } }
        private string _pw;
        public string Pw { get => _pw; set { _pw = value; } }
        private string _ip;
        public string Ip { get => _ip; set { _ip = value; } }
        private string _port;
        public string Port { get => _port; set { _port = value; } }

        public ICommand LoginCommand { get; set; }
    }
}
