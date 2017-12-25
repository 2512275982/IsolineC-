using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.GISModule
{
    /// <summary>
    /// 定义点类，并定义一个公共函数，用于判断点位置是否一致，判断精度需要再考虑
    /// 该点类应该是插值点类，不能代表所有的点
    /// 从JavaScript代码移植过来
    /// 作者：maxiaoling
    /// 日期：2017.12.15
    /// </summary>
    public class PointInfo
    {
        #region 字段
        //点位置
        private PointCoord pntCoord;

        public PointCoord PntCoord
        {
            get { return pntCoord; }
            set { pntCoord = value; }
        }
        //Z值，不一定是高程
        private double z;
        public double Z
        {
            get { return z; }
            set { z = value; }
        }
        //是否边界点
        private bool isEdge;
        public bool IsEdge
        {
            get { return isEdge; }
            set { isEdge = value; }
        }
        #endregion

        #region 构造函数，三种重载
        public PointInfo()
        {
        }
        public PointInfo(double x,double y)
        {
            pntCoord = new PointCoord(x, y);
        }

        public PointInfo(double x, double y, double z)
        {
            pntCoord = new PointCoord(x, y);
            this.z = z;
        }
        public PointInfo(double x, double y, double z, bool edgeFlag)
        {
            pntCoord = new PointCoord(x, y);
            this.z = z;
            this.isEdge = edgeFlag;
        }
        #endregion

        #region 公共方法
        /*
         * 判断另一点与当前点位置是否一致，目前认为只有插值点有这样的判断需求
         * 经过测试，插值点是完全一致的，可以用等于0进行判断
         * 判断是否相等的精度，需要再确定小数位数
         */
        public bool Equals(PointInfo pntOther)
        {
            if (Math.Abs(pntOther.PntCoord.X - this.PntCoord.X) < 0.000000000001 && Math.Abs(pntOther.PntCoord.Y - this.PntCoord.Y) < 0.000000000001)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// 定义点结构，为后面的线和面点列表做准备，也实现业务和基础数据的分离
    /// 需要对上面的PointInfo类进行调整，实现业务和基础数据的分离
    /// 作者：maxiaoling
    /// 日期：2017.12.17
    /// </summary>
    public struct PointCoord
    {
        public double X, Y;
        public PointCoord(double xCoord, double yCoord)
        {
            this.X = xCoord;
            this.Y = yCoord;
        }
    }
}
