﻿using System;
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
        public Task<List<Chat>> SelectChats(int roomNo, int page);
        public Task<int> CountChats(int roomNo);
        public Task<Room> CreateRoom(List<User> userList);
        [Obsolete]  public string Invite(int roomNo, List<User> userList);
        public Task<Room> Invite(int roomNo, List<int> userList, string invitedUsers);
        public Task<string> Leave(int roomNo, int usrNo);
        public Task<List<User>> RoomUserList(int roomNo);
        public Task<int> CountRoomWithMe(int usrNo);
        [Obsolete] public bool IsThereSomeoneinRoom(int roomNo, List<User> userList);
        public Task<bool> IsThereSomeoneinRoom(int roomNo, List<int> userList);
        public Task EditTitle(int roomNo, int usrNo, string title);
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
                meUsrNo = _userService.Me.UsrNo
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
            var dto = new {
                roomNo = roomNo,
                usrNo = usrNo,
                type = type,
                meUsrNo = _userService.Me.UsrNo
            };
            string responseBody = await HttpUtil.Post($"/chat/insert1", dto, file.Buffer, file.OriginName);
            return JsonUtil.StringToObject<int>(responseBody);
        }

        public async Task<List<Chat>> SelectChats(int roomNo)
        {
            // return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo);
            string responseBody = await HttpUtil.Get($"/chat/list?roomNo={roomNo}&usrNo={_userService.Me.UsrNo}");
            return JsonUtil.StringToObject<List<Chat>>(responseBody);
        }
        public async Task<List<Chat>> SelectChats(int roomNo, int page)
        {
            // return _chatRepository.SelectChats(roomNo, _userService.Me.UsrNo, page);
            string responseBody = await HttpUtil.Get($"/chat/list?roomNo={roomNo}&usrNo={_userService.Me.UsrNo}&page={page}");
            return JsonUtil.StringToObject<List<Chat>>(responseBody);
        }
        public async Task<int> CountChats(int roomNo)
        {
            // return _chatRepository.CountChats(roomNo);
            string responseBody = await HttpUtil.Get($"/chat/count/{roomNo}");
            return JsonUtil.StringToObject<int>(responseBody);
        }

        public async Task<Room> CreateRoom(List<User> userList)
        {
            // int newRoomNo = _chatRepository.GetRoomNo();
            // 
            // string title = _userService.Me.UsrNm;
            // foreach (User u in userList)
            // {
            //     if (_userService.Me.UsrNo != u.UsrNo)
            //     {
            //         title += "," + u.UsrNm;
            //     }
            // }
            // 
            // userList.Add(_userService.Me);
            // 
            // // 방만들기
            // _chatRepository.AddRoom(new Room()
            // {
            //     RoomNo = newRoomNo,
            //     UsrNo = _userService.Me.UsrNo,
            //     Title = title,
            // });
            // 
            // // 방-유저 연결하기
            // foreach (User user in userList)
            // {
            //     _chatRepository.AddRoomUser(new Room()
            //     {
            //         RoomNo = newRoomNo,
            //         UsrNo = user.UsrNo,
            //         Title = title,
            //     });
            // }
            // 
            // // 방만들었따는 채팅
            // InsertChat(newRoomNo, _userService.Me.UsrNo, "B", $"{_userService.Me.UsrNm}님이 방을 만들었다");
            // 
            // return newRoomNo;
            string responseBody = await HttpUtil.Post($"/room/create", new { userList=userList, me= _userService.Me });
            return JsonUtil.StringToObject<Room>(responseBody);
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

        public async Task<Room> Invite(int roomNo, List<int> usrNoList, string invitedUsers)
        {

            // // 방-유저 연결하기
            // foreach (int usrNo in userList)
            // {
            //     _chatRepository.AddRoomUser(new Room()
            //     {
            //         RoomNo = roomNo,
            //         UsrNo = usrNo,
            //         Title = $"{_userService.Me.UsrNm},{invitedUsers}",
            //     });
            // }
            // 
            // var msg = $"{_userService.Me.UsrNm}님이 {invitedUsers}님을 초대했다";
            // 
            // InsertChat(roomNo, _userService.Me.UsrNo, "C", msg);
            // 
            // return msg;
            var usrNoListString = string.Join(",", usrNoList);
            string responseBody = await HttpUtil.Get($"/chat/invite/{roomNo}?usrNos={usrNoListString}&usrNms={invitedUsers}&meNo={_userService.Me.UsrNo}&meNm={_userService.Me.UsrNm}");
            return JsonUtil.StringToObject<Room>(responseBody);
        }

        public async Task<string> Leave(int roomNo, int usrNo)
        {
            // _chatRepository.LeaveRoom(roomNo, usrNo);

            // var msg = $"{_userService.Me.UsrNm}님이 나갔다";

            // InsertChat(roomNo, _userService.Me.UsrNo, "D", msg);

            // return msg;
            var msg = $"{_userService.Me.UsrNm}님이 나갔다";
            string responseBody = await HttpUtil.Post($"/room/leave", new { roomNo = roomNo, usrNo = usrNo, msg = msg });
            // JsonUtil.StringToObject<string>(responseBody);
            return msg;
        }

        public async Task<List<User>> RoomUserList(int roomNo)
        {
            // return _chatRepository.SelectRoomUserList(roomNo);
            string responseBody = await HttpUtil.Get($"/room/users?roomNo={roomNo}");
            return JsonUtil.StringToObject<List<User>>(responseBody);
        }

        public async Task<int> CountRoomWithMe(int usrNo)
        {
            // return _chatRepository.CountRoomWithMe(_userService.Me.UsrNo, usrNo);
            string responseBody = await HttpUtil.Get($"/room/count?usrNo={usrNo}&meNo={_userService.Me.UsrNo}");
            return JsonUtil.StringToObject<int>(responseBody);
        }

        [Obsolete] public bool IsThereSomeoneinRoom(int roomNo, List<User> userList)
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
        public async Task<bool> IsThereSomeoneinRoom(int roomNo, List<int> usrNoList)
        {
            var usrNoListString = string.Join(",", usrNoList);
            string responseBody = await HttpUtil.Get($"/chat/isThereTheyinRoom/{roomNo}?usrNos={usrNoListString}");
            return JsonUtil.StringToObject<bool>(responseBody);
        }

        public async Task EditTitle(int roomNo, int usrNo, string title)
        {
            // _chatRepository.UpdateTitle(roomNo, usrNo, title);
            await HttpUtil.Put($"/room/title", new { roomNo=roomNo, usrNo=usrNo, title=title });
        }

        public async Task ReadChat(int roomNo, int usrNo)
        {
            // _chatRepository.ReadChat(roomNo, usrNo);
            await HttpUtil.Get($"/chat/read/{roomNo}?usrNo={usrNo}");
        }
    }
}
