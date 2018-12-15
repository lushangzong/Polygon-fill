using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polygon
{
    public class Edge
    {
        //边的上端点的Y坐标
        public int y_max;

        //在Edge Table中表示边的下端点的x坐标
        public int x;

        //边的斜率的倒数
        public double delta_x;

        //边的下端点的y坐标
        public int y_min;
    }
}
