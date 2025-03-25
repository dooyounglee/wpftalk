using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Models
{
    class User
    {
        public int UsrNo { get; set; }
        public string UsrId { get; set; }
        public string UsrNm { get; set; }
        public byte[]? Password { get; set; }
        public string? Password1 { get; set; }
        public int DivNo { get; set; }
        public string DivNm { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
    }
}
