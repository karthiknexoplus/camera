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
    public partial class EditAlarmTirgger : Form
    {
        private string No;
        private string Id;
        private string groupSwitch;
        private string recSwitch;

        public string[] returnStrs { get; set; }

        public EditAlarmTirgger(string[] args)
        {
            InitializeComponent();
            No = args[0];
            Id = args[1];
            groupSwitch = args[2];
            recSwitch = args[3];
            updateCtrl();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updateData();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void updateCtrl()
        {
            tb_groupSwitch.Text = groupSwitch;
            tb_recSwitch.Text = recSwitch;
        }
        private void updateData()
        {
            if (judgeInput())
            {
                groupSwitch = tb_groupSwitch.Text;
                recSwitch = tb_recSwitch.Text;
            }           
            returnStrs = new string[] { No, Id, groupSwitch, recSwitch };
        }
        private bool judgeInput()
        {
            if (tb_groupSwitch.Text.Length < 1)
            {
                MessageBox.Show("input groupSwitch");
                return false;
            }
            if (tb_recSwitch.Text.Length < 1)
            {
                MessageBox.Show("input recSwitch");
                return false;
            }
            return true;
                
        }
    }
}
