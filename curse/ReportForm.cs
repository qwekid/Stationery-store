using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace curse
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {

        }
    }
}
