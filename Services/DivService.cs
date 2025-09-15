using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;
using talkLib.Util;

namespace talk2.Services
{
    public interface IDivService
    {
        public Task<List<Div>> getDivList();
        public Task<List<Div>> InsertDiv(string divNm);
        public Task EditDiv(int divNo, string divNm);
        public Task DeleteDiv(int divNo);
    }

    public class DivService : IDivService
    {
        public async Task<List<Div>> getDivList()
        {
            string responseBody = await HttpUtil.Get($"/div/list");
            return JsonUtil.StringToObject<List<Div>>(responseBody);
        }

        public async Task<List<Div>> InsertDiv(string divNm)
        {
            string responseBody = await HttpUtil.Post($"/div", new { divNm = divNm });
            return JsonUtil.StringToObject<List<Div>>(responseBody);
        }

        public async Task EditDiv(int divNo, string divNm)
        {
            await HttpUtil.Put($"/div", new { divNo=divNo, divNm=divNm });
        }

        public async Task DeleteDiv(int divNo)
        {
            await HttpUtil.Delete($"/div", new { divNo = divNo });
        }
    }
}
