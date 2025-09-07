using OTILib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace talk2.Models
{
    public class User
    {
        [JsonPropertyName("usrNo")]
        public int UsrNo { get; set; }
        [JsonPropertyName("usrId")]
        public string UsrId { get; set; }
        [JsonPropertyName("usrNm")]
        public string UsrNm { get; set; }
        public byte[]? Password { get; set; }
        public string? Password1 { get; set; }
        [JsonPropertyName("divNo")]
        public int DivNo { get; set; }
        [JsonPropertyName("divNm")]
        public string DivNm { get; set; }
        public string Phone { get; set; }
        public string Level { get; set; }
        public string Tel { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        [JsonPropertyName("port")]
        public int Port { get; set; }
        [JsonPropertyName("profileNo")]
        public int ProfileNo { get; set; }
        public ConnState ConnState { get; set; }
        public Boolean IsAdmin { get; set; } = false;
        public string Layout
        {
            get => $"{UsrNm} ({UsrNo}) [{ConnState}]";
        }
    }
}
