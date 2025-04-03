using OTILib.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;

namespace talk2.Repositories
{
    public interface IUserRepository
    {
        void login();
        public List<User> getUserList();
    }

    public class UserRepository : IUserRepository
    {
        public void login()
        {
            Debug.WriteLine("user repository");
        }

        public List<User> getUserList()
        {
            string sql = @"SELECT a.usr_no
                                , a.usr_nm
                                , a.div_no
                                , b.div_nm
                             FROM talk.""USER"" a
                                , talk.div b
                            where a.div_no = b.div_no
                            order by 3,1";
            DataTable? dt = Query.select1(sql);

            var users = new List<User>();
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                users.Add(new User()
                {
                    UsrNo = Convert.ToInt16(dt.Rows[i]["usr_no"]),
                    UsrNm = Convert.ToString(dt.Rows[i]["usr_nm"]),
                    DivNo = Convert.ToInt16(dt.Rows[i]["div_no"]),
                    DivNm = Convert.ToString(dt.Rows[i]["div_nm"]),
                });
            }
            ;
            return users;
        }
    }
}
