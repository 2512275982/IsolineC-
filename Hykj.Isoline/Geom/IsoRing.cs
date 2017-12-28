using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.GISModule
{
    /// <summary>
    /// 定义简单多边形，组成multiPolygon的单元
    /// vertries：点（PointCoord）数组
    /// 参考了openlayers和leaflet的多边形定义，点数组不闭合，也就是起点和终点不是同一个点
    /// 作者：maxiaoling
    /// 日期：2017.12.17
    /// </summary>
    public class IsoRing
    {
        private List<PointCoord> vertries;

        public List<PointCoord> Vertries
        {
            get { return vertries; }
            set { vertries = value; }
        }
        public IsoRing(List<PointCoord> vertries)
        {
            this.vertries = new List<PointCoord>();
            this.vertries.AddRange(vertries);
        }

        public void PushPoint(PointCoord pnt)
        {
            this.vertries.Add(pnt);
        }
        //在多边形的开头加上一个点
        public void UnshiftPoint(PointCoord pnt)
        {
            this.vertries.Insert(0, pnt);
        }
        public bool JudgePntInRing(PointCoord pnt){
            double x = pnt.X;
            double y = pnt.Y;
            return CalPntInRing(x, y);
        }
        ////无必须存在的必要，但为了适应现有代码编写，后期需统一处理
        //public bool JudgePntInRing(PointInfo pntInfo)
        //{
        //    double x = pntInfo.PntCoord.X;
        //    double y = pntInfo.PntCoord.Y;
        //    return CalPntInRing(x, y);
        //}
        /// <summary>
        /// 内部方法，判断坐标值是否位于多边形呢
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CalPntInRing(double x,double y)
        {
            int count = this.vertries.Count;
            if (count < 3)
            {
                return false;
            }

            double x1, y1, x2, y2, dx;
            int pSum = 0;
            for (int i = 0; i <= count - 1; i++)
            {
                x1 = this.vertries[i].X;
                y1 = this.vertries[i].Y;
                if (i == count - 1)
                {
                    x2 = this.vertries[0].X;
                    y2 = this.vertries[0].Y;
                }
                else
                {
                    x2 = this.vertries[i + 1].X;
                    y2 = this.vertries[i + 1].Y;
                }
                if (((y >= y1) && (y < y2)) || ((y >= y2) && (y < y1)))
                {
                    if (Math.Abs(y1 - y2) > 0)
                    {
                        dx = x1 - ((x1 - x2) * (y1 - y)) / (y1 - y2);
                        if (dx < x)
                        {
                            pSum++;
                        }
                    }
                }
            }
            if ((pSum % 2) != 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 等值线生成的简单等值面类，存储等值线的属性信息
    /// 和IsoRing一起，实现了基础结构和业务结构的分离
    /// 作者：maxiaoling
    /// 日期：2017.12.17
    /// </summary>
    public class IsoRingInfo{
        private IsoRing isoRing;
        public IsoRing IsoRing
        {
            get { return isoRing; }
            set { isoRing = value; }
        }
        private string id;  //唯一ID值
        public string ID
        {
            get { return id; }
            set { id = value; }
        }
        private double value;

        public double Value  //等值线值
        {
            get { return this.value; }
            set { this.value = value; }
        }
        private bool valueFlag = false;

        public bool ValueFlag
        {
            get { return valueFlag; }
            //set { valueFlag = value; }
        }


        private double parentValue;

        public double ParentValue  //父等值线值，可能为空
        {
            get { return parentValue; }
            //set { parentValue = value; }
        }

        private bool parentFlag = false;  //是否存在父等值线，存在为true，否则为false

        public bool ParentFlag
        {
            get { return parentFlag; }
        }

        public IsoRingInfo(string id, IsoRing isoRing, double value)
        {
            this.isoRing = isoRing;
            this.id = id;
            this.value = value;
            this.valueFlag = true;
        }

        public IsoRingInfo(string id, IsoRing isoRing)
        {
            this.isoRing = isoRing;
            this.id = id;
            this.valueFlag = false;
        }

        //设置父等值线值
        public void SetParentValue(double parentValue){
            this.parentFlag = true;
            this.parentValue = parentValue;
        }
    }
}
