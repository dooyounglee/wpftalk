using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
        [JsonPropertyName("cntUnread")]
        public int CntUnread { get; set; }
        public string ColorUnreadChat { get => CntUnread > 0 ? "skyblue" : "Black"; }

        public string Layout
        {
            get => $"{Title}" + (CntUnread > 0 ? $"[{CntUnread}]" : "") + $"\n{Chat} ({RgtDtm})";
        }
    }
}
