using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using talk2.Models;
using talkLib.Util;

namespace talk2.Util
{
    class ChatUtil
    {
        public static void Chats(ObservableCollection<Chat> Chats, List<Chat> chats, int meNo)
        {
            chats = chats.Reverse<Chat>().ToList();
            Chats.Clear();
            foreach (var chat in chats)
            {
                chat.IsMine = chat.UsrNo == meNo;
                chat.UsrNm = UserUtil.getUsrNm(chat.UsrNo);
                switch (chat.ChatFg)
                {
                    case "A": /*chat.Align = chat.UsrNo == _userService.Me.UsrNo ? "Right" : "Left";*/ break;
                    case "B":
                    case "C": /*chat.Align = "Center";*/ break;
                    case "D": /*chat.Align = "Center";*/ break;
                    case "E":
                        // chat.Align = chat.UsrNo == _userService.Me.UsrNo ? "Right" : "Left";
                        chat.Image = ImageUtil.IsImage(chat.chat) ? new BitmapImage(ImageUtil.getImage(chat.FileNo)) : null;
                        chat.isImage = ImageUtil.IsImage(chat.chat) ? "Visible" : "Collapsed";
                        chat.isFile = "Visible";
                        break;
                }
                Chats.Add(chat);
            }
        }
    }
}
