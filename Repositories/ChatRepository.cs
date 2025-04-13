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
        public List<Chat> SelectChats(int roomNo, int usrNo);
        public List<User> SelectRoomUserList(int roomNo);
        public int CountRoomWithMe(int myUsrNo, int usrNo);
        public int CountHeinRoom(int roomNo, int usrNo);
    }

    internal class ChatRepository : IChatRepository
    {
        public List<Room>? GetRoomList(int usrNo)
        {
            string sql = @$"SELECT a.room_no
                                 , b.title
                                 , c.chat
                                 , coalesce(c.rgt_dtm, a.rgt_dtm) as rgt_dtm
                              FROM talk.room a
                             inner join talk.roomuser b on (a.room_no = b.room_no)
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
                               and b.del_yn = 'N'
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
            string sql = @$"INSERT INTO talk.roomuser (ROOM_NO,USR_NO,TITLE,CHAT_NO,DEL_YN) VALUES
                           ({room.RoomNo},{room.UsrNo},(SELECT TITLE FROM talk.room where ROOM_NO = {room.RoomNo}),
                            (SELECT coalesce(MAX(CHAT_NO),0) FROM talk.chat WHERE ROOM_NO = {room.RoomNo}),'N')";
            Query.insert(sql);
        }

        public void LeaveRoom(int roomNo, int usrNo)
        {
            string sql = @$"UPDATE talk.roomuser
                               SET DEL_YN = 'Y'
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

        public List<Chat> SelectChats(int roomNo, int usrNo)
        {
            string sql = @$"SELECT a.chat_no
                                 , a.chat
                                 , a.usr_no
                                 , a.chat_fg
                              FROM talk.chat a
                             where a.room_no = {roomNo}
                               and a.chat_no > (select chat_no
                                                  from talk.roomuser
                                                 where room_no = {roomNo}
                                                   and usr_no = {usrNo}
                                                   and del_yn = 'N')
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

        public List<User> SelectRoomUserList(int roomNo)
        {
            string sql = @$"SELECT b.usr_no
                                 , b.usr_nm
                              FROM talk.roomuser a
                                 , talk.""user"" b
                             where a.room_no = {roomNo}
                               and a.usr_no = b.usr_no
                               and a.del_yn = 'N'
                             order by usr_nm";
            DataTable? dt = Query.select1(sql);

            var userList = new List<User>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                userList.Add(new User()
                {
                    UsrNo = (int)(long)dt.Rows[i]["usr_no"],
                    UsrNm = (string)dt.Rows[i]["usr_nm"],
                });
            }
            ;

            return userList;
        }

        public int CountRoomWithMe(int myUsrNo, int usrNo)
        {
            string sql = @$"SELECT count(*) as cnt
                              FROM (SELECT room_no
                                         , count(*) as cnt
                                      FROM talk.roomuser
                                     WHERE DEL_YN = 'N'
                                     GROUP BY room_no
                                    HAVING count(*) = 2) A
                                 , (SELECT room_no
                                         , count(*) as cnt
                                      FROM talk.roomuser
                                     WHERE usr_no in ({myUsrNo},{usrNo})
                                       AND DEL_YN = 'N'
                                     GROUP BY room_no) B
                             where A.room_no = B.room_no
                               and A.cnt = B.cnt";
            DataTable? dt = Query.select1(sql);

            int result = -1;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                result = (int)(long)dt.Rows[i]["cnt"];
            }
            return result;
        }

        public int CountHeinRoom(int roomNo, int usrNo)
        {
            string sql = @$"SELECT count(*) as cnt
                              FROM talk.roomuser
                             where room_no = {roomNo}
                               and usr_no = {usrNo}
                               and del_yn = 'N'";
            DataTable? dt = Query.select1(sql);

            int result = -1;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                result = (int)(long)dt.Rows[i]["cnt"];
            }
            return result;
        }
    }
}
