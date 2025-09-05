using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using talk2.Models;

namespace talk2.Views
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MyMessageTemplate { get; set; }
        public DataTemplate OtherMessageTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var message = item as Chat;
            if (message == null)
                return base.SelectTemplate(item, container);

            return message.IsMine ? MyMessageTemplate : OtherMessageTemplate;
        }
    }

}
