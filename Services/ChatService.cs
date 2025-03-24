using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace talk2.Services
{
    public interface IChatService
    {
        public List<string> getChatList();
    }

    public class ChatService : IChatService
    {
        public List<string> getChatList()
        {
            List<string> chatList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                chatList.Add("채팅방" + i);
            }
            return chatList;
        }
    }
}
