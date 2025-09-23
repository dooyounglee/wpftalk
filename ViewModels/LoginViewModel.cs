using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Settings;

namespace talk2.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;

            Init();
        }

        public async Task Init()
        {
            var settings = LoginSettingsManager.Load();
            Id = settings.SavedID;
            Pw = "";
            IsCheckedSaveId = settings.IsSaveID;
            IsCheckedAutoLogin = settings.IsAutoLogin;
            Ip = settings.Ip;
            Port = settings.Port;

            // 자동로그인
            if (IsCheckedAutoLogin)
            {
                User user = await _userService.login(Id);
                if (user is null) return;

                // 바로 GoToUser() 호출 하니까, thread가 꼬인다고 하여 아래처럼 호출하라 함
                Application.Current.Dispatcher.Invoke(() => GoToUser()); // GoToUser();
            }
        }

        [ObservableProperty] public bool isCheckedSaveId;
        [ObservableProperty] public bool isCheckedAutoLogin;
        [ObservableProperty] public bool isEnabledAutoLogin;
        partial void OnIsCheckedSaveIdChanged(bool value)
        {
            if (value)
            {
                IsEnabledAutoLogin = true;
            }
            else
            {
                IsCheckedAutoLogin = false;
                IsEnabledAutoLogin = false;
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            var settings = new LoginSettings();

            // setting에 자동로그인 체크없었으면 비번 필수
            if (Pw is null || Pw.Trim().Length == 0) return;

            User user = await _userService.login(Id, Pw);
            if (user is null) return;

            settings.SavedID = IsCheckedSaveId ? Id : "";
            settings.IsSaveID = IsCheckedSaveId;
            settings.IsAutoLogin = IsCheckedAutoLogin;
            settings.Ip = Ip;
            settings.Port = Port;
            LoginSettingsManager.Save(settings);

            GoToUser();
        }

        private void GoToUser()
        {
            var _mainViewModel = (MainViewModel)App.Current.Services.GetService(typeof(MainViewModel))!;
            _mainViewModel.changeViewModel(NaviType.UserView);
            var CurrentViewModel = (UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!;
            ((UserViewModel)CurrentViewModel).InitAsync();
            var ChatViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
            ((ChatViewModel)ChatViewModel).InitAsync();
        }

        private string _id = "";
        public string Id { get => _id; set { _id = value; } }
        private string _pw;
        public string Pw { get => _pw; set { _pw = value; } }
        private string _ip;
        public string Ip { get => _ip; set { _ip = value; } }
        private string _port;
        public string Port { get => _port; set { _port = value; } }
    }
}
