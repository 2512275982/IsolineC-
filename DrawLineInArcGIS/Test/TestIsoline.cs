﻿using Hykj.BaseMethods;
using Hykj.Isoline.Geom;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Hykj.Isoline.Isobands;

namespace DrawLineInArcGIS
{
    /// <summary>
    /// 
    /// </summary>
    public class TestIsoline
    {
        public static GridIsoline ReadJsonFile()
        {
            string fieldName = "pm25";
            List<PointInfo> listPntInfo = new List<PointInfo>();
            string jsonValue = File.ReadAllText(@"E:\工作内容\Q气象局项目\等值线程序\DrawLineInArcGIS\DrawLineInArcGIS\bin\Debug\Data\test.json");
            if (!string.IsNullOrEmpty(jsonValue))
            {
                DataTable student4 = JsonHelper.DeserializeJsonToObject<DataTable>(jsonValue);
                foreach (DataRow dataRow in student4.Rows)
                {
                    double x = double.Parse(dataRow["longitude"].ToString());
                    double y = double.Parse(dataRow["latitude"].ToString());
                    double z = double.Parse(dataRow[fieldName].ToString());
                    PointInfo pntInfo = new PointInfo(x, y, z);
                    listPntInfo.Add(pntInfo);
                }
            }

            GridClass gridClass = new Hykj.Isoline.Geom.GridClass(listPntInfo);
            PointInfo[,] gridInfo = gridClass.GetGrid();
            GridIsoline gridIsoline = new GridIsoline(gridInfo);
            double[] lineValue = new double[]{5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105};
            List<IsoLineInfo> listLines = gridIsoline.WikiIsoline(lineValue);
            List<IsoPolygonInfo> listPolys = gridIsoline.WikiIsolineBand(listLines, gridClass.SuperGridCoord);
            return gridIsoline;
            
        }

        public static void TestPolys(List<IsoLineInfo> listLines)
        {
        }
    }
}
