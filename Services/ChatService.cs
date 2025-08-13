using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using talk2.Models;
using talk2.Repositories;
using talkLib.Util;
using static talk2.Dto.RequestDto;

namespace talk2.Services
{
    public interface IChatService
    {
        public Task<Room> getRoom(int roomNo);
        public Task<List<Room>> getChatList(int usrNo);
        public Task<int> InsertChat(int roomNo, int usrNo, string type, string msg);
        public Task<int> InsertChat(int roomNo, int usrNo, string type, File file);
        public Task<List<Chat>> SelectChats(int roomNo);
        public List<Chat> SelectChats(int roomNo, int page);
        public int CountChats(int roomNo);
        public int CreateRoom(List<User> userList);
        public string Invite(int roomNo, List<User> userList);
        public string Leave(int roomNo, int usrNo);
        public List<User> RoomUserList(int roomNo);
        public int CountRoomWithMe(int usrNo);
        public bool IsThereSomeoneinRoom(int roomNo, List<User> userList);
        public void EditTitle(int roomNo, int usrNo, string title);
        public Task ReadChat(int roomNo, int usrNo);
    }

    public class ChatService : IChatService
    {
        private readonly IChatRepository? _chatRepository;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;

        public ChatService(IChatRepository? chatRepository, IUserService userService, IFileService fileService)
        {
            _chatRepository = chatRepository;
            _userService = userService;
            _fileService = fileService;
        }

        public async Task<Room> getRoom(int roomNo)
        {
            // return _chatRepository.GetRoom(roomNo, _userService.Me.UsrNo);
            string responseBody = await HttpUtil.Get($"/room/{roomNo}?usrNo={_userService.Me.UsrNo}");
            return JsonUtil.StringToObject<Room>(responseBody);
        }

        public async Task<List<Room>> getChatList(int usrNo)
        {
            // return _chatRepository.GetRoomList(usrNo);
            string responseBody = await HttpUtil.Get($"/room/list?usrNo={usrNo}");
            return JsonUtil.StringToObject<List<Room>>(responseBody);
        }

        public async Task<int> InsertChat(int roomNo, int usrNo, string type, string msg)
        {
            // int newChatNo = _chatRepository.getNewChatNo();
            // _chatRepository.InsertChat(newChatNo, roomNo, usrNo, type, msg);
            // _chatRepository.InsertChatUserExceptMe(roomNo, _userService.Me.UsrNo, newChatNo);
            // return newChatNo;
            var dto = new
            {
                roomNo = roomNo,
                usrNo = usrNo,
                type = type,
                msg = msg,
            };
            string responseBody = await HttpUtil.Post($"/chat/insert", dto);
            return JsonUtil.StringToObject<int>(responseBody);
        }

        public async Task<int> InsertChat(int roomNo, int usrNo, string type, File file)
        {
            // int fileNo = _fileService.saveFile(file);

            // int newChatNo = _chatRepository.getNewChatNo();
            // _chatRepository.InsertChat(newChatNo, roomNo, usrNo, type, file.OriginName, fileNo);
            // _chatRepository.InsertChatUserExceptMe(roomNo, _userService.Me.UsrNo, newChatNo);
            // return newChatNo;
            var dto = new InsertChatDto
            {
                roomNo = roomNo,
                usrNo = usrNo,
                type = type,
            };
            string responseBody = await HttpUtil.Post($"/chat/insert1", dto, file.Buffer, file.OriginName);
            return JsonUtil.StringToObject<int>(responseBody);
        }

        public async Task<List<Chat>> SelectChats(int roomNo)
        {
            // return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo);
            string responseBody = await HttpUtil.Get($"/chat/list/{roomNo}?usrNo={_userService.Me.UsrNo}");
            return JsonUtil.StringToObject<List<Chat>>(responseBody);
        }
        public List<Chat> SelectChats(int roomNo, int page)
        {
            return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo, page);
        }
        public int CountChats(int roomNo)
        {
            return _chatRepository.CountChats(roomNo);
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

        public string Leave(int roomNo, int usrNo)
        {
            _chatRepository.LeaveRoom(roomNo, usrNo);

            var msg = $"{_userService.Me.UsrNm}님이 나갔다";

            InsertChat(roomNo, _userService.Me.UsrNo, "D", msg);

            return msg;
        }

        public List<User> RoomUserList(int roomNo)
        {
            return _chatRepository.SelectRoomUserList(roomNo);
        }

        public int CountRoomWithMe(int usrNo)
        {
            return _chatRepository.CountRoomWithMe(_userService.Me.UsrNo, usrNo);
        }

        public bool IsThereSomeoneinRoom(int roomNo, List<User> userList)
        {
            bool result = false;
            foreach(var u in userList)
            {
                int countHeinRoom = _chatRepository.CountHeinRoom(roomNo, u.UsrNo);
                if (countHeinRoom > 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void EditTitle(int roomNo, int usrNo, string title)
        {
            _chatRepository.UpdateTitle(roomNo, usrNo, title);
        }

        public async Task ReadChat(int roomNo, int usrNo)
        {
            // _chatRepository.ReadChat(roomNo, usrNo);
            await HttpUtil.Get($"/chat/read/{roomNo}?usrNo={usrNo}");
        }
    }
}
