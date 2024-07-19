using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace 大地正算
{
    public partial class Form1 : Form
    {
        string[] all_lines;
        double a;
        double fdao;
        List<Elements> elements = new List<Elements>();
        string reports;

        public Form1()
        {
            InitializeComponent();
        }

        private void 打开文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                all_lines = File.ReadAllLines(op.FileName, Encoding.Default);
            }
            else
            {
                return;
            }
            try
            {
                string firstLine = all_lines[0];
                string[] firstLineParts = firstLine.Split(',');
                a = int.Parse(firstLineParts[0]);
                fdao = double.Parse(firstLineParts[1]);
                for(int i = 2; i < all_lines.Length; i++)
                {
                    string line = all_lines[i];
                    string[] parts = line.Split(',');
                    if (parts.Length == 6)
                    {
                        Elements element = new Elements()
                        {
                            Qidian = parts[0],
                            B1 = dms2Rad(parts[1]),
                            L1 = dms2Rad(parts[2]),
                            Zhongdian = parts[3],
                            A1 = dms2Rad(parts[4]),
                            S = double.Parse(parts[5]),
                        };
                        elements.Add(element);
                        dataGridView1.Rows.Add(parts[0], parts[1], parts[2], parts[3], parts[4], parts[5]);

                    }
                }
                toolStripLabel2.Text = "椭球长半轴 a" + a + ' ';
                toolStripLabel2.Text += "扁率倒数 1/f" + fdao;
                reports += "1," + "椭球长半轴 a," + a.ToString("F3") + '\n';
                reports += "2," + "扁率倒数 1/f," + fdao.ToString("F3") + '\n';
                reports += "3," + "扁率 f," + (1/fdao).ToString("F6") + '\n';
                toolStripLabel1.Text = "导入成功";
            }
            catch
            {
                MessageBox.Show("文件格式错误");
                toolStripLabel1.Text = "导入失败";
                return;
            }
        }

        double dms2Rad(string angle)
        {
            string[] parts = angle.Split('.');

            double deg = double.Parse(parts[0]);
            double min = double.Parse(parts[1].Substring(0, 2));
            double sec = double.Parse(parts[1].Substring(2))/10;
            double Rad = (deg + min / 60 + sec / 3600) * (Math.PI / 180);
            return Rad;
        }

        public string rad2Dms(double rad)
        {
            double degrees = rad * (180.0 / Math.PI);
            int deg = (int)degrees;
            double minutes = (degrees - deg) * 60.0;
            int min = (int)minutes;
            double seconds = (minutes - min) * 60.0*100;
            string dms = deg.ToString() + '.' + min.ToString() + seconds.ToString("F0");
            return dms;
        }

        private void 计算ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double b = a * (1 - 1 / fdao);
            reports += "4," + "椭球短半轴 b," + b.ToString("F6") + '\n';

            double e12 = (a * a - b * b) / (a * a);
            double e22 = (a * a - b * b) / (b * b);
            reports += "5," + "第一偏心率平方 e2," + e12.ToString("F6") + '\n';
            reports += "6," + "第二偏心率平方 e'2," + e22.ToString("F6") + '\n' + '\n';

            double W1 = Math.Sqrt(1 - e12 * Math.Sin(elements[2].B1) * Math.Sin(elements[2].B1));
            double sinu1 = Math.Sin(elements[2].B1) * Math.Sqrt(1 - e12) / W1;
            double cosu1 = Math.Cos(elements[2].B1) / W1;
            reports += "7," + "第 3 条大地线的 W1," + W1.ToString("F3") + '\n';
            reports += "8," + "第 3 条大地线的 sinu1," + sinu1.ToString("F3") + '\n';
            reports += "9," + "第 3 条大地线的 cosu1," + cosu1.ToString("F3") + '\n';

            double sinA0 = cosu1 * Math.Sin(elements[2].A1);
            double cotseigema1 = cosu1 * Math.Cos(elements[2].A1) / sinu1;
            double seigema1 = Math.Atan(1 / cotseigema1);
            reports += "10," + "第 3 条大地线的 sinA0," + sinA0.ToString("F6") + '\n';
            reports += "11," + "第 3 条大地线的 cotσ1," + cotseigema1.ToString("F6") + '\n';
            reports += "12," + "第 3 条大地线的 σ1," + seigema1.ToString("F6") + '\n';
            double cosA02 = 1 - sinA0 * sinA0;
            double k2 = e22 * cosA02;
            double A = (1 - k2 / 4 + 7 * k2 * k2 / 64 - 15 * k2 * k2 * k2 / 256) / b;
            double B = (k2 / 4 - k2 * k2 / 8 + 37 * k2 * k2 * k2 / 512);
            double C = (k2 * k2 / 128 - k2 * k2 * k2 / 128);
            double alpha = (e12 / 2 + e12 * e12 / 8 + e12 * e12 * e12 / 16) - (e12 * e12 / 16 + e12 * e12 * e12 / 16) * cosA02 + (3 * e12 * e12 * e12 / 128) * cosA02 * cosA02;
            double beta = (e12 * e12 / 16 + e12 * e12 * e12 / 16) * cosA02 - (e12 * e12 * e12 / 32) * cosA02 * cosA02;
            double gama = (e12 * e12 * e12 / 256) * cosA02 * cosA02;
            reports += "13," + "第 3 条大地线的 A," + A.ToString("F8") + '\n';
            reports += "14," + "第 3 条大地线的 B," + B.ToString("F8") + '\n';
            reports += "15," + "第 3 条大地线的 C," + C.ToString("F8") + '\n';
            reports += "16," + "第 3 条大地线的 α," + alpha.ToString("F8") + '\n';
            reports += "17," + "第 3 条大地线的 β," + beta.ToString("F8") + '\n';
            reports += "18," + "第 3 条大地线的 γ," + gama.ToString("F8") + '\n';
            double segema2 = A * elements[2].S;
            double segema1 = 0;
            double segema = 0;
            while(segema2- segema > 1e-10)
            {
                segema = A * elements[2].S + B * Math.Sin(segema2) * Math.Cos(2 * seigema1 + segema2) + C * Math.Sin(2 * segema) * Math.Cos(4 * seigema1 + 2 * segema2);
                segema1 = segema2;
                segema2 = segema;
                segema = segema1;
            }
            double derta = (alpha * segema + beta * Math.Sin(segema) * Math.Cos(2 * seigema1 + segema) + gama * Math.Sin(2 * segema) * Math.Cos(4 * seigema1 + 2 * segema)) * sinA0;
            reports += "19," + "第 3 条大地线的球面长度σ ," + segema.ToString("F6") + '\n';
            reports += "20," + "第 3 条大地线经差改正数 ," + derta.ToString("F6") + '\n';
            double sinu2 = sinu1 * Math.Cos(segema) + cosu1 * Math.Cos(elements[2].A1) * Math.Sin(segema);
            double B2 = Math.Atan(sinu2 / (Math.Sqrt(1 - e12) * Math.Sqrt(1 - sinu2 * sinu2)));
            double lamuda = Math.Atan((Math.Sin(elements[2].A1) * Math.Sin(segema)) / (cosu1 * Math.Cos(segema) - sinu1 * Math.Sin(segema) * Math.Cos(elements[2].A1)));
            if (Math.Sin(elements[2].A1) > 0 && Math.Tan(lamuda) > 0)
            {
                lamuda = Math.Abs(lamuda);
            }
            else if(Math.Sin(elements[2].A1) > 0 && Math.Tan(lamuda) < 0)
            {
                lamuda=Math.PI - Math.Abs(lamuda);
            }
            else if (Math.Sin(elements[2].A1) < 0 && Math.Tan(lamuda) < 0)
            {
                lamuda =  - Math.Abs(lamuda);
            }
            else
            {
                lamuda = Math.Abs(lamuda) - Math.PI;
            }

            double L2 = elements[2].L1 + lamuda - derta;
            double A2 = Math.Atan(cosu1 * Math.Sin(elements[2].A1) / (cosu1 * Math.Cos(segema) * Math.Cos(elements[2].A1) - sinu1 * Math.Sin(segema)));
            if (Math.Sin(elements[2].A1) < 0 && Math.Tan(A2) > 0)
            {
                A2 = Math.Abs(A2);
            }
            else if (Math.Sin(elements[2].A1) < 0 && Math.Tan(A2) < 0)
            {
                A2 = Math.PI - Math.Abs(A2);
            }
            else if (Math.Sin(elements[2].A1) > 0 && Math.Tan(A2) > 0)
            {
                A2 = Math.PI + Math.Abs(A2);
            }
            else
            {
                A2 = 2 * Math.PI - Math.Abs(A2);
            }
            reports += "21," + "第 3 条大地线终点纬度B2 ," + rad2Dms(B2)+ '\n';
            reports += "22," + "第 3 条大地线终点经度L2 ," + rad2Dms(L2)+ '\n';
            reports += "23," + "第 3 条大地线终点坐标方位角A2 ," + rad2Dms(A2)+ '\n';
            richTextBox1.Text = reports;
            tabControl1.SelectedTab = tabPage2;
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void 报告ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.FileName = "result";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(richTextBox1.Text);
                }
            }
        }
    }
}
