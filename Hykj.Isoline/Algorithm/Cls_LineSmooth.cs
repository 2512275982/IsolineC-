using Hykj.GISModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hykj.GISModule
{
    public class LineSmooth
    {
        public static List<PointCoord> BsLine(List<PointCoord> pnts, int clipCount = 15)
        {
            try
            {
                List<PointCoord> listOutputPnts = new List<PointCoord>();

                double x0 = 2.0 * pnts[0].X - pnts[1].X;
                double y0 = 2.0 * pnts[0].Y - pnts[1].Y;
                PointCoord pnt0 = new PointCoord(x0, y0);

                int count = pnts.Count;
                double xn = 2.0 * pnts[count - 1].X - pnts[count - 2].X;
                double yn = 2.0 * pnts[count - 1].Y - pnts[count - 2].Y;
                PointCoord pntn = new PointCoord(xn, yn);

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

                        listOutputPnts.Add(new PointCoord(x, y));
                    }
                }
                return listOutputPnts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
