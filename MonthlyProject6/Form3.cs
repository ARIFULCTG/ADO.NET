using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonthlyProject6
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            CrystalReport2 cr2 = new CrystalReport2();
            cr2.SetParameterValue("ID",Form1.GetComBo);
            cr2.SetParameterValue("DonorID", Form1.GetComBo, cr2.Subreports[0].Name.ToString());
            crystalReportViewer1.ReportSource = cr2;
            crystalReportViewer1.Refresh();

        }
    }
}
