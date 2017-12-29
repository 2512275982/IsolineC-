using Hykj.BaseMethods;
using Hykj.GISModule;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Hykj.GISModule.Isobands;
using Hykj.EagleClass;

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

            GridClass gridClass = new Hykj.GISModule.GridClass(listPntInfo);
            gridClass.GetGrid();
            GridIsoline gridIsoline = new GridIsoline(gridClass);
            double[] lineValue = new double[]{5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105};
            gridIsoline.WikiIsoLineToBands(lineValue);

            return gridIsoline;
            
        }

        public static void ReadJsonFile1()
        {
            string jsonValue = File.ReadAllText(@"E:\工作内容\Q气象局项目\等值线程序\DrawLineInArcGIS\DrawLineInArcGIS\bin\Debug\Data\test.json");
            if (!string.IsNullOrEmpty(jsonValue))
            {
                DataTable student4 = JsonHelper.DeserializeJsonToObject<DataTable>(jsonValue);

                EagleCls eagle = new EagleCls(student4);
                string isoLinesStr = null;
                string isoBandsStr = eagle.calculator_PM25(out isoLinesStr);

                isoBandsStr = isoBandsStr.Replace("\"X\":", "");
                isoBandsStr = isoBandsStr.Replace("\"Y\":", "");
                //using (StreamWriter sw = new StreamWriter(@"C:\Users\admin\Desktop\test\isoLines.txt", false, Encoding.Default))
                //{
                //    sw.Write(isoLinesStr);
                //} 
                using (StreamWriter sw = new StreamWriter(@"C:\Users\admin\Desktop\test\isoBands.txt", false, Encoding.Default))
                {
                    sw.Write(isoBandsStr);
                }
            }
        }
    }
}
