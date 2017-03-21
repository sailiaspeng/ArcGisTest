using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;

namespace ArcGisTest
{
    public delegate void SendFun(string str);
    public partial class Property : Form
    {

        public Property()
        {
            InitializeComponent();
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            int Count = this.dataGridView1.RowCount - 1;
            IdentifyCount.Text = "识别到" + Count.ToString() + "条记录";
           
        }
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(this.dataGridView1.RowCount.ToString());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //获取点击行值
            int RowIndex = e.RowIndex;
            int ColomnIndex = e.ColumnIndex;
            string OID = null;
            if (e.RowIndex!=-1 && e.RowIndex!=this.dataGridView1.Rows.Count-1)
            {
                OID = dataGridView1.Rows[RowIndex].Cells[0].Value.ToString();
                MainForm Main = (MainForm)this.Owner;
                Main.display(OID);
            }

        
        }

    }
}
