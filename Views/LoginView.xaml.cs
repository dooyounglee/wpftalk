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

namespace talk2.Views
{
    /// <summary>
    /// LoginView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Id.Focus();
        }

        private void Id_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Id.SelectAll();  // 전체 텍스트 선택
        }

        private void Pw_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Pw.SelectAll();  // 전체 텍스트 선택
        }

        private void IpBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            IpBox.SelectAll();  // 전체 텍스트 선택
        }

        private void PortBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PortBox.SelectAll();  // 전체 텍스트 선택
        }
    }
}
