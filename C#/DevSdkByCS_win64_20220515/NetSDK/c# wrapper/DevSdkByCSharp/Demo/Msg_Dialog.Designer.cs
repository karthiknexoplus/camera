namespace Demo
{
    partial class Msg_Dialog
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
            this.lv_msg = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btn_msg_clear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lv_msg
            // 
            this.lv_msg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lv_msg.FullRowSelect = true;
            this.lv_msg.Location = new System.Drawing.Point(12, 13);
            this.lv_msg.Name = "lv_msg";
            this.lv_msg.Size = new System.Drawing.Size(795, 217);
            this.lv_msg.TabIndex = 2;
            this.lv_msg.UseCompatibleStateImageBehavior = false;
            this.lv_msg.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "msg";
            this.columnHeader1.Width = 360;
            // 
            // btn_msg_clear
            // 
            this.btn_msg_clear.Location = new System.Drawing.Point(159, 237);
            this.btn_msg_clear.Name = "btn_msg_clear";
            this.btn_msg_clear.Size = new System.Drawing.Size(75, 25);
            this.btn_msg_clear.TabIndex = 3;
            this.btn_msg_clear.Text = "Clear";
            this.btn_msg_clear.UseVisualStyleBackColor = true;
            this.btn_msg_clear.Click += new System.EventHandler(this.btn_msg_clear_Click);
            // 
            // Msg_Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(819, 271);
            this.Controls.Add(this.btn_msg_clear);
            this.Controls.Add(this.lv_msg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Msg_Dialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Message";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lv_msg;
        private System.Windows.Forms.Button btn_msg_clear;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}