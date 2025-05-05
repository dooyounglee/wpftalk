using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Models
{
    public class File
    {
        public int FileNo { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string OriginName { get; set; }
        public byte[] Buffer { get; set; }
    }
}
