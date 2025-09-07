using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using talk2.Models;
using talk2.Repositories;
using talkLib.Util;

namespace talk2.Services
{
    public interface IUserService
    {
        public Task<User> login(string id, string pw);
        public Task<List<User>> getUserList();
        public Task<User> getUser(int usrNo);

        public User Me { get; }
        public void logout();
        public void save(User user);
        public void saveProfile(File file);
        public void deleteProfile();
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private User _user;
        public User Me { get => _user; }

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> login(string id, string pw)
        {
            // _user = _userRepository.login("user" + id, "");
            // _user.Ip = "127.0.0.1";
            // _user.Port = 8080;
            // if (_user.UsrNo == 0) _user.IsAdmin = true;
            // return _user;
            string responseBody = await HttpUtil.Post("/user/login", new { usrId = id, password = pw });
            _user = JsonUtil.StringToObject<User>(responseBody);
            if (_user.UsrNo == 0) _user.IsAdmin = true;
            return _user;
        }

        public async Task<List<User>> getUserList()
        {
            // return _userRepository.getUserList();
            string responseBody = await HttpUtil.Get("/user/list");
            return JsonUtil.StringToObject<List<User>>(responseBody);
        }

        public async Task<User> getUser(int usrNo)
        {
            // return _userRepository.findById(usrNo);
            string responseBody = await HttpUtil.Get($"/user/{usrNo}");
            return JsonUtil.StringToObject<User>(responseBody);
        }

        public void logout()
        {
            _user = null;
        }

        public async void save(User user)
        {
            // _userRepository.save(user);
            await HttpUtil.Post("/user/create", user);
            // return JsonUtil.StringToObject<User>(responseBody);
        }

        public async void saveProfile(File file)
        {
            var dto = new
            {
                usrNo = Me.UsrNo
            };
            await HttpUtil.Post($"/user/profile", dto, file.Buffer, file.OriginName);
        }

        public async void deleteProfile()
        {
            await HttpUtil.Delete("/user/profile", new { usrNo = Me.UsrNo });
        }
    }
}
