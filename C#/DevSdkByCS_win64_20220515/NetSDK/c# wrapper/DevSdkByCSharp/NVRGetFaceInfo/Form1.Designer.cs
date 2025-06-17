namespace NVRGetFaceInfo
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btn_AddListen = new System.Windows.Forms.Button();
            this.tb_Psw_L = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.tb_Name_L = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.tb_DeviceId = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.tb_Port_L = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btn_startListen = new System.Windows.Forms.Button();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.tb_BackupPathEx = new System.Windows.Forms.TextBox();
            this.btn_BrowserEx = new System.Windows.Forms.Button();
            this.btn_GetDevList = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tb_PortEx = new System.Windows.Forms.TextBox();
            this.tb_PswEx = new System.Windows.Forms.TextBox();
            this.btn_AddLogin = new System.Windows.Forms.Button();
            this.tb_UserNameEx = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tb_IPEx = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_Similarity = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_Num = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Export = new System.Windows.Forms.Button();
            this.tb_BackupPath = new System.Windows.Forms.TextBox();
            this.btn_Browser = new System.Windows.Forms.Button();
            this.dtp_SmartSearchEnd = new System.Windows.Forms.DateTimePicker();
            this.dtp_SmartSearchStart = new System.Windows.Forms.DateTimePicker();
            this.label85 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tb_Port = new System.Windows.Forms.TextBox();
            this.tb_Psw = new System.Windows.Forms.TextBox();
            this.btn_Login = new System.Windows.Forms.Button();
            this.tb_UserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_IP = new System.Windows.Forms.TextBox();
            this.IP = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(751, 348);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox5);
            this.tabPage1.Controls.Add(this.radioButton2);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.label12);
            this.tabPage1.Controls.Add(this.tb_BackupPathEx);
            this.tabPage1.Controls.Add(this.btn_BrowserEx);
            this.tabPage1.Controls.Add(this.btn_GetDevList);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(743, 322);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "实时比对";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_AddListen);
            this.groupBox5.Controls.Add(this.tb_Psw_L);
            this.groupBox5.Controls.Add(this.label16);
            this.groupBox5.Controls.Add(this.tb_Name_L);
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.tb_DeviceId);
            this.groupBox5.Controls.Add(this.label14);
            this.groupBox5.Controls.Add(this.tb_Port_L);
            this.groupBox5.Controls.Add(this.label13);
            this.groupBox5.Controls.Add(this.btn_startListen);
            this.groupBox5.Location = new System.Drawing.Point(11, 52);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(707, 101);
            this.groupBox5.TabIndex = 44;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "设备主动注册";
            // 
            // btn_AddListen
            // 
            this.btn_AddListen.Location = new System.Drawing.Point(593, 56);
            this.btn_AddListen.Name = "btn_AddListen";
            this.btn_AddListen.Size = new System.Drawing.Size(108, 23);
            this.btn_AddListen.TabIndex = 28;
            this.btn_AddListen.Text = "添加监听设备";
            this.btn_AddListen.UseVisualStyleBackColor = true;
            this.btn_AddListen.Click += new System.EventHandler(this.btn_AddListen_Click);
            // 
            // tb_Psw_L
            // 
            this.tb_Psw_L.Location = new System.Drawing.Point(459, 58);
            this.tb_Psw_L.Name = "tb_Psw_L";
            this.tb_Psw_L.PasswordChar = '*';
            this.tb_Psw_L.Size = new System.Drawing.Size(100, 21);
            this.tb_Psw_L.TabIndex = 26;
            this.tb_Psw_L.Text = "tvt123";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(389, 61);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(29, 12);
            this.label16.TabIndex = 27;
            this.label16.Text = "密码";
            // 
            // tb_Name_L
            // 
            this.tb_Name_L.Location = new System.Drawing.Point(275, 58);
            this.tb_Name_L.Name = "tb_Name_L";
            this.tb_Name_L.Size = new System.Drawing.Size(100, 21);
            this.tb_Name_L.TabIndex = 24;
            this.tb_Name_L.Text = "admin";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(205, 61);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 12);
            this.label15.TabIndex = 25;
            this.label15.Text = "用户名";
            // 
            // tb_DeviceId
            // 
            this.tb_DeviceId.Location = new System.Drawing.Point(89, 58);
            this.tb_DeviceId.Name = "tb_DeviceId";
            this.tb_DeviceId.Size = new System.Drawing.Size(100, 21);
            this.tb_DeviceId.TabIndex = 22;
            this.tb_DeviceId.Text = "22";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(19, 61);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 12);
            this.label14.TabIndex = 23;
            this.label14.Text = "设备 id";
            // 
            // tb_Port_L
            // 
            this.tb_Port_L.Location = new System.Drawing.Point(120, 22);
            this.tb_Port_L.Name = "tb_Port_L";
            this.tb_Port_L.Size = new System.Drawing.Size(100, 21);
            this.tb_Port_L.TabIndex = 21;
            this.tb_Port_L.Text = "2009";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(77, 12);
            this.label13.TabIndex = 20;
            this.label13.Text = "监听注册端口";
            // 
            // btn_startListen
            // 
            this.btn_startListen.Location = new System.Drawing.Point(237, 20);
            this.btn_startListen.Name = "btn_startListen";
            this.btn_startListen.Size = new System.Drawing.Size(75, 23);
            this.btn_startListen.TabIndex = 19;
            this.btn_startListen.Text = "开始接收";
            this.btn_startListen.UseVisualStyleBackColor = true;
            this.btn_startListen.Click += new System.EventHandler(this.btn_startListen_Click);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(369, 14);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(95, 16);
            this.radioButton2.TabIndex = 43;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "设备主动注册";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(484, 14);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(95, 16);
            this.radioButton1.TabIndex = 42;
            this.radioButton1.Text = "主动连接设备";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(9, 16);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(77, 12);
            this.label12.TabIndex = 41;
            this.label12.Text = "图片保存路径";
            // 
            // tb_BackupPathEx
            // 
            this.tb_BackupPathEx.Location = new System.Drawing.Point(94, 13);
            this.tb_BackupPathEx.Name = "tb_BackupPathEx";
            this.tb_BackupPathEx.Size = new System.Drawing.Size(168, 21);
            this.tb_BackupPathEx.TabIndex = 40;
            this.tb_BackupPathEx.Text = "D:\\123";
            // 
            // btn_BrowserEx
            // 
            this.btn_BrowserEx.Location = new System.Drawing.Point(271, 11);
            this.btn_BrowserEx.Name = "btn_BrowserEx";
            this.btn_BrowserEx.Size = new System.Drawing.Size(30, 23);
            this.btn_BrowserEx.TabIndex = 39;
            this.btn_BrowserEx.Text = "...";
            this.btn_BrowserEx.UseVisualStyleBackColor = true;
            this.btn_BrowserEx.Click += new System.EventHandler(this.btn_BrowserEx_Click);
            // 
            // btn_GetDevList
            // 
            this.btn_GetDevList.Location = new System.Drawing.Point(11, 215);
            this.btn_GetDevList.Name = "btn_GetDevList";
            this.btn_GetDevList.Size = new System.Drawing.Size(127, 23);
            this.btn_GetDevList.TabIndex = 26;
            this.btn_GetDevList.Text = "查询添加连接的设备";
            this.btn_GetDevList.UseVisualStyleBackColor = true;
            this.btn_GetDevList.Click += new System.EventHandler(this.btn_GetDevList_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tb_PortEx);
            this.groupBox4.Controls.Add(this.tb_PswEx);
            this.groupBox4.Controls.Add(this.btn_AddLogin);
            this.groupBox4.Controls.Add(this.tb_UserNameEx);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.tb_IPEx);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(11, 159);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(707, 50);
            this.groupBox4.TabIndex = 25;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "主动连接设备";
            // 
            // tb_PortEx
            // 
            this.tb_PortEx.Location = new System.Drawing.Point(506, 19);
            this.tb_PortEx.Name = "tb_PortEx";
            this.tb_PortEx.Size = new System.Drawing.Size(86, 21);
            this.tb_PortEx.TabIndex = 21;
            this.tb_PortEx.Text = "6036";
            // 
            // tb_PswEx
            // 
            this.tb_PswEx.Location = new System.Drawing.Point(379, 19);
            this.tb_PswEx.Name = "tb_PswEx";
            this.tb_PswEx.PasswordChar = '*';
            this.tb_PswEx.Size = new System.Drawing.Size(86, 21);
            this.tb_PswEx.TabIndex = 19;
            this.tb_PswEx.Text = "123456";
            // 
            // btn_AddLogin
            // 
            this.btn_AddLogin.Location = new System.Drawing.Point(598, 18);
            this.btn_AddLogin.Name = "btn_AddLogin";
            this.btn_AddLogin.Size = new System.Drawing.Size(103, 23);
            this.btn_AddLogin.TabIndex = 23;
            this.btn_AddLogin.Text = "添加连接设备";
            this.btn_AddLogin.UseVisualStyleBackColor = true;
            this.btn_AddLogin.Click += new System.EventHandler(this.btn_AddLogin_Click);
            // 
            // tb_UserNameEx
            // 
            this.tb_UserNameEx.Location = new System.Drawing.Point(209, 19);
            this.tb_UserNameEx.Name = "tb_UserNameEx";
            this.tb_UserNameEx.Size = new System.Drawing.Size(86, 21);
            this.tb_UserNameEx.TabIndex = 18;
            this.tb_UserNameEx.Text = "admin";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(136, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 15;
            this.label8.Text = "UserName";
            // 
            // tb_IPEx
            // 
            this.tb_IPEx.Location = new System.Drawing.Point(34, 19);
            this.tb_IPEx.Name = "tb_IPEx";
            this.tb_IPEx.Size = new System.Drawing.Size(86, 21);
            this.tb_IPEx.TabIndex = 20;
            this.tb_IPEx.Text = "10.100.20.144";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 22;
            this.label9.Text = "IP";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(471, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 17;
            this.label10.Text = "Port";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(306, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 16;
            this.label11.Text = "Password";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(743, 322);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "历史比对";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.tb_Similarity);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tb_Num);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.btn_Export);
            this.groupBox2.Controls.Add(this.tb_BackupPath);
            this.groupBox2.Controls.Add(this.btn_Browser);
            this.groupBox2.Controls.Add(this.dtp_SmartSearchEnd);
            this.groupBox2.Controls.Add(this.dtp_SmartSearchStart);
            this.groupBox2.Controls.Add(this.label85);
            this.groupBox2.Controls.Add(this.label86);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(6, 98);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(698, 176);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "获取图片";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 142);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(668, 23);
            this.progressBar1.TabIndex = 44;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(322, 91);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(11, 12);
            this.label7.TabIndex = 43;
            this.label7.Text = "%";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(175, 91);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 42;
            this.label6.Text = "相似度";
            // 
            // tb_Similarity
            // 
            this.tb_Similarity.Location = new System.Drawing.Point(245, 88);
            this.tb_Similarity.Name = "tb_Similarity";
            this.tb_Similarity.Size = new System.Drawing.Size(66, 21);
            this.tb_Similarity.TabIndex = 41;
            this.tb_Similarity.Text = "75";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 40;
            this.label5.Text = "导出数量";
            // 
            // tb_Num
            // 
            this.tb_Num.Location = new System.Drawing.Point(85, 88);
            this.tb_Num.Name = "tb_Num";
            this.tb_Num.Size = new System.Drawing.Size(66, 21);
            this.tb_Num.TabIndex = 39;
            this.tb_Num.Text = "10";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 38;
            this.label4.Text = "保存路径";
            // 
            // btn_Export
            // 
            this.btn_Export.Location = new System.Drawing.Point(309, 113);
            this.btn_Export.Name = "btn_Export";
            this.btn_Export.Size = new System.Drawing.Size(75, 23);
            this.btn_Export.TabIndex = 37;
            this.btn_Export.Text = "导出";
            this.btn_Export.UseVisualStyleBackColor = true;
            this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
            // 
            // tb_BackupPath
            // 
            this.tb_BackupPath.Location = new System.Drawing.Point(85, 115);
            this.tb_BackupPath.Name = "tb_BackupPath";
            this.tb_BackupPath.Size = new System.Drawing.Size(168, 21);
            this.tb_BackupPath.TabIndex = 36;
            this.tb_BackupPath.Text = "D:\\";
            // 
            // btn_Browser
            // 
            this.btn_Browser.Location = new System.Drawing.Point(262, 113);
            this.btn_Browser.Name = "btn_Browser";
            this.btn_Browser.Size = new System.Drawing.Size(30, 23);
            this.btn_Browser.TabIndex = 35;
            this.btn_Browser.Text = "...";
            this.btn_Browser.UseVisualStyleBackColor = true;
            this.btn_Browser.Click += new System.EventHandler(this.btn_Browser_Click);
            // 
            // dtp_SmartSearchEnd
            // 
            this.dtp_SmartSearchEnd.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_SmartSearchEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_SmartSearchEnd.Location = new System.Drawing.Point(54, 50);
            this.dtp_SmartSearchEnd.Name = "dtp_SmartSearchEnd";
            this.dtp_SmartSearchEnd.Size = new System.Drawing.Size(151, 21);
            this.dtp_SmartSearchEnd.TabIndex = 32;
            // 
            // dtp_SmartSearchStart
            // 
            this.dtp_SmartSearchStart.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtp_SmartSearchStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_SmartSearchStart.Location = new System.Drawing.Point(54, 20);
            this.dtp_SmartSearchStart.Name = "dtp_SmartSearchStart";
            this.dtp_SmartSearchStart.Size = new System.Drawing.Size(151, 21);
            this.dtp_SmartSearchStart.TabIndex = 31;
            // 
            // label85
            // 
            this.label85.AutoSize = true;
            this.label85.Location = new System.Drawing.Point(13, 56);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(23, 12);
            this.label85.TabIndex = 34;
            this.label85.Text = "End";
            // 
            // label86
            // 
            this.label86.AutoSize = true;
            this.label86.Location = new System.Drawing.Point(13, 26);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(35, 12);
            this.label86.TabIndex = 33;
            this.label86.Text = "Start";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(698, 86);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "登录";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_Port);
            this.groupBox3.Controls.Add(this.tb_Psw);
            this.groupBox3.Controls.Add(this.btn_Login);
            this.groupBox3.Controls.Add(this.tb_UserName);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tb_IP);
            this.groupBox3.Controls.Add(this.IP);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(17, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(666, 50);
            this.groupBox3.TabIndex = 24;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "主动连接";
            // 
            // tb_Port
            // 
            this.tb_Port.Location = new System.Drawing.Point(506, 19);
            this.tb_Port.Name = "tb_Port";
            this.tb_Port.Size = new System.Drawing.Size(86, 21);
            this.tb_Port.TabIndex = 21;
            this.tb_Port.Text = "6036";
            // 
            // tb_Psw
            // 
            this.tb_Psw.Location = new System.Drawing.Point(379, 19);
            this.tb_Psw.Name = "tb_Psw";
            this.tb_Psw.PasswordChar = '*';
            this.tb_Psw.Size = new System.Drawing.Size(86, 21);
            this.tb_Psw.TabIndex = 19;
            this.tb_Psw.Text = "123456";
            // 
            // btn_Login
            // 
            this.btn_Login.Location = new System.Drawing.Point(598, 18);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(55, 23);
            this.btn_Login.TabIndex = 23;
            this.btn_Login.Text = "Login";
            this.btn_Login.UseVisualStyleBackColor = true;
            this.btn_Login.Click += new System.EventHandler(this.btn_Login_Click);
            // 
            // tb_UserName
            // 
            this.tb_UserName.Location = new System.Drawing.Point(209, 19);
            this.tb_UserName.Name = "tb_UserName";
            this.tb_UserName.Size = new System.Drawing.Size(86, 21);
            this.tb_UserName.TabIndex = 18;
            this.tb_UserName.Text = "admin";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(136, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "UserName";
            // 
            // tb_IP
            // 
            this.tb_IP.Location = new System.Drawing.Point(34, 19);
            this.tb_IP.Name = "tb_IP";
            this.tb_IP.Size = new System.Drawing.Size(86, 21);
            this.tb_IP.TabIndex = 20;
            this.tb_IP.Text = "192.168.54.190";
            // 
            // IP
            // 
            this.IP.AutoSize = true;
            this.IP.Location = new System.Drawing.Point(11, 22);
            this.IP.Name = "IP";
            this.IP.Size = new System.Drawing.Size(17, 12);
            this.IP.TabIndex = 22;
            this.IP.Text = "IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(471, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(306, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 16;
            this.label2.Text = "Password";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button1);
            this.tabPage3.Controls.Add(this.panel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(743, 322);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(303, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 33;
            this.button1.Text = "start";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(94, 66);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(194, 144);
            this.panel1.TabIndex = 32;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 363);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NVR获取抓拍图";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_Similarity;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_Num;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_Export;
        private System.Windows.Forms.TextBox tb_BackupPath;
        private System.Windows.Forms.Button btn_Browser;
        private System.Windows.Forms.DateTimePicker dtp_SmartSearchEnd;
        private System.Windows.Forms.DateTimePicker dtp_SmartSearchStart;
        private System.Windows.Forms.Label label85;
        private System.Windows.Forms.Label label86;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tb_Port;
        private System.Windows.Forms.TextBox tb_Psw;
        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.TextBox tb_UserName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_IP;
        private System.Windows.Forms.Label IP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btn_GetDevList;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox tb_PortEx;
        private System.Windows.Forms.TextBox tb_PswEx;
        private System.Windows.Forms.Button btn_AddLogin;
        private System.Windows.Forms.TextBox tb_UserNameEx;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_IPEx;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tb_BackupPathEx;
        private System.Windows.Forms.Button btn_BrowserEx;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TextBox tb_Port_L;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btn_startListen;
        private System.Windows.Forms.TextBox tb_DeviceId;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox tb_Name_L;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox tb_Psw_L;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btn_AddListen;
    }
}

