using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace talkLib.Util
{
    class ProfileUtil
    {
        private static readonly Dictionary<int, BitmapImage> _cache = new Dictionary<int, BitmapImage>();

        public static async Task<BitmapImage> GetProfileImageAsync(int usrNo)
        {
            if (_cache.TryGetValue(usrNo, out var cachedImage))
                return cachedImage;

            try
            {
                var bitmap = new BitmapImage(ImageUtil.getProfile(usrNo));
                _cache[usrNo] = bitmap;
                return bitmap;
            }
            catch
            {
                // 실패시 null이나 기본 이미지 리턴 처리
                return null;
            }
        }

        public static void Clear() => _cache.Clear();
    }
}
