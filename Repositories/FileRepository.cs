using OTILib.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;

namespace talk2.Repositories
{
    public interface IFileRepository
    {
        int GetNewFileNo();
        void saveFile(File file);
    }

    public class FileRepository : IFileRepository
    {
        public int GetNewFileNo()
        {
            string sql = @$"SELECT COALESCE(MAX(file_no),0)+1 as file_no
                              FROM talk.chatfile";
            DataTable? dt = Query.select1(sql);

            var roomNo = 0;
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                roomNo = (int)(long)dt.Rows[0]["file_no"];
            }
            ;

            return roomNo;
        }

        public void saveFile(File file)
        {
            string sql = @$"INSERT INTO talk.chatfile
                           (FILE_NO,FILE_PATH,FILE_NAME,FILE_EXT,ORIGIN_NAME) VALUES
                           ({file.FileNo},'{file.FilePath}','{file.FileName}','{file.FileExt}','{file.OriginName}')";
            Query.insert(sql);
        }
    }
}
