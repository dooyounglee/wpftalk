using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using talk2.Commands;
// using talk2.Services;
// using talk2.Stores;

namespace talk2.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // private readonly MainNavigationStore _mainNavigationStore;
        private INotifyPropertyChanged? _currentViewModel;

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
                    break;
                case NaviType.UserView:
                    CurrentViewModel = (UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!;
                    ((UserViewModel)CurrentViewModel).Init();
                    break;
                case NaviType.ChatView:
                    CurrentViewModel = (ChatViewModel)App.Current.Services.GetService(typeof(ChatViewModel))!;
                    break;
                case NaviType.SettingView:
                    CurrentViewModel = (SettingViewModel)App.Current.Services.GetService(typeof(SettingViewModel))!;
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
    }
}
