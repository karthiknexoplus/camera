using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Demo
{
    public partial class ChangePsw : Form
    {
        private string userName;
        public string currentPsw { get; set; }
        public string newPsw { get; set; }
        public ChangePsw()
        {
            InitializeComponent();
            
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (tb_Current.Text.Trim().Length < 6)
            {
                MessageBox.Show("wrong pass word");
                return;
            }
            if (tb_New.Text.Trim().Length < 6)
            {
                MessageBox.Show("invalid new password ");
                return;
            }
            if (tb_Confirm.Text.Trim().Length < 6)
            {
                MessageBox.Show("invalid confirm password ");
                return;
            }
            if (tb_New.Text.Trim() != tb_Confirm.Text.Trim())
            {
                MessageBox.Show("wrong confirm password ");
                return;
            }
            currentPsw = tb_Current.Text.Trim();
            newPsw = tb_New.Text.Trim();
      
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        public void SetUserName(String name)
        {
            userName = name;
            lb_Name.Text = userName;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
