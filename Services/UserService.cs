using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;
using talk2.Repositories;

namespace talk2.Services
{
    public interface IUserService
    {
        public User login(string id, string pw);
        public List<User> getUserList();

        public User Me { get; }
        public void logout();
        public void save(User user);
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
        public User login(string id, string pw)
        {
            _user = _userRepository.login("user" + id, "");
            _user.Ip = "127.0.0.1";
            _user.Port = 8080;
            return _user;
        }

        public List<User> getUserList()
        {
            return _userRepository.getUserList();
        }

        public void logout()
        {
            _user = null;
        }

        public void save(User user)
        {
            _userRepository.save(user);
        }
    }
}
