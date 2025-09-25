using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OTILib.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Views;
using talkLib.Util;

namespace talk2.ViewModels
{
    public partial class NewUserViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly Window _userInfoView;

        public string Visibility_save { get => _userService.Me.IsAdmin ? "Visible" : "Hidden"; }
        public Boolean IsNotAdmin { get => !_userService.Me.IsAdmin; }

        [ObservableProperty]
        private User he = new User();

        [ObservableProperty]
        private BitmapImage profileImage;

        [ObservableProperty]
        private Visibility isMe;

        public NewUserViewModel(Window userInfoView, IUserService userService)
        {
            _userService = userService;
            _userInfoView = userInfoView;
        }

        public async void InitAsync(int usrNo)
        {
            He = await _userService.getUser(usrNo);
            ProfileImage = He.ProfileNo > 0 ? new BitmapImage(ImageUtil.getImage(He.ProfileNo)) : ProfileUtil.getDefault();
            IsMe = usrNo == _userService.Me.UsrNo ? Visibility.Visible : Visibility.Hidden;
        }

        [RelayCommand]
        private async Task Save()
        {
            He.Password1 = "1";
            _userService.save(He);
        }
        [RelayCommand]
        private async Task Close()
        {
            _userInfoView.Close();
        }

        private string _profilePath;
        [RelayCommand]
        private async Task LoadProfile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.*)|*.png|*.bmp|*.jpeg|*.gif|*.tiff";
            if (dialog.ShowDialog() == true)
            {
                _profilePath = dialog.FileName;
                ProfileImage = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        [RelayCommand]
        private async Task SaveProfile()
        {
            FileInfo fi = new FileInfo(_profilePath);
            Models.File f = new Models.File()
            {
                OriginName = fi.Name,
                Buffer = System.IO.File.ReadAllBytes(_profilePath),
            };
            await _userService.saveProfile(f);

            // 프사 바꾼거 전파
            var UserViewModel = (UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!;
            UserViewModel.SendReloadProfile();
        }

        [RelayCommand]
        private async Task DeleteProfile()
        {
            _userService.deleteProfile();
            ProfileImage = ProfileUtil.getDefault();

            // 프사 바꾼거 전파
            var UserViewModel = (UserViewModel)App.Current.Services.GetService(typeof(UserViewModel))!;
            UserViewModel.SendReloadProfile();
        }
    }
}
