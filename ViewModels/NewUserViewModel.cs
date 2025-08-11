﻿using OTILib.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using talk2.Commands;
using talk2.Models;
using talk2.Services;
using talk2.Views;

namespace talk2.ViewModels
{
    public class NewUserViewModel : INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly Window _userInfoView;

        public string Visibility_save { get => _userService.Me.IsAdmin ? "Visible" : "Hidden"; }
        public Boolean IsNotAdmin { get => !_userService.Me.IsAdmin; }

        private User _he = new User();
        public User He
        {
            get => _he;
            set
            {
                _he = value;
                OnPropertyChanged(nameof(He));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NewUserViewModel(Window userInfoView, IUserService userService)
        {
            _userService = userService;
            _userInfoView = userInfoView;
        }

        public async void InitAsync(int usrNo)
        {
            He = await _userService.getUser(usrNo);
        }

        public ICommand SaveCommand
        {
            get => new RelayCommand<object>((_) =>
            {
                He.Password1 = "1";
                _userService.save(He);
            });
        }
        public ICommand CloseCommand
        {
            get => new RelayCommand<object>((_) =>
            {
                _userInfoView.Close();
            });
        }
    }
}
