using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hykj.Isoline.Geom;

namespace Hykj.Isoline.Isobands
{
    public class GridIsoline
    {
        private List<IsoLineInfo> tempIsolines;
        private List<IsoLineInfo> listIsolines;
        private List<PointInfo pntGrid;

        public void WikiIsoline()
        {

        }

        /*
		 * 判断值格网点值与目标值的关系，小于最小值返回0，大于等于返回1
		 */
		private int GetTypeValue(double zValue, double lineValue) {
			int type = -1;
			if(zValue < lineValue) {
				type = 0;
			} else if(zValue >= lineValue) {
				type = 1;
			} 
//			else if(zValue > lineValue) {
//				type = 2;
//			}
			return type;
		}

        /*
		 * 将追踪的短线段添加到等值线列表中
		 * 输入参数lineFromPnt和lineToPnt，分别标识短线段的起始点
		 */
		private void UpdateIsolines(PointInfo lineFromPnt,PointInfo lineToPnt,double value){
			//当两个点都是边界点时，该等值线由这两个点组成
			if(lineFromPnt.IsEdge && lineToPnt.IsEdge){
				IsoLineInfo isoline = new IsoLineInfo(value);  
				isoline.AddPointInfo(lineFromPnt);
				isoline.AddPointInfo(lineToPnt);
				isoline.LineType = true;  //开放型等值线
				isoline.FinishState = true;  
				tempIsolines.Add(isoline);
			}
			else{
				bool matchFlag = false;
				if(tempIsolines.Count > 0){   //当等值线数量为空时
					for(int i = 0;i<tempIsolines.Count - 1;i++){  //遍历所有的等值线
						IsoLineInfo isoline = tempIsolines[i];
						if(isoline.FinishState)  //如果等值线追踪完成
							continue;
						matchFlag = false;
						PointInfo pntStart = isoline.GetLineFrom();
						PointInfo pntEnd = isoline.GetLineEnd();
						if(pntStart.IsEdge){
							if(lineFromPnt.IsEdge){
								matchFlag = pntEnd.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineFromPnt);
									isoline.FinishState = true;
								}
							}
							else if(lineToPnt.IsEdge){
								matchFlag = pntEnd.Equals(lineFromPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineToPnt);
									isoline.FinishState = true;
								}
							}
							else{
								matchFlag = pntEnd.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineFromPnt);
								}
								else{
									matchFlag = pntEnd.Equals(lineFromPnt);
									if(matchFlag){
										isoline.AddPointInfo(lineToPnt);
									}
								}
							}
						}
						else if(pntEnd.IsEdge){   //在等值线的起始点插入值
							if(lineFromPnt.IsEdge){
								matchFlag = pntStart.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineToPnt,0);
									isoline.AddPointInfo(lineFromPnt,0);
									isoline.FinishState = true;
								}
							}
							else if(lineToPnt.IsEdge){
								matchFlag = pntStart.Equals(lineFromPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineFromPnt,0);
									isoline.AddPointInfo(lineToPnt,0);
									isoline.FinishState = true;
								}
							}
							else{
								matchFlag = pntStart.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineFromPnt,0);
								}
								else{
									matchFlag = pntStart.Equals(lineFromPnt);
									if(matchFlag){
										isoline.AddPointInfo(lineToPnt,0);
									}
								}
							}
						}
						else{
							if(lineFromPnt.IsEdge){
								matchFlag = pntStart.Equals(lineToPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineFromPnt,0);
									isoline.LineType = true;
								}
								else{
									matchFlag = pntEnd.Equals(lineToPnt);
									if(matchFlag){
										isoline.AddPointInfo(lineFromPnt);
										isoline.LineType = true;
									}
								}
							}
							else if(lineToPnt.IsEdge){
								matchFlag = pntStart.Equals(lineFromPnt);
								if(matchFlag){
									isoline.AddPointInfo(lineToPnt,0);
									isoline.LineType = true;
								}
								else{
									matchFlag = pntEnd.Equals(lineFromPnt);
									if(matchFlag){
										isoline.AddPointInfo(lineToPnt);
										isoline.LineType = true;
									}
								}
							}
							else{
								matchFlag = true;
								if(pntStart.Equals(lineFromPnt)&&pntEnd.Equals(lineToPnt)){
									isoline.AddPointInfo(lineFromPnt);
									isoline.FinishState = true;
								}
								else if(pntStart.Equals(lineToPnt)&&pntEnd.Equals(lineFromPnt)){
									isoline.AddPointInfo(lineToPnt);
									isoline.FinishState = true;
								}
								else if(pntStart.Equals(lineFromPnt)){
									isoline.AddPointInfo(lineToPnt,0);
								}
								else if(pntStart.Equals(lineToPnt)){
									isoline.AddPointInfo(lineFromPnt,0);
								}
								else if(pntEnd.Equals(lineFromPnt)){
									isoline.AddPointInfo(lineToPnt);
								}
								else if(pntEnd.Equals(lineToPnt)){
									isoline.AddPointInfo(lineFromPnt);
								}
								else{
									matchFlag = false;
								}
							}
						}
						if(matchFlag)  //如果找到匹配的等值线，则跳出循环
							break;
					}
				}
				if(!matchFlag){    //如果没有找到匹配的等值线，则添加一条新的等值线
					var isoline = new IsoLineInfo(value);  
					isoline.AddPointInfo(lineFromPnt);
					isoline.AddPointInfo(lineToPnt);
					
					if(lineFromPnt.IsEdge || lineToPnt.IsEdge){
						isoline.LineType = true;  //开放型等值线
					}
					tempIsolines.Add(isoline);
				}
			}
		}

        /*
		 * 获取等值线的标注信息，包括位置，角度以及值
		 */
		private LabelInfo GetLabelInfo(IsoLineInfo isoline)
        {
			double angle = 0,dis;
			double maxDis = 0;
			List<PointInfo> linePnts = isoline.ListVertrix;
			PointInfo pnt1,pnt2;
            PointInfo pntLabel = new PointInfo();
			for(var i = 0; i < linePnts.Count - 1; i++){
				pnt1 = linePnts[i];
				pnt2 = linePnts[i + 1];
				dis = Math.Sqrt((pnt1.X - pnt2.X)*(pnt1.X - pnt2.X) + (pnt1.Y - pnt2.Y)*(pnt1.Y - pnt2.Y));
				if(dis>maxDis){
					pntLabel.X = (pnt1.X+pnt2.X)/2;
                    pntLabel.Y = (pnt1.Y+pnt2.Y)/2;
					angle = (pnt2.Y - pnt1.Y)/(pnt2.X - pnt1.X);
				}
			}
			return new LabelInfo(pntLabel,angle,isoline.LineValue);
		}

        private List<PointInfo> BsLine(List<PointInfo> pnts, int clipCount = 15)
        {
            try
            {
                List<PointInfo> listOutputPnts = new List<PointInfo>();

                double x0 = 2.0 * pnts[0].X - pnts[1].X;
                double y0 = 2.0 * pnts[0].Y - pnts[1].Y;
                PointInfo pnt0 = new PointInfo(x0, y0);

                int count = pnts.Count;
                double xn = 2.0 * pnts[count - 1].X - pnts[count - 2].X;
                double yn = 2.0 * pnts[count - 1].Y - pnts[count - 2].Y;
                PointInfo pntn = new PointInfo(xn, yn);

                pnts.Insert(0, pnt0);
                pnts.Add(pntn);

                double A0, A1, A2, A3;
                double B0, B1, B2, B3;

                double dt = 1.0 / clipCount;
                for (int i = 0; i < pnts.Count - 3; i++)
                {
                    A0 = (pnts[i].X + 4.0 * pnts[i + 1].X + pnts[i + 2].X) / 6.0;
                    A1 = -(pnts[i].X - pnts[i + 2].X) / 2.0;
                    A2 = (pnts[i].X - 2.0 * pnts[i + 1].X + pnts[i + 2].X) / 2.0;
                    A3 = -(pnts[i].X - 3.0 * pnts[i + 1].X + 3.0 * pnts[i + 2].X - pnts[i + 3].X) / 6.0;
                    B0 = (pnts[i].Y + 4.0 * pnts[i + 1].Y + pnts[i + 2].Y) / 6.0;
                    B1 = -(pnts[i].Y - pnts[i + 2].Y) / 2.0;
                    B2 = (pnts[i].Y - 2.0 * pnts[i + 1].Y + pnts[i + 2].Y) / 2.0;
                    B3 = -(pnts[i].Y - 3.0 * pnts[i + 1].Y + 3.0 * pnts[i + 2].Y - pnts[i + 3].Y) / 6.0;

                    double t1, t2, t3 = 0;
                    for (int j = 0; j < clipCount + 1; j++)
                    {
                        t1 = dt * j;
                        t2 = t1 * t1;
                        t3 = t1 * t2;

                        double x = A0 + A1 * t1 + A2 * t2 + A3 * t3;
                        double y = B0 + B1 * t1 + B2 * t2 + B3 * t3;

                        listOutputPnts.Add(new PointInfo(x, y));
                    }
                }
                return listOutputPnts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*
		 * 合并单值等值线，将一段一段的线合并为一整条等值线
		 */
		private void MergeIsolines()
        {
			for(int i = 0;i < tempIsolines.Count; i++){
				var line = tempIsolines[i];
				if(line.FinishState)
					continue;
				if(MergeLine(i)){
                    tempIsolines.RemoveAt(i);
                    //tempIsolines.splice(i,1);
					i = 0;
				}
			}
		}

        /*
		 * 指定某段线的合并实现
		 */
		private bool MergeLine(int index)
        {
			IsoLineInfo line = tempIsolines[index];
			for(int i = 0;i < tempIsolines.Count; i++){
				if(i == index)
					continue;
				IsoLineInfo lineM = tempIsolines[i];
				if(lineM.FinishState)
					continue;
				PointInfo pntMFrom = lineM.GetLineFrom();
				PointInfo pntMEnd = lineM.GetLineEnd();
				
				PointInfo pntFrom = line.GetLineFrom();
				PointInfo pntEnd = line.GetLineEnd();
				
				if(pntMFrom.Equals(pntFrom) && pntMEnd.Equals(pntEnd)){  //首尾相接
					lineM.ListVertrix = lineM.ListVertrix.concat(line.ListVertrix.reverse());
					lineM.FinishState = true;
					return true;
				}
				else if(pntMFrom.Equals(pntEnd) && pntMEnd.Equals(pntFrom)){  //首尾相接
					lineM.ListVertrix = lineM.ListVertrix.concat(line.ListVertrix);
					lineM.FinishState = true;
					return true;
				}
				else if(pntMFrom.Equals(pntFrom)){
					lineM.ListVertrix = lineM.ListVertrix.reverse().concat(line.ListVertrix);
					if(pntMEnd.IsEdge && pntEnd.IsEdge)
					{
						lineM.FinishState = true;
					}
					lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
				else if(pntMFrom.Equals(pntEnd)){
					lineM.ListVertrix = line.ListVertrix.concat(lineM.ListVertrix);
					if(pntMEnd.IsEdge && pntFrom.IsEdge)
					{
						lineM.FinishState = true;
					}
					lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
				else if(pntMEnd.Equals(pntFrom)){
					lineM.ListVertrix = lineM.ListVertrix.concat(line.ListVertrix);
					if(pntMFrom.IsEdge && pntEnd.IsEdge)
					{
						lineM.FinishState = true;
					}
					lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
				else if(pntMEnd.Equals(pntEnd)){
					lineM.ListVertrix = lineM.ListVertrix.concat(line.ListVertrix.reverse());
					if(pntMFrom.IsEdge && pntFrom.IsEdge){
						lineM.FinishState = true;
					}
					lineM.LineType = (lineM.LineType || line.LineType);
					return true;
				}
			}
			
			return false;
		}

        private void GetIsolines(double lineValue)
        {
            PointInfo[][] pntGrid = new PointInfo[][]{};
			tempIsolines.Clear(); //清空数组
			for(int i = 0; i < pntGrid.Length - 1; i++) {
				for(int j = 0; j < pntGrid[i].Length - 1; j++) {
					PointInfo pntV4 = pntGrid[i][j];
					PointInfo pntV1 = pntGrid[i][j + 1];
					PointInfo pntV2 = pntGrid[i + 1][j + 1];
					PointInfo pntV3 = pntGrid[i + 1][j];

					int type1 = GetTypeValue(pntV1.Z, lineValue);
					int type2 = GetTypeValue(pntV2.Z, lineValue);
					int type3 = GetTypeValue(pntV3.Z, lineValue);
					int type4 = GetTypeValue(pntV4.Z, lineValue);
					string type = type1.ToString() + type2.ToString() + type3.ToString() + type4.ToString();
					
					//j为0时，点4为边界点；i为0时，点1为边界点；i为length-1时，点3为边界点；j为length-1时，点2为边界点
					PointInfo pnt1,pnt2,pnt3,pnt4;
					double x1, y1, x2, y2;
					switch(type){
						case "0000":
						case "1111":
							break;
						case "0001":  //1
						case "1110": //14
							x1 = pntV4.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.X - pntV4.X);
							y1 = pntV4.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.Y - pntV4.Y);
							if(i == 0)  
							{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,true);
								
							}else{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
							}
							if(j == 0){
								pnt4 = new PointInfo(x1,pntV4.Y,lineValue,true);
							}
							else{
								pnt4 = new PointInfo(x1,pntV4.Y,lineValue,false);
							}
							UpdateIsolines(pnt1,pnt4,lineValue);
							break;
						case "0010"://2
						case "1101"://13
							x1 = pntV4.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.X - pntV4.X);
							y1 = pntV3.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.Y - pntV3.Y);
							if(i == pntGrid.Length - 2)  
							{
								pnt3 = new PointInfo(pntV3.X,y1,lineValue,true);
								
							}else{
								pnt3 = new PointInfo(pntV3.X,y1,lineValue,false);
							}
							if(j == 0){
								pnt4 = new PointInfo(x1,pntV4.Y,lineValue,true);
							}
							else{
								pnt4 = new PointInfo(x1,pntV4.Y,lineValue,false);
							}
							UpdateIsolines(pnt3,pnt4,lineValue);
							break;
						case "0011":  //3
						case "1100":  //12
							y1 = pntV4.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.Y - pntV4.Y);
							y2 = pntV3.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.Y - pntV3.Y);
							if(i == 0)  
							{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,true);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
								
							}
							else if(i == pntGrid.Length - 2)  
							{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,true);
							}
							else{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
							}
							UpdateIsolines(pnt3,pnt1,lineValue);
							break;
						case "0100":   //4
						case "1011":   //11
							x1 = pntV1.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.X - pntV1.X);
							y2 = pntV3.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.Y - pntV3.Y);
							if(j == pntGrid[i].Length - 2){
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,true);
							}
							else{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
							}
							if(i == pntGrid.Length - 2)  
							{
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,true);
							}
							else{
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
							}
							UpdateIsolines(pnt3,pnt2,lineValue);
							break;
						case "0101": //5
							y1 = pntV4.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.Y - pntV4.Y);
							x1 = pntV1.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.X - pntV1.X);
							y2 = pntV3.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.Y - pntV3.Y);
							x2 = pntV4.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.X - pntV4.X);
							if(j == 0){
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,true);
							}
							else if(j == pntGrid[i].Length - 2)
							{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,true);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,false);
							}
							else{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,false);
							}
							if(i == 0){
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,true);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
							}
							else if(i == pntGrid.Length - 2)  
							{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,true);
							}
							else{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
							}
							UpdateIsolines(pnt1,pnt2,lineValue);
							UpdateIsolines(pnt3,pnt4,lineValue);
							break;
						case "0110":  //6
						case "1001":  //9
							x1 = pntV1.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.X - pntV1.X);
							x2 = pntV4.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.X - pntV4.X);
							if(j == 0){
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,true);
							}
							else if(j == pntGrid[i].Length - 2)
							{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,true);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,false);
							}
							else{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,false);
							}
							UpdateIsolines(pnt4,pnt2,lineValue);
							break;
						case "0111":  //7
						case "1000": //8
							y1 = pntV4.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.Y - pntV4.Y);
							x1 = pntV1.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.X - pntV1.X);
							if(j == pntGrid[i].Length - 2){
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,true);
							}
							else{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
							}
							if(i==0){
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,true);
							}
							else{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
							}
							UpdateIsolines(pnt1,pnt2,lineValue);
							break;
						case "1010":  //10
							y1 = pntV4.Y + (lineValue - pntV4.Z) / (pntV1.Z - pntV4.Z) * (pntV1.Y - pntV4.Y);
							x1 = pntV1.X + (lineValue - pntV1.Z) / (pntV2.Z - pntV1.Z) * (pntV2.X - pntV1.X);
							y2 = pntV3.Y + (lineValue - pntV3.Z) / (pntV2.Z - pntV3.Z) * (pntV2.Y - pntV3.Y);
							x2 = pntV4.X + (lineValue - pntV4.Z) / (pntV3.Z - pntV4.Z) * (pntV3.X - pntV4.X);
							if(j==0){
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,true);
							}
							else if(j== pntGrid[i].Length - 2)
							{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,true);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,false);
							}
							else{
								pnt2 = new PointInfo(x1,pntV1.Y,lineValue,false);
								pnt4 = new PointInfo(x2,pntV4.Y,lineValue,false);
							}
							if(i==0){
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,true);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
							}
							else if(i == pntGrid.Length - 2)  
							{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,true);
							}
							else{
								pnt1 = new PointInfo(pntV1.X,y1,lineValue,false);
								pnt3 = new PointInfo(pntV3.X,y2,lineValue,false);
							}
							UpdateIsolines(pnt3,pnt2,lineValue);
							UpdateIsolines(pnt1,pnt4,lineValue);
							break;
					}
				}
			}
		}
    }
}
