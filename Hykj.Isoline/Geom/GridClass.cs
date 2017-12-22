using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.Isoline.Geom
{
    /// <summary>
    /// 网格生成类，用于网格插值
    /// 插值方法包括：1、反距离权重法
    /// 作者：maxiaoling
    /// 日期：2017.12.17
    /// </summary>
    public class GridClass
    {
        private List<PointInfo> listOriginPnts;
        private int gridStep = 150;
        private int extendGridNum = 2;
        private PointInfo[,] pntGrid;  //对应

        private GridCoord superGridCoord;

        public GridCoord SuperGridCoord
        {
            get { return superGridCoord; }
            set { superGridCoord = value; }
        }

        //private double xmin;
        //public double Xmin
        //{
        //    get { return xmin; }
        //    set { xmin = value; }
        //}
        //private double ymin;
        //public double Ymin
        //{
        //    get { return ymin; }
        //    set { ymin = value; }
        //}
        //private double xmax;
        //public double Xmax
        //{
        //    get { return xmax; }
        //    set { xmax = value; }
        //}
        //private double ymax;
        //public double Ymax
        //{
        //    get { return ymax; }
        //    set { ymax = value; }
        //}
        public GridClass(List<PointInfo> listPntInfo)
        {
            this.listOriginPnts = listPntInfo;
        }

        public GridClass(List<PointInfo> listPntInfo, GridCoord gridCoord)
        {
            this.listOriginPnts = listPntInfo;
        }

        private void GetSuperGrid()
        {
            double ymax = -1,
                   ymin = -1,
                   xmax = -1,
                   xmin = -1;
            if (listOriginPnts.Count > 0)
            {
                ymin = ymax = listOriginPnts[0].PntCoord.Y;
                xmin = xmax = listOriginPnts[0].PntCoord.X;
            }
            foreach (PointInfo tempPnt in listOriginPnts)
            {
                if (tempPnt.PntCoord.Y < ymin)
                {
                    ymin = tempPnt.PntCoord.Y;
                }
                else if (tempPnt.PntCoord.Y > ymax)
                {
                    ymax = tempPnt.PntCoord.Y;
                }

                if (tempPnt.PntCoord.X < xmin)
                {
                    xmin = tempPnt.PntCoord.X;
                }
                else if (tempPnt.PntCoord.X > xmax)
                {
                    xmax = tempPnt.PntCoord.X;
                }
            }

            double dx = xmax - xmin;
            double dy = ymax - ymin;

            double step = 0;
            if (dx > dy)
            {
                step = 1.0 * dx / (gridStep - 1);
            }
            else
            {
                step = 1.0 * dy / (gridStep - 1);
            }

            xmin = xmin - extendGridNum * step;
            ymin = ymin - extendGridNum * step;

            dx = dx + extendGridNum * step * 2;
            dy = dy + extendGridNum * step * 2;

            xmax = xmin + dx;
            ymax = ymin + dy - (dy % step);
        }

        public PointInfo[,] GetGrid()
        {
            GetSuperGrid();
            
            double dx = xmax - xmin;
            double dy = ymax - ymin;

            double step = 0;
            if (dx > dy)
            {
                step = 1.0 * dx / (gridStep - 1);
            }
            else
            {
                step = 1.0 * dy / (gridStep - 1);
            }

            xmin = xmin - extendGridNum * step;
            ymin = ymin - extendGridNum * step;

            dx = dx + extendGridNum * step * 2;
            dy = dy + extendGridNum * step * 2;

            xmax = xmin + dx;
            ymax = ymin + dy - (dy % step);

            int iMaxValue = (int)(dx / step + 1);
            int jMaxValue = (int)(dy / step + 1);

            pntGrid = new PointInfo[iMaxValue,jMaxValue];

            for (int i = 0; i < iMaxValue; i++)
            {
                double x = xmin + i * step;
                for (int j = 0; j < jMaxValue; j++)
                {
                    double y = ymin + j * step;
                    double value = GetGridPntValue(x, y);
                    PointInfo pnt = new PointInfo(x, y, value);
                    pntGrid[i,j] = pnt;
                }
            }
            return pntGrid;
        }

        /*
         * 插值取网格值，返回网格值
         * 反距离权重法
         */
        private double GetGridPntValue(double x, double y) {
			double valueSum = 0;
			double disSum = 0;
            PointInfo item = null;
            for(int i = 0;i<listOriginPnts.Count;i++){
                item = listOriginPnts[i];
                double dis2 = Math.Pow((item.PntCoord.X - x), 2) + Math.Pow((item.PntCoord.Y - y), 2);
				disSum += 1 / dis2;
				valueSum += 1 / dis2 * item.Z;
            }
			var gridValue = valueSum / disSum;
			return gridValue;
		}
    }

    public struct GridCoord{
        public double xMin;
        public double yMin;
        public double xMax;
        public double yMax;

        public GridCoord(double xmin,double xmax,double ymin,double ymax){
            xMin = xmin;
            xMax = xmax;
            yMin = ymin;
            yMax = ymax;
        }
    }
}
