namespace Demo
{
    partial class DiscoverDevices
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
            this.lv_DevInfo = new System.Windows.Forms.ListView();
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_product_subID = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // lv_DevInfo
            // 
            this.lv_DevInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader20,
            this.columnHeader1});
            this.lv_DevInfo.FullRowSelect = true;
            this.lv_DevInfo.GridLines = true;
            this.lv_DevInfo.Location = new System.Drawing.Point(12, 13);
            this.lv_DevInfo.Name = "lv_DevInfo";
            this.lv_DevInfo.Size = new System.Drawing.Size(460, 375);
            this.lv_DevInfo.TabIndex = 2;
            this.lv_DevInfo.UseCompatibleStateImageBehavior = false;
            this.lv_DevInfo.View = System.Windows.Forms.View.Details;
            this.lv_DevInfo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lv_DevInfo_MouseDoubleClick);
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "IP";
            this.columnHeader15.Width = 140;
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Type";
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "ProductName";
            this.columnHeader20.Width = 140;
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(12, 394);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 25);
            this.btn_OK.TabIndex = 3;
            this.btn_OK.Text = "Discover";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_product_subID
            // 
            this.btn_product_subID.Location = new System.Drawing.Point(160, 394);
            this.btn_product_subID.Name = "btn_product_subID";
            this.btn_product_subID.Size = new System.Drawing.Size(75, 25);
            this.btn_product_subID.TabIndex = 4;
            this.btn_product_subID.Text = "Login";
            this.btn_product_subID.UseVisualStyleBackColor = true;
            this.btn_product_subID.Click += new System.EventHandler(this.btn_product_subID_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(303, 394);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 25);
            this.btn_Cancel.TabIndex = 5;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "SecondIP";
            this.columnHeader1.Width = 97;
            // 
            // DiscoverDevices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(485, 433);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_product_subID);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.lv_DevInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DiscoverDevices";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DiscoverDevices";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lv_DevInfo;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private System.Windows.Forms.ColumnHeader columnHeader16;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_product_subID;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}