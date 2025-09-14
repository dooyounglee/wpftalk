using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace talk2.ViewModels
{
    public partial class ImagePreviewViewModel : ObservableObject
    {
        public BitmapImage Image { get; set; }

        public ImagePreviewViewModel(BitmapImage b)
        {
            Image = b;
        }
    }
}
