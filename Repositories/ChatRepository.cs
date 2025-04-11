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
        public void AddRoomUser(Room room);
        public void LeaveRoom(int roomNo, int usrNo);
        public void UpdateTitle(int? roomNo, int usrNo, string title);
        public void InsertChat(int roomNo, int usrNo, string type, string msg);
        public List<Chat> SelectChats(int roomNo);
    }

    internal class ChatRepository : IChatRepository
    {
        public List<Room>? GetRoomList(int usrNo)
        {
            string sql = @$"SELECT a.room_no
                                 , a.title
                                 , c.chat
                                 , coalesce(c.rgt_dtm, a.rgt_dtm) as rgt_dtm
                              FROM talk.room a
                             inner join talk.chatuser b on (a.room_no = b.room_no)
                              left join (select room_no
                                              , chat
                                              , rgt_dtm
                                           from talk.chat
                                          where (room_no, chat_no) in (select room_no, max(chat_no) as chat_no
                                                                         from talk.chat
                                                                        group by room_no)
                                        ) c
                                on (a.room_no = c.room_no)
                             where b.usr_no = {usrNo}
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
                              FROM talk.room";
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
            string sql = @$"INSERT INTO talk.room (ROOM_NO,USR_NO,TITLE,RGT_DTM) VALUES
                           ('{room.RoomNo}',{room.UsrNo},'{room.Title}',to_char(now(),'YYYYMMDDHH24MISS'))";
            Query.insert(sql);
        }

        public void AddRoomUser(Room room)
        {
            string sql = @$"INSERT INTO talk.chatuser (ROOM_NO,USR_NO,TITLE) VALUES
                           ('{room.RoomNo}',{room.UsrNo},'{room.Title}')";
            Query.insert(sql);
        }

        public void LeaveRoom(int roomNo, int usrNo)
        {
            string sql = @$"DELETE FROM talk.room
                             WHERE ROOM_NO = {roomNo}
                               AND USR_NO = {usrNo}"
                         ;
            Query.insert(sql);
        }

        public void UpdateTitle(int? roomNo, int usrNo, string title)
        {
            string sql = @$"UPDATE talk.room
                               SET TITLE = '{title}'
                             WHERE ROOM_NO = {roomNo}
                               AND USR_NO = {usrNo}"
                         ;
            Query.insert(sql);
        }

        public void InsertChat(int roomNo, int usrNo, string type, string msg)
        {
            string sql = @$"INSERT INTO talk.chat
                            (CHAT_NO,ROOM_NO,USR_NO,CHAT_FG,CHAT,RGT_DTM) VALUES
                            ((SELECT COALESCE(MAX(chat_no),0)+1 FROM talk.chat),
                            '{roomNo}',{usrNo},'{type}','{msg}',to_char(now(),'YYYYMMDDHH24MISS'))"
                         ;
            Query.insert(sql);
        }

        public List<Chat> SelectChats(int roomNo)
        {
            string sql = @$"SELECT a.chat_no
                                 , a.chat
                                 , a.usr_no
                                 , a.chat_fg
                              FROM talk.chat a
                             where a.room_no = {roomNo}
                             order by chat_no desc
                             limit 10";
            DataTable? dt = Query.select1(sql);

            var chats = new List<Chat>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                chats.Add(new Chat()
                {
                    ChatNo = (int)(long)dt.Rows[i]["chat_no"],
                    UsrNo = (int)(long)dt.Rows[i]["usr_no"],
                    chat = dt.Rows[i].IsNull("chat") ? "" : (string)dt.Rows[i]["chat"],
                    ChatFg = (string)dt.Rows[i]["chat_fg"],
                });
            };

            return chats;
        }
    }
}
