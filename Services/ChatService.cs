using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using talk2.Models;
using talk2.Repositories;

namespace talk2.Services
{
    public interface IChatService
    {
        public List<Room> getChatList(int usrNo);
        public void InsertChat(int roomNo, int usrNo, string type, string msg);
        public List<Chat> SelectChats(int roomNo);
        public int CreateRoom(List<User> userList);
        public string Invite(int roomNo, List<User> userList);
    }

    public class ChatService : IChatService
    {
        private readonly IChatRepository? _chatRepository;
        private readonly IUserService _userService;

        public ChatService(IChatRepository? chatRepository, IUserService userService)
        {
            _chatRepository = chatRepository;
            _userService = userService;
        }

        public List<Room> getChatList(int usrNo)
        {
            return _chatRepository.GetRoomList(usrNo);
        }

        public void InsertChat(int roomNo, int usrNo, string type, string msg)
        {
            _chatRepository.InsertChat(roomNo, usrNo, type, msg);
        }

        public List<Chat> SelectChats(int roomNo)
        {
            return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo);
        }

        public int CreateRoom(List<User> userList)
        {
            int newRoomNo = _chatRepository.GetRoomNo();
            
            string title = _userService.Me.UsrNm;
            foreach (User u in userList)
            {
                if (_userService.Me.UsrNo != u.UsrNo)
                {
                    title += "," + u.UsrNm;
                }
            }

            userList.Add(_userService.Me);

            // 방만들기
            _chatRepository.AddRoom(new Room()
            {
                RoomNo = newRoomNo,
                UsrNo = _userService.Me.UsrNo,
                Title = title,
            });

            // 방-유저 연결하기
            foreach (User user in userList)
            {
                _chatRepository.AddRoomUser(new Room()
                {
                    RoomNo = newRoomNo,
                    UsrNo = user.UsrNo,
                    Title = title,
                });
            }

            // 방만들었따는 채팅
            InsertChat(newRoomNo, _userService.Me.UsrNo, "B", $"{_userService.Me.UsrNm}님이 방을 만들었다");

            return newRoomNo;
        }

        public string Invite(int roomNo, List<User> userList)
        {
            string invitedUsers = string.Join(",", userList.Select(u => u.UsrNm));

            // 방-유저 연결하기
            foreach (User user in userList)
            {
                _chatRepository.AddRoomUser(new Room()
                {
                    RoomNo = roomNo,
                    UsrNo = user.UsrNo,
                    Title = $"{_userService.Me.UsrNm},{invitedUsers}",
                });
            }

            var msg = $"{_userService.Me.UsrNm}님이 {invitedUsers}님을 초대했다";

            InsertChat(roomNo, _userService.Me.UsrNo, "C", msg);

            return msg;
        }
    }
}
