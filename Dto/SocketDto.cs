using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;

namespace talk2.Dto
{
    public class SocketDto
    {
        public Room Room { get; set; }
        public List<int> UsrNoList { get; set; }
    }
}
