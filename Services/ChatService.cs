using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;
using talk2.Repositories;

namespace talk2.Services
{
    public interface IChatService
    {
        public List<Room> getChatList(int usrNo);
        public void InsertChat(int roomNo, int usrNo, string msg);
        public List<Chat> SelectChats(int roomNo);
    }

    public class ChatService : IChatService
    {
        private readonly IChatRepository? _chatRepository;

        public ChatService(IChatRepository? chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public List<Room> getChatList(int usrNo)
        {
            return _chatRepository.GetRoomList(usrNo);
        }

        public void InsertChat(int roomNo, int usrNo, string msg)
        {
            _chatRepository.InsertChat(roomNo, usrNo, msg);
        }

        public List<Chat> SelectChats(int roomNo)
        {
            return _chatRepository.SelectChats(roomNo);
        }
    }
}
