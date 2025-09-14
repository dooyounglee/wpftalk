using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace talk2.Models
{
    public class Div
    {
        [JsonPropertyName("divNo")]
        public int DivNo { get; set; }
        [JsonPropertyName("divNm")]
        public string DivNm { get; set; }
    }
}
