namespace FaceCaptureAndFTPEx
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
            this.btn_start = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_Psw = new System.Windows.Forms.TextBox();
            this.tb_Name = new System.Windows.Forms.TextBox();
            this.tb_DeviceId = new System.Windows.Forms.TextBox();
            this.tb_Port = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_AddListen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(237, 12);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 0;
            this.btn_start.Text = "开始接收";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.button1_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "密码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "用户名";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "设备 id";
            // 
            // tb_Psw
            // 
            this.tb_Psw.Location = new System.Drawing.Point(94, 87);
            this.tb_Psw.Name = "tb_Psw";
            this.tb_Psw.Size = new System.Drawing.Size(100, 21);
            this.tb_Psw.TabIndex = 13;
            this.tb_Psw.Text = "123456";
            // 
            // tb_Name
            // 
            this.tb_Name.Location = new System.Drawing.Point(94, 60);
            this.tb_Name.Name = "tb_Name";
            this.tb_Name.Size = new System.Drawing.Size(100, 21);
            this.tb_Name.TabIndex = 12;
            this.tb_Name.Text = "admin";
            // 
            // tb_DeviceId
            // 
            this.tb_DeviceId.Location = new System.Drawing.Point(94, 33);
            this.tb_DeviceId.Name = "tb_DeviceId";
            this.tb_DeviceId.Size = new System.Drawing.Size(100, 21);
            this.tb_DeviceId.TabIndex = 11;
            this.tb_DeviceId.Text = "2012";
            // 
            // tb_Port
            // 
            this.tb_Port.Location = new System.Drawing.Point(120, 14);
            this.tb_Port.Name = "tb_Port";
            this.tb_Port.Size = new System.Drawing.Size(100, 21);
            this.tb_Port.TabIndex = 18;
            this.tb_Port.Text = "2009";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 17;
            this.label1.Text = "监听注册端口";
            // 
            // btn_AddListen
            // 
            this.btn_AddListen.Location = new System.Drawing.Point(26, 124);
            this.btn_AddListen.Name = "btn_AddListen";
            this.btn_AddListen.Size = new System.Drawing.Size(108, 23);
            this.btn_AddListen.TabIndex = 19;
            this.btn_AddListen.Text = "添加监听设备";
            this.btn_AddListen.UseVisualStyleBackColor = true;
            this.btn_AddListen.Click += new System.EventHandler(this.btn_AddListen_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_Psw);
            this.groupBox1.Controls.Add(this.btn_AddListen);
            this.groupBox1.Controls.Add(this.tb_DeviceId);
            this.groupBox1.Controls.Add(this.tb_Name);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 167);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 240);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tb_Port);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_start);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_Psw;
        private System.Windows.Forms.TextBox tb_Name;
        private System.Windows.Forms.TextBox tb_DeviceId;
        private System.Windows.Forms.TextBox tb_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_AddListen;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

