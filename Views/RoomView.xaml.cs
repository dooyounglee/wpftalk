using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using talk2.Commands;
using talk2.ViewModels;

namespace talk2.Views
{
    /// <summary>
    /// RoomView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RoomView : Window
    {
        // private FlashWindow FlashWindow;
        public RoomView()
        {
            InitializeComponent();
            // var chatViewModel = this.DataContext as ChatViewModel;
            this.Closed += new EventHandler(OnClose);
        }

        private void OnClose(object sender, EventArgs e)
        {
            var roomViewModel = this.DataContext as RoomViewModel;
            roomViewModel.Disconnected();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Debug.WriteLine("deactivated");
            FlashWindow flashWindow = new FlashWindow(this);
            flashWindow.FlashApplicationWindow();
        }
    }
}
