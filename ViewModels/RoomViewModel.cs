using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.ViewModels
{
    public class RoomViewModel : ViewModelBase
    {
        public int RoomNo { get; set; }
        public RoomViewModel(int roomNo)
        {
            RoomNo = roomNo;
        }
    }
}
