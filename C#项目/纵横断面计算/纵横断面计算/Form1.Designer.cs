
namespace 纵横断面计算
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.基本计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据文件读入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.坐标方位角计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.内插点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.反距离加权法ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.断面面积计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.道路纵断面计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.纵断面长度计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.内插点平面坐标计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.道路横断面计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.横断面中心店计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.横断面插值点的ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.基本计算ToolStripMenuItem,
            this.内插点ToolStripMenuItem,
            this.道路纵断面计算ToolStripMenuItem,
            this.道路横断面计算ToolStripMenuItem,
            this.退出ToolStripMenuItem,
            this.帮助ToolStripMenuItem,
            this.退出ToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1037, 32);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 基本计算ToolStripMenuItem
            // 
            this.基本计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.数据文件读入ToolStripMenuItem,
            this.坐标方位角计算ToolStripMenuItem});
            this.基本计算ToolStripMenuItem.Name = "基本计算ToolStripMenuItem";
            this.基本计算ToolStripMenuItem.Size = new System.Drawing.Size(98, 28);
            this.基本计算ToolStripMenuItem.Text = "基本计算";
            // 
            // 数据文件读入ToolStripMenuItem
            // 
            this.数据文件读入ToolStripMenuItem.Name = "数据文件读入ToolStripMenuItem";
            this.数据文件读入ToolStripMenuItem.Size = new System.Drawing.Size(236, 34);
            this.数据文件读入ToolStripMenuItem.Text = "数据文件读入";
            this.数据文件读入ToolStripMenuItem.Click += new System.EventHandler(this.数据文件读入ToolStripMenuItem_Click);
            // 
            // 坐标方位角计算ToolStripMenuItem
            // 
            this.坐标方位角计算ToolStripMenuItem.Name = "坐标方位角计算ToolStripMenuItem";
            this.坐标方位角计算ToolStripMenuItem.Size = new System.Drawing.Size(236, 34);
            this.坐标方位角计算ToolStripMenuItem.Text = "坐标方位角计算";
            this.坐标方位角计算ToolStripMenuItem.Click += new System.EventHandler(this.坐标方位角计算ToolStripMenuItem_Click);
            // 
            // 内插点ToolStripMenuItem
            // 
            this.内插点ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.反距离加权法ToolStripMenuItem,
            this.断面面积计算ToolStripMenuItem});
            this.内插点ToolStripMenuItem.Name = "内插点ToolStripMenuItem";
            this.内插点ToolStripMenuItem.Size = new System.Drawing.Size(170, 28);
            this.内插点ToolStripMenuItem.Text = "内插点高程值计算";
            // 
            // 反距离加权法ToolStripMenuItem
            // 
            this.反距离加权法ToolStripMenuItem.Name = "反距离加权法ToolStripMenuItem";
            this.反距离加权法ToolStripMenuItem.Size = new System.Drawing.Size(218, 34);
            this.反距离加权法ToolStripMenuItem.Text = "反距离加权法";
            this.反距离加权法ToolStripMenuItem.Click += new System.EventHandler(this.反距离加权法ToolStripMenuItem_Click);
            // 
            // 断面面积计算ToolStripMenuItem
            // 
            this.断面面积计算ToolStripMenuItem.Name = "断面面积计算ToolStripMenuItem";
            this.断面面积计算ToolStripMenuItem.Size = new System.Drawing.Size(218, 34);
            this.断面面积计算ToolStripMenuItem.Text = "断面面积计算";
            this.断面面积计算ToolStripMenuItem.Click += new System.EventHandler(this.断面面积计算ToolStripMenuItem_Click);
            // 
            // 道路纵断面计算ToolStripMenuItem
            // 
            this.道路纵断面计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.纵断面长度计算ToolStripMenuItem,
            this.内插点平面坐标计算ToolStripMenuItem});
            this.道路纵断面计算ToolStripMenuItem.Name = "道路纵断面计算ToolStripMenuItem";
            this.道路纵断面计算ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.道路纵断面计算ToolStripMenuItem.Text = "道路纵断面计算";
            // 
            // 纵断面长度计算ToolStripMenuItem
            // 
            this.纵断面长度计算ToolStripMenuItem.Name = "纵断面长度计算ToolStripMenuItem";
            this.纵断面长度计算ToolStripMenuItem.Size = new System.Drawing.Size(272, 34);
            this.纵断面长度计算ToolStripMenuItem.Text = "纵断面长度计算";
            this.纵断面长度计算ToolStripMenuItem.Click += new System.EventHandler(this.纵断面长度计算ToolStripMenuItem_Click);
            // 
            // 内插点平面坐标计算ToolStripMenuItem
            // 
            this.内插点平面坐标计算ToolStripMenuItem.Name = "内插点平面坐标计算ToolStripMenuItem";
            this.内插点平面坐标计算ToolStripMenuItem.Size = new System.Drawing.Size(272, 34);
            this.内插点平面坐标计算ToolStripMenuItem.Text = "内插点平面坐标计算";
            this.内插点平面坐标计算ToolStripMenuItem.Click += new System.EventHandler(this.内插点平面坐标计算ToolStripMenuItem_Click);
            // 
            // 道路横断面计算ToolStripMenuItem
            // 
            this.道路横断面计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.横断面中心店计算ToolStripMenuItem,
            this.横断面插值点的ToolStripMenuItem});
            this.道路横断面计算ToolStripMenuItem.Name = "道路横断面计算ToolStripMenuItem";
            this.道路横断面计算ToolStripMenuItem.Size = new System.Drawing.Size(152, 28);
            this.道路横断面计算ToolStripMenuItem.Text = "道路横断面计算";
            // 
            // 横断面中心店计算ToolStripMenuItem
            // 
            this.横断面中心店计算ToolStripMenuItem.Name = "横断面中心店计算ToolStripMenuItem";
            this.横断面中心店计算ToolStripMenuItem.Size = new System.Drawing.Size(398, 34);
            this.横断面中心店计算ToolStripMenuItem.Text = "横断面中心点计算";
            this.横断面中心店计算ToolStripMenuItem.Click += new System.EventHandler(this.横断面中心店计算ToolStripMenuItem_Click);
            // 
            // 横断面插值点的ToolStripMenuItem
            // 
            this.横断面插值点的ToolStripMenuItem.Name = "横断面插值点的ToolStripMenuItem";
            this.横断面插值点的ToolStripMenuItem.Size = new System.Drawing.Size(398, 34);
            this.横断面插值点的ToolStripMenuItem.Text = "横断面插值点的平面坐标和高程计算";
            this.横断面插值点的ToolStripMenuItem.Click += new System.EventHandler(this.横断面插值点的ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(134, 28);
            this.退出ToolStripMenuItem.Text = "保存当前报告";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.保存当前报告ToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(62, 28);
            this.帮助ToolStripMenuItem.Text = "帮助";
            this.帮助ToolStripMenuItem.Click += new System.EventHandler(this.帮助ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem1
            // 
            this.退出ToolStripMenuItem1.Name = "退出ToolStripMenuItem1";
            this.退出ToolStripMenuItem1.Size = new System.Drawing.Size(62, 28);
            this.退出ToolStripMenuItem1.Text = "退出";
            this.退出ToolStripMenuItem1.Click += new System.EventHandler(this.退出ToolStripMenuItem1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 32);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1037, 597);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusStrip1);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1029, 565);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "点数据";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(3, 531);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1023, 31);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(82, 24);
            this.toolStripStatusLabel1.Text = "欢迎使用";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(1023, 559);
            this.dataGridView1.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "点名";
            this.Column1.MinimumWidth = 8;
            this.Column1.Name = "Column1";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "X(m)";
            this.Column2.MinimumWidth = 8;
            this.Column2.Name = "Column2";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "Y(m)";
            this.Column3.MinimumWidth = 8;
            this.Column3.Name = "Column3";
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "Z(m)";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1029, 565);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "计算报告";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1023, 559);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1037, 629);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "纵横断面计算";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 基本计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据文件读入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 坐标方位角计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 内插点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 反距离加权法ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 断面面积计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 道路纵断面计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 纵断面长度计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 内插点平面坐标计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 道路横断面计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 横断面中心店计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 横断面插值点的ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem1;
    }
}

