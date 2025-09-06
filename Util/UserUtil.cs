using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;

namespace talk2.Util
{
    class UserUtil
    {
        private static Dictionary<int, User> _cache = new Dictionary<int, User>();

        public static void setUsers(List<User> users)
        {
            foreach (var user in users)
            {
                _cache.Add(user.UsrNo, user);
            }
        }

        public static User getUser(int usrNo) => _cache.GetValueOrDefault(usrNo);

        public static string getUsrNm(int usrNo) => getUser(usrNo).UsrNm;
    }
}
