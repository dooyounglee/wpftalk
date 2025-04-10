using OTILib.Util;
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
using talk2.Models;
using talk2.ViewModels;

namespace talk2.Views
{
    /// <summary>
    /// UserPopupView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserPopupView : Window
    {
        public UserPopupView()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var x in e.AddedItems)
            {
                ((UserPopupViewModel)this.DataContext).SelectedList.Add((User)x);
            }
        }
    }
}
