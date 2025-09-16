using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Settings
{
    public class LoginSettings
    {
        public string SavedID { get; set; } = "";
        public bool IsSaveID { get; set; } = false;
        public bool IsAutoLogin { get; set; } = false;
        public string Ip { get; set; } = "";
        public string Port { get; set; } = "";
    }
}
