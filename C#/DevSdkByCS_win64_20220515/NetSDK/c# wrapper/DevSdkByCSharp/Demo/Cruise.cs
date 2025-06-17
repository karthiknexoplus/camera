using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevSdkByCS;

namespace Demo
{
#if OS64
    using POINTERHANDLE = Int64;
#else
    using POINTERHANDLE = Int32;
#endif
    public partial class Cruise : Form
    {
        public int m_cruiseNo;
        private int userId = 0;
        private POINTERHANDLE m_playHandle = -1;
        public DD_CRUISE_POINT_INFO[] pCruisePoint
        {
            get;set;
        }
        


        //public Cruise(int id,int lLiveHandle = 0)
        public Cruise()
        {
            //userId = id;
            //m_playHandle = lLiveHandle;
            InitializeComponent();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {           
            int index = dataGridView1.Rows.Add();

            dataGridView1.Rows[index].Cells[0].Value = "Preset" + (index + 1);
            dataGridView1.Rows[index].Cells[1].Value = "5s";
            dataGridView1.Rows[index].Cells[2].Value = "4";
                            
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            for (int i = this.dataGridView1.SelectedRows.Count; i > 0; i--)
            {
                if (dataGridView1.SelectedRows[i - 1].Index != dataGridView1.Rows.Count - 1)
                {
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[i - 1].Index);

                }
            }         
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            int count = dataGridView1.Rows.Count;
            DD_CRUISE_POINT_INFO cruiseInfo = new DD_CRUISE_POINT_INFO();
            pCruisePoint = new DD_CRUISE_POINT_INFO[count];
            for (int i = 0; i < count; i++)
            {
                string tempName = dataGridView1.Rows[i].Cells[0].Value.ToString();
                string tempTime = dataGridView1.Rows[i].Cells[1].Value.ToString();
                string tempSpeed = dataGridView1.Rows[i].Cells[2].Value.ToString();
                cruiseInfo.presetIndex = Convert.ToUInt32(tempName.Substring(tempName.Length - 1));
                cruiseInfo.dwellTime = Convert.ToUInt32(tempTime.Substring(0, tempTime.Length - 1));
                cruiseInfo.dwellSpeed = Convert.ToUInt32(tempSpeed);

                pCruisePoint[i] = cruiseInfo;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }





    }
}
