
namespace BlankApp.Correction
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
            this.txt = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.gb = new System.Windows.Forms.GroupBox();
            this.gb.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt
            // 
            this.txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt.Font = new System.Drawing.Font("等线", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txt.Location = new System.Drawing.Point(3, 17);
            this.txt.Multiline = true;
            this.txt.Name = "txt";
            this.txt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txt.Size = new System.Drawing.Size(443, 240);
            this.txt.TabIndex = 0;
            this.txt.TextChanged += new System.EventHandler(this.txt_TextChanged);
            this.txt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_KeyUp);
            this.txt.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txt_MouseUp);
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBox1.Font = new System.Drawing.Font("等线", 12F);
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(449, 24);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // gb
            // 
            this.gb.Controls.Add(this.txt);
            this.gb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gb.Location = new System.Drawing.Point(0, 24);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(449, 260);
            this.gb.TabIndex = 2;
            this.gb.TabStop = false;
            this.gb.Text = "纠错区";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 284);
            this.Controls.Add(this.gb);
            this.Controls.Add(this.textBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "自动纠错";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.gb.ResumeLayout(false);
            this.gb.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox gb;
    }
}

