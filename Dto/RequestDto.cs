using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;

namespace talk2.Dto
{
    public class RequestDto
    {
        public class InsertChatDto
        {
            public int roomNo { get; set; }
            public int usrNo { get; set; }
            public string type { get; set; }
            public string msg { get; set; }
        }

        public class InsertChatFileDto
        {
            public int roomNo { get; set; }
            public int usrNo { get; set; }
            public string type { get; set; }
            public File file { get; set; }
        }

        public class CreateRoomDto
        {
            public List<User> userList { get; set; }
            public User me { get; set; }
        }
    }
}
