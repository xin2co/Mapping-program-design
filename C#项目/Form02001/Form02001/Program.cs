using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Form02001
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            MyForm form = new MyForm();
            Application.Run(form);
        }
    }
}
