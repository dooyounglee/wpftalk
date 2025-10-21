using OTILib.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using talk2.Models;
using talk2.Util;

namespace talk2.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainView : Window
    {
        // [중복실행 방지2]이중실행 방지를 위한 DLL import
        [DllImportAttribute("user32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string clsName, string wndName);

        public MainView()
        {
            InitializeComponent();

            // [중복실행 방지2]
            // this.Title = "dlendud";
            // if (FindWindow(null, Title) > 1) Application.Current.Shutdown();

            this.Loaded += (s, e) =>
            {
                // 실행위치 저장
                PositionUtil.SavePosition(-1, this.Left, this.Top);
            };
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var roomWin = Application.Current.Windows.Cast<Window>().FirstOrDefault(p => p.Tag is not null && Convert.ToInt16(p.Tag) > 0);
            if (roomWin is not null)
            {
                MessageBox.Show("열린 채팅창이 있어요");
                roomWin.Activate();
                e.Cancel = true;
            }
        }
    }
}
