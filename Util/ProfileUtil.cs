using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using talk2.Util;

namespace talkLib.Util
{
    class ProfileUtil
    {
        private static readonly Dictionary<int, BitmapImage> _cache = new Dictionary<int, BitmapImage>();
        private static BitmapImage _default;

        public static async Task<BitmapImage> GetProfileImageAsync(int usrNo)
        {
            if (_cache.TryGetValue(usrNo, out var cachedImage))
                return cachedImage;

            if (UserUtil.getUser(usrNo).ProfileNo == 0)
            {
                return getDefaultImage();
            }

            try
            {
                var bitmap = new BitmapImage(ImageUtil.getProfile(UserUtil.getUser(usrNo).ProfileNo));
                _cache[usrNo] = bitmap;
                return bitmap;
            }
            catch
            {
                // 실패시 null이나 기본 이미지 리턴 처리
                return getDefaultImage();
            }
        }

        public static void Clear() => _cache.Clear();

        private static BitmapImage getDefaultImage()
        {
            if (_default is null)
                _default = new BitmapImage(new Uri("pack://application:,,,/talk2;component/Images/cat-5183427_640.jpg"));
                // _default = new BitmapImage(new Uri("C:\\Users\\doo\\Pictures\\small.jpg"));

            return _default;
        }
    }
}
