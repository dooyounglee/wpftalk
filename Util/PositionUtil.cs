using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using talk2.Views;

namespace talk2.Util
{
    public class PositionUtil
    {
        private static Dictionary<int, Point> _positions = new();

        public static void SavePosition(int roomNo, double left, double top)
        {
            _positions[roomNo] = new Point(left, top);
            // 여기서 파일 저장 등도 가능
        }

        public static Point? GetSavedPosition(int roomNo)
        {
            OtiLogger.log1(SystemParameters.VirtualScreenWidth);
            OtiLogger.log1(SystemParameters.VirtualScreenHeight);
            if (_positions.TryGetValue(roomNo, out var point))
                return point;

            return null;
        }

        public static int GetSavedPositionCount()
        {
            return _positions.Count;
        }

        public static (int,int) GetRoomPosition(int roomNo)
        {
            var savedPos = GetSavedPosition(roomNo);
            var mainPos = GetSavedPosition(-1);
            var savedPosCnt = GetSavedPositionCount() - 1;

            var left = (int)(savedPos != null ? savedPos.Value.X : (mainPos.Value.X + 300 + 30 * savedPosCnt));
            var top = (int)(savedPos != null ? savedPos.Value.Y : (mainPos.Value.Y + 30 * savedPosCnt));
            if (savedPos == null) SavePosition(roomNo, left, top);
            return (left,top);
        }
    }
}
