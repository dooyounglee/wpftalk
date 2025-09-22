using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using talk2.Commands;
// using talk2.Services;
// using talk2.Stores;

namespace talk2.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        // private readonly MainNavigationStore _mainNavigationStore;
        private INotifyPropertyChanged? _currentViewModel;

        private Brush _selectedColor = Brushes.AliceBlue;

        public MainViewModel()
        {
            changeViewModel(NaviType.LoginView);
        }

        public void changeViewModel(NaviType naviType)
        {
            switch (naviType)
            {
                case NaviType.LoginView:
                    CurrentViewModel = (LoginViewModel)App.Current.Services.GetService(typeof(LoginViewModel))!;
                    MenuHeight = 0;
                    break;
                case NaviType.UserView:
                    CurrentViewModel = (UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!;
                    MenuHeight = 50;
                    changeMenuColor(naviType);
                    // ((UserViewModel)CurrentViewModel).Init();
                    break;
                case NaviType.ChatView:
                    CurrentViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
                    MenuHeight = 50;
                    changeMenuColor(naviType);
                    break;
                case NaviType.SettingView:
                    CurrentViewModel = (SettingViewModel)App.Current.Services.GetService(typeof(SettingViewModel))!;
                    MenuHeight = 50;
                    changeMenuColor(naviType);
                    break;
                default:
                    return;

            }
        }

        public INotifyPropertyChanged? CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        [ObservableProperty]
        private int menuHeight = 0;

        [ObservableProperty] private Brush colorUser = null;
        [ObservableProperty] private Brush colorChat = null;
        [ObservableProperty] private Brush colorSetting = null;

        private void changeMenuColor(NaviType type)
        {
            ColorUser = type == NaviType.UserView ? _selectedColor : null;
            ColorChat = type == NaviType.ChatView ? _selectedColor : null;
            ColorSetting = type == NaviType.SettingView ? _selectedColor : null;
        }

        [RelayCommand]
        private void GotoUser(object _)
        {
            changeViewModel(NaviType.UserView);
        }

        [RelayCommand]
        private void GotoChat(object _)
        {
            changeViewModel(NaviType.ChatView);
        }

        [RelayCommand]
        private void GotoSetting(object _)
        {
            changeViewModel(NaviType.SettingView);
        }
    }
}
