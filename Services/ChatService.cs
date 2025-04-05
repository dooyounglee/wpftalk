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
    }
}
