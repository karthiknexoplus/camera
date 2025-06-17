namespace Demo
{
    partial class PlayBack_Dialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tkBar_PlayPos = new System.Windows.Forms.TrackBar();
            this.btn_PlayBack = new System.Windows.Forms.Button();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_Rew = new System.Windows.Forms.Button();
            this.btn_Ff = new System.Windows.Forms.Button();
            this.btn_Frame = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tkBar_Volume = new System.Windows.Forms.TrackBar();
            this.lb_Speed = new System.Windows.Forms.Label();
            this.btn_PlayBackSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Slow = new System.Windows.Forms.Button();
            this.btn_Fast = new System.Windows.Forms.Button();
            this.lb_format = new System.Windows.Forms.Label();
            this.textBox_PlaybackTitle = new System.Windows.Forms.TextBox();
            this.label_PlaybackTitle = new System.Windows.Forms.Label();
            this.btn_PlaybackTitle = new System.Windows.Forms.Button();
            this.btn_PlaybackChlName = new System.Windows.Forms.Button();
            this.btn_PlaybackTime = new System.Windows.Forms.Button();
            this.label_PlaybackRtsp = new System.Windows.Forms.Label();
            this.textBox_Rtsp = new System.Windows.Forms.TextBox();
            this.btn_Rtsp = new System.Windows.Forms.Button();
            this.comboFastSpeed = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tkBar_PlayPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkBar_Volume)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(8, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(381, 266);
            this.panel1.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gray;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(389, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(381, 266);
            this.panel2.TabIndex = 9;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Gray;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Location = new System.Drawing.Point(389, 271);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(381, 266);
            this.panel3.TabIndex = 11;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Gray;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Location = new System.Drawing.Point(8, 271);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(381, 266);
            this.panel4.TabIndex = 10;
            // 
            // tkBar_PlayPos
            // 
            this.tkBar_PlayPos.Location = new System.Drawing.Point(12, 543);
            this.tkBar_PlayPos.Maximum = 100;
            this.tkBar_PlayPos.Name = "tkBar_PlayPos";
            this.tkBar_PlayPos.Size = new System.Drawing.Size(746, 45);
            this.tkBar_PlayPos.TabIndex = 12;
            this.tkBar_PlayPos.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tkBar_PlayPos_MouseUp);
            // 
            // btn_PlayBack
            // 
            this.btn_PlayBack.Location = new System.Drawing.Point(12, 594);
            this.btn_PlayBack.Name = "btn_PlayBack";
            this.btn_PlayBack.Size = new System.Drawing.Size(31, 23);
            this.btn_PlayBack.TabIndex = 13;
            this.btn_PlayBack.Text = ">";
            this.btn_PlayBack.UseVisualStyleBackColor = true;
            this.btn_PlayBack.Click += new System.EventHandler(this.btn_PlayBack_Click);
            // 
            // btn_Stop
            // 
            this.btn_Stop.Enabled = false;
            this.btn_Stop.Location = new System.Drawing.Point(49, 594);
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.Size = new System.Drawing.Size(31, 23);
            this.btn_Stop.TabIndex = 14;
            this.btn_Stop.Text = "口";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_Rew
            // 
            this.btn_Rew.Enabled = false;
            this.btn_Rew.Location = new System.Drawing.Point(86, 594);
            this.btn_Rew.Name = "btn_Rew";
            this.btn_Rew.Size = new System.Drawing.Size(31, 23);
            this.btn_Rew.TabIndex = 15;
            this.btn_Rew.Text = "<<";
            this.btn_Rew.UseVisualStyleBackColor = true;
            this.btn_Rew.Click += new System.EventHandler(this.btn_Rew_Click);
            // 
            // btn_Ff
            // 
            this.btn_Ff.Enabled = false;
            this.btn_Ff.Location = new System.Drawing.Point(123, 594);
            this.btn_Ff.Name = "btn_Ff";
            this.btn_Ff.Size = new System.Drawing.Size(31, 23);
            this.btn_Ff.TabIndex = 16;
            this.btn_Ff.Text = ">>";
            this.btn_Ff.UseVisualStyleBackColor = true;
            this.btn_Ff.Click += new System.EventHandler(this.btn_Ff_Click);
            // 
            // btn_Frame
            // 
            this.btn_Frame.Enabled = false;
            this.btn_Frame.Location = new System.Drawing.Point(160, 594);
            this.btn_Frame.Name = "btn_Frame";
            this.btn_Frame.Size = new System.Drawing.Size(31, 23);
            this.btn_Frame.TabIndex = 17;
            this.btn_Frame.Text = "|>";
            this.btn_Frame.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(197, 599);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "Volume";
            // 
            // tkBar_Volume
            // 
            this.tkBar_Volume.Enabled = false;
            this.tkBar_Volume.Location = new System.Drawing.Point(241, 594);
            this.tkBar_Volume.Name = "tkBar_Volume";
            this.tkBar_Volume.Size = new System.Drawing.Size(148, 45);
            this.tkBar_Volume.TabIndex = 19;
            // 
            // lb_Speed
            // 
            this.lb_Speed.AutoSize = true;
            this.lb_Speed.Location = new System.Drawing.Point(395, 573);
            this.lb_Speed.Name = "lb_Speed";
            this.lb_Speed.Size = new System.Drawing.Size(35, 12);
            this.lb_Speed.TabIndex = 20;
            this.lb_Speed.Text = "speed";
            this.lb_Speed.Visible = false;
            // 
            // btn_PlayBackSave
            // 
            this.btn_PlayBackSave.Location = new System.Drawing.Point(472, 594);
            this.btn_PlayBackSave.Name = "btn_PlayBackSave";
            this.btn_PlayBackSave.Size = new System.Drawing.Size(42, 23);
            this.btn_PlayBackSave.TabIndex = 21;
            this.btn_PlayBackSave.Text = "Save";
            this.btn_PlayBackSave.UseVisualStyleBackColor = true;
            this.btn_PlayBackSave.Click += new System.EventHandler(this.btn_PlayBackSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(520, 599);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 12);
            this.label3.TabIndex = 22;
            this.label3.Text = "PlayBackChannels";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "1",
            "4"});
            this.comboBox1.Location = new System.Drawing.Point(627, 596);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(37, 20);
            this.comboBox1.TabIndex = 23;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(665, 587);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 24;
            // 
            // btn_Slow
            // 
            this.btn_Slow.Location = new System.Drawing.Point(670, 596);
            this.btn_Slow.Name = "btn_Slow";
            this.btn_Slow.Size = new System.Drawing.Size(44, 21);
            this.btn_Slow.TabIndex = 25;
            this.btn_Slow.Text = "Slow";
            this.btn_Slow.UseVisualStyleBackColor = true;
            this.btn_Slow.Click += new System.EventHandler(this.btn_Slow_Click);
            // 
            // btn_Fast
            // 
            this.btn_Fast.Location = new System.Drawing.Point(720, 596);
            this.btn_Fast.Name = "btn_Fast";
            this.btn_Fast.Size = new System.Drawing.Size(44, 21);
            this.btn_Fast.TabIndex = 26;
            this.btn_Fast.Text = "Fast";
            this.btn_Fast.UseVisualStyleBackColor = true;
            this.btn_Fast.Click += new System.EventHandler(this.btn_Fast_Click);
            // 
            // lb_format
            // 
            this.lb_format.AutoSize = true;
            this.lb_format.Location = new System.Drawing.Point(219, 572);
            this.lb_format.Name = "lb_format";
            this.lb_format.Size = new System.Drawing.Size(41, 12);
            this.lb_format.TabIndex = 27;
            this.lb_format.Text = "label4";
            this.lb_format.Visible = false;
            // 
            // textBox_PlaybackTitle
            // 
            this.textBox_PlaybackTitle.Location = new System.Drawing.Point(45, 621);
            this.textBox_PlaybackTitle.Name = "textBox_PlaybackTitle";
            this.textBox_PlaybackTitle.Size = new System.Drawing.Size(106, 21);
            this.textBox_PlaybackTitle.TabIndex = 28;
            // 
            // label_PlaybackTitle
            // 
            this.label_PlaybackTitle.AutoSize = true;
            this.label_PlaybackTitle.Location = new System.Drawing.Point(4, 625);
            this.label_PlaybackTitle.Name = "label_PlaybackTitle";
            this.label_PlaybackTitle.Size = new System.Drawing.Size(41, 12);
            this.label_PlaybackTitle.TabIndex = 29;
            this.label_PlaybackTitle.Text = "Title:";
            // 
            // btn_PlaybackTitle
            // 
            this.btn_PlaybackTitle.Enabled = false;
            this.btn_PlaybackTitle.Location = new System.Drawing.Point(155, 620);
            this.btn_PlaybackTitle.Name = "btn_PlaybackTitle";
            this.btn_PlaybackTitle.Size = new System.Drawing.Size(82, 23);
            this.btn_PlaybackTitle.TabIndex = 30;
            this.btn_PlaybackTitle.Text = "Title Open";
            this.btn_PlaybackTitle.UseVisualStyleBackColor = true;
            this.btn_PlaybackTitle.Click += new System.EventHandler(this.btn_PlaybackTitle_Click);
            // 
            // btn_PlaybackChlName
            // 
            this.btn_PlaybackChlName.Enabled = false;
            this.btn_PlaybackChlName.Location = new System.Drawing.Point(239, 620);
            this.btn_PlaybackChlName.Name = "btn_PlaybackChlName";
            this.btn_PlaybackChlName.Size = new System.Drawing.Size(89, 23);
            this.btn_PlaybackChlName.TabIndex = 30;
            this.btn_PlaybackChlName.Text = "ChlName Open";
            this.btn_PlaybackChlName.UseVisualStyleBackColor = true;
            this.btn_PlaybackChlName.Click += new System.EventHandler(this.btn_PlaybackChlName_Click);
            // 
            // btn_PlaybackTime
            // 
            this.btn_PlaybackTime.Enabled = false;
            this.btn_PlaybackTime.Location = new System.Drawing.Point(330, 620);
            this.btn_PlaybackTime.Name = "btn_PlaybackTime";
            this.btn_PlaybackTime.Size = new System.Drawing.Size(82, 23);
            this.btn_PlaybackTime.TabIndex = 30;
            this.btn_PlaybackTime.Text = "Time Open";
            this.btn_PlaybackTime.UseVisualStyleBackColor = true;
            this.btn_PlaybackTime.Click += new System.EventHandler(this.btn_PlaybackTime_Click);
            // 
            // label_PlaybackRtsp
            // 
            this.label_PlaybackRtsp.AutoSize = true;
            this.label_PlaybackRtsp.Location = new System.Drawing.Point(421, 626);
            this.label_PlaybackRtsp.Name = "label_PlaybackRtsp";
            this.label_PlaybackRtsp.Size = new System.Drawing.Size(59, 12);
            this.label_PlaybackRtsp.TabIndex = 29;
            this.label_PlaybackRtsp.Text = "Rtsp url:";
            // 
            // textBox_Rtsp
            // 
            this.textBox_Rtsp.Location = new System.Drawing.Point(478, 621);
            this.textBox_Rtsp.Name = "textBox_Rtsp";
            this.textBox_Rtsp.Size = new System.Drawing.Size(243, 21);
            this.textBox_Rtsp.TabIndex = 31;
            // 
            // btn_Rtsp
            // 
            this.btn_Rtsp.Location = new System.Drawing.Point(721, 621);
            this.btn_Rtsp.Name = "btn_Rtsp";
            this.btn_Rtsp.Size = new System.Drawing.Size(44, 21);
            this.btn_Rtsp.TabIndex = 26;
            this.btn_Rtsp.Text = "Rtsp";
            this.btn_Rtsp.UseVisualStyleBackColor = true;
            this.btn_Rtsp.Click += new System.EventHandler(this.btn_Fast_Click);
            // 
            // comboFastSpeed
            // 
            this.comboFastSpeed.FormattingEnabled = true;
            this.comboFastSpeed.Items.AddRange(new object[] {
            "1/32x",
            "1/16x",
            "1/8x",
            "1/4x",
            "1/2x",
            "1x",
            "2x",
            "4x",
            "8x",
            "16x",
            "32x"});
            this.comboFastSpeed.Location = new System.Drawing.Point(389, 595);
            this.comboFastSpeed.Name = "comboFastSpeed";
            this.comboFastSpeed.Size = new System.Drawing.Size(78, 20);
            this.comboFastSpeed.TabIndex = 32;
            // 
            // PlayBack_Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 643);
            this.Controls.Add(this.comboFastSpeed);
            this.Controls.Add(this.textBox_Rtsp);
            this.Controls.Add(this.btn_PlaybackTime);
            this.Controls.Add(this.btn_PlaybackChlName);
            this.Controls.Add(this.btn_PlaybackTitle);
            this.Controls.Add(this.label_PlaybackRtsp);
            this.Controls.Add(this.label_PlaybackTitle);
            this.Controls.Add(this.textBox_PlaybackTitle);
            this.Controls.Add(this.lb_format);
            this.Controls.Add(this.btn_Rtsp);
            this.Controls.Add(this.btn_Fast);
            this.Controls.Add(this.btn_Slow);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_PlayBackSave);
            this.Controls.Add(this.lb_Speed);
            this.Controls.Add(this.tkBar_Volume);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Frame);
            this.Controls.Add(this.btn_Ff);
            this.Controls.Add(this.btn_Rew);
            this.Controls.Add(this.btn_Stop);
            this.Controls.Add(this.btn_PlayBack);
            this.Controls.Add(this.tkBar_PlayPos);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PlayBack_Dialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PlayBack_Dialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlayBack_Dialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tkBar_PlayPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tkBar_Volume)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TrackBar tkBar_PlayPos;
        private System.Windows.Forms.Button btn_PlayBack;
        private System.Windows.Forms.Button btn_Stop;
        private System.Windows.Forms.Button btn_Rew;
        private System.Windows.Forms.Button btn_Ff;
        private System.Windows.Forms.Button btn_Frame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar tkBar_Volume;
        private System.Windows.Forms.Label lb_Speed;
        private System.Windows.Forms.Button btn_PlayBackSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Slow;
        private System.Windows.Forms.Button btn_Fast;
        private System.Windows.Forms.Label lb_format;
        private System.Windows.Forms.TextBox textBox_PlaybackTitle;
        private System.Windows.Forms.Label label_PlaybackTitle;
        private System.Windows.Forms.Button btn_PlaybackTitle;
        private System.Windows.Forms.Button btn_PlaybackChlName;
        private System.Windows.Forms.Button btn_PlaybackTime;
        private System.Windows.Forms.Label label_PlaybackRtsp;
        private System.Windows.Forms.TextBox textBox_Rtsp;
        private System.Windows.Forms.Button btn_Rtsp;
        private System.Windows.Forms.ComboBox comboFastSpeed;
    }
}