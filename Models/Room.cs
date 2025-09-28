using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using talkLib.Util;

namespace talk2.Models
{
    public class Room
    {
        [JsonPropertyName("roomNo")]
        public int RoomNo { get; set; }
        [JsonPropertyName("usrNo")]
        public int UsrNo { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("chat")]
        public string Chat { get; set; }
        [JsonPropertyName("rgtDtm")]
        public string RgtDtm { get; set; }
        public string Layout_Date { get => string.IsNullOrEmpty(RgtDtm) ? "" : DateUtil.TimeWhenToday(RgtDtm); }
        [JsonPropertyName("cntUnread")]
        public int CntUnread { get; set; }
        public Visibility HasUnreadCnt { get => CntUnread > 0 ? Visibility.Visible : Visibility.Hidden; }

        public string Layout
        {
            get => $"{Title}" + (CntUnread > 0 ? $"[{CntUnread}]" : "") + $"\n{Chat} ({RgtDtm})";
        }
    }
}
