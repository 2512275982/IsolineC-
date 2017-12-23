using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.Isoline.Geom
{
    /// <summary>
    /// 定义等值线对象类
    /// 作者：maxiaoling
    /// 日期：2017.12.16
    /// </summary>
    public class IsoLineInfo
    {
        //等值线节点集合
        private List<PointCoord> listVertrix;
        public List<PointCoord> ListVertrix
        {
            get { return listVertrix; }
            set { listVertrix = value; }
        }
        //标识等值线类型，开放或者闭合,true表示开放型，false表示闭合型
        private bool lineType = false;
        public bool LineType
        {
            get { return lineType; }
            //set { lineType = value; }
        }
        //标识等值线状态，是否完成追踪
        private bool finishState;
        public bool FinishState
        {
            get { return finishState; }
            set { finishState = value; }
        }
        //等值线值
        private double lineValue;
        public double LineValue
        {
            get { return lineValue; }
            set { lineValue = value; }
        }

        //十六进制颜色值
        private string lineColor;

        public string LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        private PointInfo fromPoint;

        //等值线起点
        public PointInfo FromPoint
        {
            get { return fromPoint; }
            set { fromPoint = value; }
        }
        //等值线终点
        private PointInfo toPoint;

        public PointInfo ToPoint
        {
            get { return toPoint; }
            set { toPoint = value; }
        }

        //等值线标注信息
        private LabelInfo label;
        public LabelInfo Label
        {
            get { return label; }
            set { label = value; }
        }

        /*
         * 构造函数
         * value：初始参数，等值线的值
         */
        public IsoLineInfo(double value)
        {
            this.listVertrix = new List<PointCoord>();
            this.lineValue = value;
        }

        public void AddEndPoint(PointInfo pntInfo)
        {
            this.listVertrix.Add(pntInfo.PntCoord);
            //if (this.fromPoint == null)
            //{
            //    this.fromPoint = pntInfo;
            //}
            //else
            //{
            this.toPoint = pntInfo;
            //}
            if (pntInfo.IsEdge)
            {
                this.lineType = true;
            }
        }

        /*
         * 给当前等值线对象添加点
         */
        public void AddStartPoint(PointInfo pntInfo)
        {
            this.listVertrix.Insert(0, pntInfo.PntCoord);
            this.fromPoint = pntInfo;
            if (pntInfo.IsEdge)
            {
                this.lineType = true;
            }
        }

        /// <summary>
        /// 设置线的起点，如果起点是边界点，则等值线为开等值线
        /// </summary>
        /// <param name="pnt"></param>
        public void SetFromPoint(PointInfo pnt)
        {
            this.fromPoint = pnt;
            if(pnt.IsEdge)
            {
                this.lineType = true;
            }
        }

        /// <summary>
        /// 设置线的终点，如果终点是边界点，则等值线为开等值线
        /// </summary>
        /// <param name="pnt"></param>
        public void SetToPoint(PointInfo pnt)
        {
            this.toPoint = pnt;
            if (pnt.IsEdge)
            {
                this.lineType = true;
            }
        }
        /*
         * 获取等值线的起点
         */
        //public PointInfo GetLineFrom()
        //{
        //    return listVertrix[0];
        //}
        /*
        * 获取等值线的终点
        */
        //public PointInfo GetLineEnd()
        //{
        //    return listVertrix[this.listVertrix.Count - 1];
        //}
    }

    /// <summary>
    /// 等值线标注信息
    /// 作者：maxiaoling
    /// 日期：2017.12.16
    /// </summary>
    public class LabelInfo
    {
        //label位置
        private PointCoord labelPnt;
        public PointCoord LabelPnt
        {
            get { return labelPnt; }
            set { labelPnt = value; }
        }
        //label的偏移角度
        private double labelAngle;
        public double LabelAngle
        {
            get { return labelAngle; }
            set { labelAngle = value; }
        }
        //label的值
        private double value;
        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        /*
         * 构造函数
         */
        public LabelInfo(PointCoord labelPnt,double labelAngle,double value)
        {
            this.labelPnt = labelPnt;
            this.labelAngle = labelAngle;
            this.value = value;
        }
    }
}
