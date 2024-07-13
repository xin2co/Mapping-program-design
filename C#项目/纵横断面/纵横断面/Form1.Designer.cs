namespace 纵横断面
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.退出ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.横断面中心店计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.道路横断面计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.横断面插值点的ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.内插点平面坐标计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.纵断面长度计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.道路纵断面计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.断面面积计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.反距离加权法ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.内插点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.坐标方位角计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.数据文件读入ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.基本计算ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.richTextBox1.Location = new System.Drawing.Point(3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1309, 708);
            this.richTextBox1.TabIndex = 1;
            this.richTextBox1.Text = "";
            // 
            // Column4
            // 
            this.Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column4.HeaderText = "Z(m)";
            this.Column4.MinimumWidth = 8;
            this.Column4.Name = "Column4";
            // 
            // Column3
            // 
            this.Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column3.HeaderText = "Y(m)";
            this.Column3.MinimumWidth = 8;
            this.Column3.Name = "Column3";
            // 
            // Column2
            // 
            this.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column2.HeaderText = "X(m)";
            this.Column2.MinimumWidth = 8;
            this.Column2.Name = "Column2";
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.HeaderText = "点名";
            this.Column1.MinimumWidth = 8;
            this.Column1.Name = "Column1";
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
            this.dataGridView1.Size = new System.Drawing.Size(1309, 714);
            this.dataGridView1.TabIndex = 0;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(82, 24);
            this.toolStripStatusLabel1.Text = "欢迎使用";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(3, 688);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1309, 29);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.statusStrip1);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage1.Size = new System.Drawing.Size(1315, 720);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "点数据";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            this.tabControl1.Size = new System.Drawing.Size(1323, 752);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.richTextBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPage2.Size = new System.Drawing.Size(1315, 714);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "计算报告";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // 退出ToolStripMenuItem1
            // 
            this.退出ToolStripMenuItem1.Name = "退出ToolStripMenuItem1";
            this.退出ToolStripMenuItem1.Size = new System.Drawing.Size(58, 28);
            this.退出ToolStripMenuItem1.Text = "退出";
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(58, 28);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(130, 28);
            this.退出ToolStripMenuItem.Text = "保存当前报告";
            // 
            // 横断面中心店计算ToolStripMenuItem
            // 
            this.横断面中心店计算ToolStripMenuItem.Name = "横断面中心店计算ToolStripMenuItem";
            this.横断面中心店计算ToolStripMenuItem.Size = new System.Drawing.Size(380, 30);
            this.横断面中心店计算ToolStripMenuItem.Text = "横断面中心点计算";
            // 
            // 道路横断面计算ToolStripMenuItem
            // 
            this.道路横断面计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.横断面中心店计算ToolStripMenuItem,
            this.横断面插值点的ToolStripMenuItem});
            this.道路横断面计算ToolStripMenuItem.Name = "道路横断面计算ToolStripMenuItem";
            this.道路横断面计算ToolStripMenuItem.Size = new System.Drawing.Size(148, 28);
            this.道路横断面计算ToolStripMenuItem.Text = "道路横断面计算";
            // 
            // 横断面插值点的ToolStripMenuItem
            // 
            this.横断面插值点的ToolStripMenuItem.Name = "横断面插值点的ToolStripMenuItem";
            this.横断面插值点的ToolStripMenuItem.Size = new System.Drawing.Size(380, 30);
            this.横断面插值点的ToolStripMenuItem.Text = "横断面插值点的平面坐标和高程计算";
            // 
            // 内插点平面坐标计算ToolStripMenuItem
            // 
            this.内插点平面坐标计算ToolStripMenuItem.Name = "内插点平面坐标计算ToolStripMenuItem";
            this.内插点平面坐标计算ToolStripMenuItem.Size = new System.Drawing.Size(254, 30);
            this.内插点平面坐标计算ToolStripMenuItem.Text = "内插点平面坐标计算";
            // 
            // 纵断面长度计算ToolStripMenuItem
            // 
            this.纵断面长度计算ToolStripMenuItem.Name = "纵断面长度计算ToolStripMenuItem";
            this.纵断面长度计算ToolStripMenuItem.Size = new System.Drawing.Size(254, 30);
            this.纵断面长度计算ToolStripMenuItem.Text = "纵断面长度计算";
            // 
            // 道路纵断面计算ToolStripMenuItem
            // 
            this.道路纵断面计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.纵断面长度计算ToolStripMenuItem,
            this.内插点平面坐标计算ToolStripMenuItem});
            this.道路纵断面计算ToolStripMenuItem.Name = "道路纵断面计算ToolStripMenuItem";
            this.道路纵断面计算ToolStripMenuItem.Size = new System.Drawing.Size(148, 28);
            this.道路纵断面计算ToolStripMenuItem.Text = "道路纵断面计算";
            // 
            // 断面面积计算ToolStripMenuItem
            // 
            this.断面面积计算ToolStripMenuItem.Name = "断面面积计算ToolStripMenuItem";
            this.断面面积计算ToolStripMenuItem.Size = new System.Drawing.Size(200, 30);
            this.断面面积计算ToolStripMenuItem.Text = "断面面积计算";
            // 
            // 反距离加权法ToolStripMenuItem
            // 
            this.反距离加权法ToolStripMenuItem.Name = "反距离加权法ToolStripMenuItem";
            this.反距离加权法ToolStripMenuItem.Size = new System.Drawing.Size(200, 30);
            this.反距离加权法ToolStripMenuItem.Text = "反距离加权法";
            // 
            // 内插点ToolStripMenuItem
            // 
            this.内插点ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.反距离加权法ToolStripMenuItem,
            this.断面面积计算ToolStripMenuItem});
            this.内插点ToolStripMenuItem.Name = "内插点ToolStripMenuItem";
            this.内插点ToolStripMenuItem.Size = new System.Drawing.Size(166, 28);
            this.内插点ToolStripMenuItem.Text = "内插点高程值计算";
            // 
            // 坐标方位角计算ToolStripMenuItem
            // 
            this.坐标方位角计算ToolStripMenuItem.Name = "坐标方位角计算ToolStripMenuItem";
            this.坐标方位角计算ToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.坐标方位角计算ToolStripMenuItem.Text = "坐标方位角计算";
            // 
            // 数据文件读入ToolStripMenuItem
            // 
            this.数据文件读入ToolStripMenuItem.Name = "数据文件读入ToolStripMenuItem";
            this.数据文件读入ToolStripMenuItem.Size = new System.Drawing.Size(218, 30);
            this.数据文件读入ToolStripMenuItem.Text = "数据文件读入";
            // 
            // 基本计算ToolStripMenuItem
            // 
            this.基本计算ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.数据文件读入ToolStripMenuItem,
            this.坐标方位角计算ToolStripMenuItem});
            this.基本计算ToolStripMenuItem.Name = "基本计算ToolStripMenuItem";
            this.基本计算ToolStripMenuItem.Size = new System.Drawing.Size(94, 28);
            this.基本计算ToolStripMenuItem.Text = "基本计算";
            // 
            // menuStrip1
            // 
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
            this.menuStrip1.Size = new System.Drawing.Size(1323, 32);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1323, 784);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "纵横断面计算";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 横断面中心店计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 道路横断面计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 横断面插值点的ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 内插点平面坐标计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 纵断面长度计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 道路纵断面计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 断面面积计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 反距离加权法ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 内插点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 坐标方位角计算ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 数据文件读入ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 基本计算ToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
    }
}

