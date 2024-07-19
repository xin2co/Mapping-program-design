using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form0404
{
    public partial class Form1 : Form
    {
        private Button testButton = new Button();

        public Form1()
        {
            InitializeComponent();
            testButton.Click += new EventHandler(this.Ontest);
            this.Controls.Add(testButton);
            testButton.Text = "我的测试";
            testButton.Location = new Point(40,40);
            testButton.Size = new Size(100,40);
        }

        private void Ontest(object sender, EventArgs e)
        {
            InitializeComponent();
            
        }

        private void Myclick2(object sender, EventArgs e)
        {

        }
    }
}
