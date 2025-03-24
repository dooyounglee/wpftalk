using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using talk2.Commands;
using talk2.Services;

namespace talk2.ViewModels
{
    class UserViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IUserService _userService;
        private List<string> _userList = new List<string>();

        public UserViewModel(IUserService userService)
        {
            _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;

            _userService = userService;

            LogoutCommand = new RelayCommand<object>(GoToLogout);
            GotoUserCommand = new RelayCommand<object>(GotoUser);
            GotoChatCommand = new RelayCommand<object>(GotoChat);
            GotoSettingCommand = new RelayCommand<object>(GotoSetting);

            Init();
        }

        private void Init()
        {
            Debug.WriteLine("user init");
            UserList = _userService.getUserList();
        }

        public List<string> UserList
        {
            get => _userList;
            set
            {
                _userList = value;
                OnPropertyChanged();
            }
        }

        private void GoToLogout(object _)
        {
            _mainViewModel.changeViewModel(NaviType.LoginView);
        }

        private void GotoUser(object _)
        {
            _mainViewModel.changeViewModel(NaviType.UserView);
        }

        private void GotoChat(object _)
        {
            _mainViewModel.changeViewModel(NaviType.ChatView);
        }

        private void GotoSetting(object _)
        {
            _mainViewModel.changeViewModel(NaviType.SettingView);
        }

        public ICommand LogoutCommand { get; set; }
        public ICommand GotoUserCommand { get; set; }
        public ICommand GotoChatCommand { get; set; }
        public ICommand GotoSettingCommand { get; set; }
    }
}
