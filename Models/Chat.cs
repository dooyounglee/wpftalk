using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace talk2.Models
{
    public class Chat
    {
        public int ChatNo { get; set; }
        public int UsrNo { get; set; }
        public string chat { get; set; }
        public string Align { get; set; }
        public string ChatFg { get; set; }
        public int FileNo { get; set; }
        public byte[] FileBuffer { get; set; }
        public BitmapImage Image { get; set; }

        // public string isFile { get => FileNo > 0 || FileBuffer != null ? "Visible" : "Collapsed"; }
        public string isFile { get; set; } = "Collapsed";
        public string isImage { get; set; } = "Collapsed";
        public string Layout
        {
            get => $"{UsrNo}: {chat}";
        }
    }
}
