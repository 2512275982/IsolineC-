using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.Isoline.Geom
{
    /// <summary>
    /// 多边形，支持multiPolygon，由IsoRing组成
    /// outerRings:外围多边形IsoRing一个（暂时未想到必须多个的情况）
    /// interRings:内部镂空多边形IsoRing数组，可由一个或多个组成
    /// 作者：maxiaoling
    /// 日期：2017.12.17
    /// </summary>
    public  class IsoPolygonInfo
    {
        private double minValue;
        public double MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        private double maxValue;
        public double MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        private IsoRing outerRing;
        public IsoRing OuterRing
        {
            get { return outerRing; }
            set { outerRing = value; }
        }

        private List<IsoRing> interRings;
        public List<IsoRing> InterRings
        {
            get { return interRings; }
            set { interRings = value; }
        }
        private string polygonColor;

        public string PolygonColor
        {
            get { return polygonColor; }
            set { polygonColor = value; }
        }

        public IsoPolygonInfo(IsoRing outerRing)
        {
            this.outerRing = outerRing;
            this.interRings = new List<IsoRing>();
        }

        public IsoPolygonInfo(IsoRing outerRing, List<IsoRing> interRings)
        {
            this.outerRing = outerRing;
            this.interRings = interRings;
        }

        public void AddInterRing(IsoRing isoRing)
        {
            this.interRings.Add(isoRing);
        }
    }
}
