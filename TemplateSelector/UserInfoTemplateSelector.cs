using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using talk2.ViewModels;

namespace talk2.Views
{
    class UserInfoTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AdminTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is NewUserViewModel vm)
                return vm.IsNotAdmin ? UserTemplate : AdminTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
