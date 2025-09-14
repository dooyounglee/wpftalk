using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace talk2.ViewModels
{
    /// <summary>
    /// Interaction logic for ImagePreviewView.xaml
    /// </summary>
    public partial class ImagePreviewView : Window
    {
        private const double ZoomFactor = 1.1;
        private const double MinScale = 0.1;
        private const double MaxScale = 10.0;

        public ImagePreviewView()
        {
            InitializeComponent();
        }

        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double zoom = e.Delta > 0 ? ZoomFactor : 1 / ZoomFactor;

                double newScaleX = imageScale.ScaleX * zoom;
                double newScaleY = imageScale.ScaleY * zoom;

                if (newScaleX < MinScale || newScaleX > MaxScale)
                    return;

                imageScale.ScaleX = newScaleX;
                imageScale.ScaleY = newScaleY;

                // LayoutTransform은 레이아웃에 영향을 주므로, 별도 위치 조정 필요 없음
                e.Handled = true;
            }
        }

    }
}
