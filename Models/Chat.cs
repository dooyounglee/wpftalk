using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using talkLib.Util;

namespace talk2.Models
{
    public class Chat
    {
        [JsonPropertyName("chatNo")]
        public int ChatNo { get; set; }
        [JsonPropertyName("usrNo")]
        public int UsrNo { get; set; }
        public string UsrNm { get; set; }
        [JsonPropertyName("chat")]
        public string chat { get; set; }
        [JsonPropertyName("align")]
        public string Align { get; set; }
        [JsonPropertyName("chatFg")]
        public string ChatFg { get; set; }
        [JsonPropertyName("fileNo")]
        public int FileNo { get; set; }
        public byte[] FileBuffer { get; set; }
        public BitmapImage Image { get; set; }
        public BitmapImage ProfileImage { get; set; }

        // public string isFile { get => FileNo > 0 || FileBuffer != null ? "Visible" : "Collapsed"; }
        public string isFile { get; set; } = "Collapsed";
        public string isImage { get; set; } = "Collapsed";
        public bool IsMine { get; set; }
        [JsonPropertyName("rgtDtm")]
        public string RgtDtm { get; set; }
        public string Layout_Date { get => DateUtil.EmptyWhenSame(RgtDtm, "yyyy") + "\n" + DateUtil.EmptyWhenSame(RgtDtm, "MM-dd") + "\n" + DateUtil.format(RgtDtm, "HH:mm"); }
        public string Layout
        {
            get
            {
                switch (ChatFg)
                {
                    case "B":
                    case "C":
                    case "D": return $"{chat}";
                    default: return $"{UsrNo}: {chat}";
                }
                ;
            }
        }
    }
}
