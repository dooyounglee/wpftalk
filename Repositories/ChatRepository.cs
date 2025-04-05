using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;
using OTILib.DB;

namespace talk2.Repositories
{
    public interface IChatRepository
    {
        public List<Room>? GetRoomList(int usrNo);
        public int GetRoomNo();
        public void AddRoom(Room room);
        public void LeaveRoom(int roomNo, int usrNo);
        public void UpdateTitle(int? roomNo, int usrNo, string title);
        public void InsertChat(int roomNo, int usrNo, string msg);
        public List<Chat> SelectChats(int roomNo);
    }

    internal class ChatRepository : IChatRepository
    {
        public List<Room>? GetRoomList(int usrNo)
        {
            string sql = @$"SELECT a.room_no
                                 , a.title
                                 , b.chat
                                 , coalesce(b.rgt_dtm, a.rgt_dtm) as rgt_dtm
                              FROM kakao.room a
                              left join (select room_no
                                              , chat
                                              , rgt_dtm
                                           from kakao.chat
                                          where (room_no, chat_no) in (select room_no, max(chat_no) as chat_no
                                                                         from kakao.chat
                                                                        group by room_no)
                                        ) b
                                on (a.room_no = b.room_no)
                             where a.usr_no = {usrNo}
                             order by rgt_dtm desc";
            DataTable? dt = Query.select1(sql);

            var rooms = new List<Room>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                rooms.Add(new Room()
                {
                    RoomNo = (int)(long)dt.Rows[i]["room_no"],
                    Title = (string)dt.Rows[i]["title"],
                    Chat = dt.Rows[i].IsNull("chat") ? "" : (string)dt.Rows[i]["chat"],
                    RgtDtm = dt.Rows[i].IsNull("rgt_dtm") ? "" : (string)dt.Rows[i]["rgt_dtm"],
                });
            };

            return rooms;
        }

        public int GetRoomNo()
        {
            string sql = @$"SELECT COALESCE(MAX(room_no),0)+1 as room_no
                              FROM kakao.room";
            DataTable? dt = Query.select1(sql);

            var roomNo = 0;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                roomNo = (int)(long)dt.Rows[0]["room_no"];
            };

            return roomNo;
        }

        public void AddRoom(Room room)
        {
            string sql = @$"INSERT INTO kakao.room (ROOM_NO,USR_NO,TITLE,RGT_DTM) VALUES
                           ('{room.RoomNo}',{room.UsrNo},'{room.Title}',to_char(now() + interval '9 hour','YYYYMMDDHH24MISS'))";
            Query.insert(sql);
        }

        public void LeaveRoom(int roomNo, int usrNo)
        {
            string sql = @$"DELETE FROM kakao.room
                             WHERE ROOM_NO = {roomNo}
                               AND USR_NO = {usrNo}"
                         ;
            Query.insert(sql);
        }

        public void UpdateTitle(int? roomNo, int usrNo, string title)
        {
            string sql = @$"UPDATE kakao.room
                               SET TITLE = '{title}'
                             WHERE ROOM_NO = {roomNo}
                               AND USR_NO = {usrNo}"
                         ;
            Query.insert(sql);
        }

        public void InsertChat(int roomNo, int usrNo, string msg)
        {
            string sql = @$"INSERT INTO kakao.chat
                            (CHAT_NO,ROOM_NO,USR_NO,CHAT_FG,CHAT,RGT_DTM) VALUES
                            ((SELECT COALESCE(MAX(chat_no),0)+1 FROM kakao.chat),
                            '{roomNo}',{usrNo},'A','{msg}',to_char(now() + interval '9 hour','YYYYMMDDHH24MISS'))"
                         ;
            Query.insert(sql);
        }

        public List<Chat> SelectChats(int roomNo)
        {
            string sql = @$"SELECT a.chat_no
                                 , a.chat
                                 , a.usr_no
                              FROM kakao.chat a
                             where a.room_no = {roomNo}
                             order by chat_no";
            DataTable? dt = Query.select1(sql);

            var chats = new List<Chat>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                chats.Add(new Chat()
                {
                    ChatNo = (int)(long)dt.Rows[i]["chat_no"],
                    UsrNo = (int)(long)dt.Rows[i]["usr_no"],
                    chat = dt.Rows[i].IsNull("chat") ? "" : (string)dt.Rows[i]["chat"],
                });
            };

            return chats;
        }
    }
}
