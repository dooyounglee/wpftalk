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
        public string Title { get; set; }

        public Room(int RoomNo, string Title)
        {
            this.RoomNo = RoomNo;
            this.Title = Title;
        }
    }
}
