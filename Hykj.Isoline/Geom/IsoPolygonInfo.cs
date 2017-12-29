using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.GISModule
{
    /// <summary>
    /// 多边形，支持环多边形，由IsoRing组成
    /// outerRings:外围多边形IsoRing一个（暂时未想到必须多个的情况）
    /// interRings:内部镂空多边形IsoRing数组，可由一个或多个组成
    /// 作者：maxiaoling
    /// 日期：2017.12.17
    /// </summary>
    public  class IsoPolygonInfo
    {
        #region 属性

        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        //当前等值线外环值，用于计算等值线渲染颜色
        private double value;
        public double Value
        {
            get { return this.value; }
        }

        //valueType标识值的类型，1标识当前值为等值面的最小值，0标识当前值为等值面的最大值，-1标识等值面未赋值
        private int valueType = -1;
        public int ValueType
        {
            get { return valueType; }
        }

        //private double minValue = -1;

        //public double MinValue
        //{
        //    get { return minValue; }
        //    set { minValue = value; }
        //}
        //private double maxValue = -1;

        //public double MaxValue
        //{
        //    get { return maxValue; }
        //    set { maxValue = value; }
        //}

        private IsoRing outerRing;
        public IsoRing OuterRing
        {
            get { return outerRing; }
            set { outerRing = value; }
        }

        private List<string> interRingsID;

        public List<string> InterRingsID
        {
            get { return interRingsID; }
            set { interRingsID = value; }
        }

        //private List<IsoRing> interRings;
        //public List<IsoRing> InterRings
        //{
        //    get { return interRings; }
        //    set { interRings = value; }
        //}
        private string polygonColor;

        public string PolygonColor
        {
            get { return polygonColor; }
            set { polygonColor = value; }
        }
        #endregion

        #region 构造函数
        public IsoPolygonInfo(string id,IsoRing outerRing)
        {
            this.outerRing = outerRing;
            this.interRingsID = new List<string>();
            //this.interRings = new List<IsoRing>();
            this.id = id;
        }
        #endregion

        #region 公共方法
        /*
         * 添加内环多边形
         */
        //public void AddInterRing(IsoRing isoRing)
        //{
        //    this.interRings.Add(isoRing);
        //}

        public void AddInterRingId(string ringId)
        {
            this.interRingsID.Add(ringId);
        }

        public void SetValue(double value,bool isMax)
        {
            this.value = value;
            if (isMax)
            {
                this.valueType = 0;
            }
            else
            {
                this.valueType = 1;
            }
        }
        #endregion
    }
}
