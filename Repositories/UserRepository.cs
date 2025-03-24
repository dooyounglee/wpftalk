using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Repositories
{
    public interface IUserRepository
    {
        void login();
    }

    public class UserRepository : IUserRepository
    {
        public void login()
        {
            Debug.WriteLine("user repository");
        }
    }
}
