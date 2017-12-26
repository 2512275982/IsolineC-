using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hykj.GISModule.Isobands;
using Hykj.BaseMethods;
using Hykj.GISModule;


namespace TestIsoband
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = Application.StartupPath + @"\Data\test.json";
            GridIsoline gridIsoline = TestIsoline.ReadJsonFile(filePath);
            List<IsoPolygonInfo> listPolys = gridIsoline.IsoBands;
            int count = 0;
            for (int i = 0; i < listPolys.Count; i++)
            {
                if (listPolys[i].MinValue == listPolys[i].MaxValue)
                    count++;
            }
            MessageBox.Show(count.ToString());
        }
    }
}
