using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Models
{
    public class Room
    {
        public int RoomNo { get; set; }
        public int UsrNo { get; set; }
        public string Title { get; set; }
        public string Chat { get; set; }
        public string RgtDtm { get; set; }
        public int CntUnread { get; set; }
        public string ColorUnreadChat { get => CntUnread > 0 ? "skyblue" : "Black"; }

        public string Layout
        {
            get => $"{Title}" + (CntUnread > 0 ? $"[{CntUnread}]" : "") + $"\n{Chat} ({RgtDtm})";
        }
    }
}
