using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;

namespace talk2.Services
{
    public interface IChatService
    {
        public List<Room> getChatList();
    }

    public class ChatService : IChatService
    {
        public List<Room> getChatList()
        {
            List<Room> chatList = new List<Room>();
            for (int i = 0; i < 10; i++)
            {
                chatList.Add(new Room(i, "채팅방" + i));
            }
            return chatList;
        }
    }
}
