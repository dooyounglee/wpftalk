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
            return _user;
        }

        public List<User> getUserList()
        {
            return _userRepository.getUserList();
        }
    }
}
