using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Repositories;

namespace talk2.Services
{
    public interface IUserService
    {
        public void login(string id, string pw);
        public List<string> getUserList();
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void login(string id, string pw)
        {
            Debug.WriteLine(id);
            Debug.WriteLine(pw);
            _userRepository.login();
        }

        public List<string> getUserList()
        {
            List<string> userList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                userList.Add("이두영" + i);
            }
            return userList;
        }
    }
}
