using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Models
{
    public class Chat
    {
        public int ChatNo { get; set; }
        public int UsrNo { get; set; }
        public string chat { get; set; }
        public string Align { get; set; }
        public string ChatFg { get; set; }
        public string Layout
        {
            get => $"{UsrNo}: {chat}";
        }
    }
}
