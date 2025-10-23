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
        private static int cnt = 0;
        private static bool reset = false;

        public static void SavePosition(int roomNo, double left, double top)
        {
            _positions[roomNo] = new Point(left, top);
            cnt++;
            // 여기서 파일 저장 등도 가능
        }

        public static Point? GetSavedPosition(int roomNo)
        {
            if (roomNo == -1)
            {
                Window mw = Application.Current.MainWindow;
                return new Point(mw.Left, mw.Top);
            }
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
            // var savedPosCnt = GetSavedPositionCount() - 1;

            var left = (int)(savedPos != null ? savedPos.Value.X : (mainPos.Value.X + 300 + 30 * cnt));
            var top = (int)(savedPos != null ? savedPos.Value.Y : (mainPos.Value.Y + 30 * cnt));

            var screen = SystemParameters.WorkArea; // 작업 영역(작업표시줄 제외)
            double rightEdge = left + 300;
            double bottomEdge = top + 450;
            if (reset || (rightEdge > screen.Right || bottomEdge > screen.Bottom))
            {
                if (!reset) cnt = 1;
                reset = true;
                left = (int)screen.Left + 30 * cnt;
                top = (int)screen.Top + 30 * cnt;
            }

            if (savedPos == null) SavePosition(roomNo, left, top);

            return (left,top);
        }

        public static void SetCnt0()
        {
            cnt = 0;
        }

        public static void SetResetFalse()
        {
            reset = false;
        }
    }
}
