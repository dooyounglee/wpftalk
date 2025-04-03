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
        public void login(string id, string pw);
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
        public void login(string id, string pw)
        {
            Debug.WriteLine(id);
            Debug.WriteLine(pw);
            _userRepository.login();
            _user = new()
            {
                UsrNo = int.Parse(id),
                UsrId = "user" + id,
                UsrNm = "이두영" + id,
                DivNo = 1,
                DivNm = "전략사업2 Div.",
                Ip = "127.0.0.1",
                Port = 8080,
            };
        }

        public List<User> getUserList()
        {
            return _userRepository.getUserList();
        }
    }
}
