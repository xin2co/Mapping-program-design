using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form0405
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnbuttonClicked(object sender, EventArgs e)
        {
            string timeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.timeField.Text = timeStr;
        }
    }
}
