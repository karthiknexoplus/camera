using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices; 

namespace Demo
{
    public partial class Msg_Dialog : Form
    {
        private bool inited = false;
        public Msg_Dialog()
        {
            InitializeComponent();
            inited = true;
        }

        private void btn_msg_clear_Click(object sender, EventArgs e)
        {
            lv_msg.Items.Clear();
        }
        public void ShowMsg(object msg)
        {
            ListViewItem item = new ListViewItem(new string[] { msg.ToString() });
            lv_msg.Items.Add(item); 
        }
       // protected override void DefWndProc(ref System.Windows.Forms.Message m)    
       // {
       //     switch (m.Msg)
       //     {
       //         //case Live_Dialog.CUSTOM_MESSAGE:
       //         case 1:
       //             ListViewItem item = new ListViewItem(new string[] {m.Msg.ToString() });
       //             lv_msg.Items.Add(item); 
       //             break;
       //         default:                   
       //             base.DefWndProc(ref m);
       //             break;
       //     }   
       //}  
    }
}
