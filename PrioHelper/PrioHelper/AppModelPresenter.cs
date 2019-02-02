using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrioHelper
{
    class AppModelPresenter
    {
        public  byte[] GetRgb(byte[] colors, byte color)
        {
            byte[] res = new byte[] { 100, 100, 100};
            res[0] = (byte)((float)color / colors.Max() * 255);
            return res;
        }
        public double GetTop(byte[] heights, byte height, double maxHeight)
        {
            return (1-((double)height/heights.Max()))*maxHeight;
        }
        public double GetLeft(byte[] widths, byte width, double maxWidth)
        {

            double res;
            int sum = 0;
            Array.ForEach(widths, x => sum += x);

            int partialSum = 0;
            for (int i = 0; i < widths.Length; i++)
                if (widths[i] > width)
                    partialSum += widths[i];

            res = ((double)partialSum / sum) * maxWidth;

            return res;
        }
    }
}
