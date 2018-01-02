using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hykj.GISModule;
using Hykj.GISModule.Isobands;
using System.Data;
using Hykj.BaseMethods;

namespace Hykj.EagleClass
{
    enum EagleField
    {
        PM10,
        PM25,
        SO2,
        NO2,
        O3,
        CO
    }
    public class EagleCls
    {
        private DataTable tab;
        /// <summary>
        /// 初始化传入要计算的数据
        /// </summary>
        /// <param name="dt">原始数据表</param>
        public EagleCls(DataTable dt)
        {
            tab = dt;
        }

        private EagleField choseField;
        /// <summary>
        /// 等待计算的字段
        /// </summary>
        private EagleField ChoseField
        {
            get { return choseField; }
            set { choseField = value; }
        }

        /// <summary>
        /// 计算生成pm25的等值线
        /// </summary>
        /// <param name="isoLineStr">待输出的等值线</param>
        /// <returns>返回的色斑图字符串</returns>
        public string calculator_PM25(out string isoLineStr)
        {
            this.ChoseField = EagleField.PM25;

            List<IsoLineInfo> listLines = null;
            List<IsoPolygonInfo> listPolys = calculatorIsoGrids(out listLines);

            isoLineStr = JsonHelper.SerializeObject(listLines);
            return JsonHelper.SerializeObject(listPolys);
        }

        /// <summary>
        /// 计算生成pm10的等值线
        /// </summary>
        /// <param name="isoLineStr">待输出的等值线</param>
        /// <returns>返回的色斑图字符串</returns>
        public string calculator_PM10(out string isoLineStr)
        {
            this.ChoseField = EagleField.PM10;

            List<IsoLineInfo> listLines = null;
            List<IsoPolygonInfo> listPolys = calculatorIsoGrids(out listLines);

            isoLineStr = JsonHelper.SerializeObject(listLines);
            return JsonHelper.SerializeObject(listPolys);
        }
        /// <summary>
        /// 计算生成SO2的等值线
        /// </summary>
        /// <param name="isoLineStr">待输出的等值线</param>
        /// <returns>返回的色斑图字符串</returns>
        public string calculator_SO2(out string isoLineStr)
        {
            this.ChoseField = EagleField.SO2;

            List<IsoLineInfo> listLines = null;
            List<IsoPolygonInfo> listPolys = calculatorIsoGrids(out listLines);

            isoLineStr = JsonHelper.SerializeObject(listLines);
            return JsonHelper.SerializeObject(listPolys);
        }
        /// <summary>
        /// 计算生成CO的等值线
        /// </summary>
        /// <param name="isoLineStr">待输出的等值线</param>
        /// <returns>返回的色斑图字符串</returns>
        public string calculator_CO(out string isoLineStr)
        {
            this.ChoseField = EagleField.CO;

            List<IsoLineInfo> listLines = null;
            List<IsoPolygonInfo> listPolys = calculatorIsoGrids(out listLines);

            isoLineStr = JsonHelper.SerializeObject(listLines);
            return JsonHelper.SerializeObject(listPolys);
        }
        /// <summary>
        /// 计算生成NO2的等值线
        /// </summary>
        /// <param name="isoLineStr">待输出的等值线</param>
        /// <returns>返回的色斑图字符串</returns>
        public string calculator_NO2(out string isoLineStr)
        {
            this.ChoseField = EagleField.NO2;

            List<IsoLineInfo> listLines = null;
            List<IsoPolygonInfo> listPolys = calculatorIsoGrids(out listLines);

                isoLineStr = JsonHelper.SerializeObject(listLines);
            return JsonHelper.SerializeObject(listPolys);
        }
        /// <summary>
        /// 计算生成O3的等值线
        /// </summary>
        /// <param name="isoLineStr">待输出的等值线</param>
        /// <returns>返回的色斑图字符串</returns>
        public string calculator_O3(out string isoLineStr)
        {
            this.ChoseField = EagleField.O3;

            List<IsoLineInfo> listLines = null;
            List<IsoPolygonInfo> listPolys = calculatorIsoGrids(out listLines);

            isoLineStr = JsonHelper.SerializeObject(listLines);
            return JsonHelper.SerializeObject(listPolys);
        }

        /// <summary>
        /// 计算等值线公共方法
        /// </summary>
        /// <param name="listLines">输出的等值线</param>
        /// <returns>输出已上色的色斑图</returns>
        private List<IsoPolygonInfo> calculatorIsoGrids(out List<IsoLineInfo> listLines)
        {
            List<PointInfo> listPntInfo = new List<PointInfo>();
            listPntInfo = this.GetOriginalData(this.tab, this.ChoseField); //提取选择的数据

            GridClass gridClass = new Hykj.GISModule.GridClass(listPntInfo);
            gridClass.GetGrid();

            GridIsoline gridIsoline = new GridIsoline(gridClass);
            double[] lineValue = this.GetSplitArray();
            gridIsoline.WikiIsoLineToBands(lineValue);

            listLines = gridIsoline.Isolines;
            return TintingISObands(gridIsoline.IsoBands);

        }

        /// <summary>
        /// 根据源数据与所选字段，提取出来需要的内容
        /// </summary>
        /// <param name="student4"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private List<PointInfo> GetOriginalData(System.Data.DataTable student4, EagleField fieldName)
        {

            this.ChoseField = fieldName;
            List<PointInfo> listPntInfo = new List<PointInfo>();
            foreach (DataRow dataRow in student4.Rows)
            {
                double x = double.Parse(dataRow["longitude"].ToString());
                double y = double.Parse(dataRow["latitude"].ToString());
                double z = double.Parse(dataRow[fieldName.ToString()].ToString());
                PointInfo pntInfo = new PointInfo(x, y, z);
                listPntInfo.Add(pntInfo);
            }
            return listPntInfo;
        }

        /// <summary>
        /// 获取数据对应的分割数组
        /// </summary>
        /// <returns></returns>
        private double[] GetSplitArray()
        {
            switch (this.ChoseField)
            {
                case EagleField.CO:
                    return new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                case EagleField.NO2:
                    return new double[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270 };
                case EagleField.O3:
                    return new double[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240 };
                case EagleField.PM25:
                    return new double[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105 };
                case EagleField.PM10:
                    return new double[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260 };
                case EagleField.SO2:
                    return new double[] { 50, 100, 150, 200, 250, 300, 350, 400, 450, 500, 550, 600, 650, 700, 750 };
                default:
                    break;
            }
            return new double[2];
        }
        /// <summary>
        /// 获取分割对应的颜色组合
        /// </summary>
        /// <returns></returns>
        private string[] GetSplitCols()
        {
            string[] cols = new string[] { };
            switch (this.ChoseField)
            {
                case EagleField.PM10:
                    cols = new string[] { "#02fe00", "#0dfe00", "#2bfb07", "#3afe00", "#52fb04", "#60fe03", "#71ff03", "#8afe00", "#9efc0a", "#b2fe01", "#c5fd02", "#d7fe02", "#eefd00", "#fdfc07", "#f9d100", "#fe9c00", "#fe6c00", "#fd3b00", "#fb080e", "#e50200", "#d20003", "#bb0200", "#9e0100", "#870006", "#630001", "#2e0100", "#000000" };
                    break;
                case EagleField.PM25:
                    cols = new string[] { "#01fd04", "#1bfb04", "#33fd00", "#49fe04", "#5fff00", "#7afe00", "#95fc01", "#a9fe08", "#c2fd06", "#ddfd00", "#f3fe01", "#fbe300", "#fca700", "#fc6801", "#fb2909", "#f80006", "#d60004", "#ba0001", "#990001", "#770100", "#3a0000", "#000000" };
                    break;
                case EagleField.SO2:
                    cols = new string[] { "#00fe03", "#22fd02", "#47fe00", "#6bff01", "#8ffe00", "#bafd00", "#dbfe00", "#fffc00", "#ffa401", "#ff4703", "#f40100", "#cc0000", "#990001", "#580000", "#200506", "#000000" };
                    break;
                case EagleField.NO2:
                    cols = new string[] { "#01fe00", "#11fd02", "#29fb04", "#43f806", "#4efd03", "#63fb03", "#72fe02", "#89fd06", "#9afd08", "#abfe08", "#bffd07", "#d4fe00", "#e7fe04", "#f9fe03", "#fed900", "#fea50b", "#fe7700", "#fd4902", "#fe1701", "#ec0302", "#d60100", "#c70001", "#ab0000", "#900100", "#740200", "#450000", "#130000", "#000000" };
                    break;
                case EagleField.O3:
                    cols = new string[] { "#00fe00", "#17fd00", "#2ffd01", "#41fc04", "#54fe00", "#6bfd05", "#7cfe00", "#96fd04", "#aaff00", "#bdfe04", "#d4fe01", "#ebfe01", "#fefc0e", "#fecb00", "#ff9500", "#fb6101", "#fd2900", "#f90000", "#dd0101", "#c20100", "#a70102", "#900000", "#690003", "#330101", "#000000" };
                    break;
                case EagleField.CO:
                    cols = new string[] { "#00fe06", "#2bff00", "#55fe00", "#7eff01", "#a7ff01", "#d5fe01", "#fffd02", "#fe9501", "#fe2a00", "#dd0100", "#a90100", "#680100", "#000000" };
                    break;
                default:
                    break;
            }
            return cols;
        }

        /// <summary>
        /// 给色斑图上色
        /// </summary>
        /// <param name="listPolys"></param>
        private List<IsoPolygonInfo> TintingISObands(List<IsoPolygonInfo> listPolys)
        {
            string[] cols = GetSplitCols();
            double[] splitArrs = GetSplitArray();
            foreach (IsoPolygonInfo polygonInfo in listPolys)
            {
                if (polygonInfo.ValueType == -1)
                {
                    continue;
                }
                else
                {
                    int index = Array.IndexOf(splitArrs, polygonInfo.Value);
                    if (index == 0 || index == splitArrs.Length || index == -1)
                    {
                        //未确认下面写法是否正确
                    }
                    else
                        if (polygonInfo.ValueType == 1)//当前值为等值面的最小值
                        {
                            polygonInfo.PolygonColor = cols[index + 1];

                        }
                        else if (polygonInfo.ValueType == 0)//当前值为等值面的最大值
                        {
                            polygonInfo.PolygonColor = cols[index];
                        }
                }

            }
            return listPolys;
        }
    }
}