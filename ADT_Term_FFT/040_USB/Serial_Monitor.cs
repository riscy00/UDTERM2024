using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace UDT_Term_FFT
{
    public partial class Serial_Monitor : Form
    {

        GlobalBase myGlobalBase;

        public void MyGlobalBase(GlobalBase myGlobalBaseRef)
        {
            myGlobalBase = myGlobalBaseRef;

        }

        private Int32 m_Item;

        private System.Windows.Forms.DataGridViewTextBoxColumn[] CANColumn;
        private System.Windows.Forms.ColumnHeader[] CANColumnTop;

        //=====================================================Getter/Setter
        public int Item { get { return m_Item; } set { m_Item = value; } }

        //=====================================================Constructor
        public Serial_Monitor()
        {
            InitializeComponent();
            OutputView.DoubleBuffered2(true);
            #region //=========================================DataGridView Column Definition
            int i;
            //--------------------------------------------------------------Add Title Header
            CANColumnTop = new System.Windows.Forms.ColumnHeader[6];
            for (i = 0; i < myCANColumnFormatTop.Length; i++)
            {
                this.CANColumnTop[i] = new System.Windows.Forms.ColumnHeader();
                this.CANColumnTop[i].Width = myCANColumnFormatTop[i].Width;
                this.CANColumnTop[i].Text = myCANColumnFormatTop[i].Label;
                this.OutputViewTop.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.CANColumnTop[i] });
            }
            //--------------------------------------------------------------Add Response/Request Column
            CANColumn = new System.Windows.Forms.DataGridViewTextBoxColumn[myCANColumnFormat.Length];
            int widthtotal = 0;
            for (i = 0; i < myCANColumnFormat.Length; i++)
            {
                this.CANColumn[i] = new System.Windows.Forms.DataGridViewTextBoxColumn();
                this.CANColumn[i].HeaderText = myCANColumnFormat[i].Label;
                this.CANColumn[i].Width = myCANColumnFormat[i].Width;
                widthtotal += myCANColumnFormat[i].Width;

                this.OutputView.Columns.AddRange(new System.Windows.Forms.DataGridViewTextBoxColumn[] { this.CANColumn[i] });
            }
            //--------------------------------------------------------------optimize window size
            this.Width = widthtotal + 20;
            this.OutputView.Width = widthtotal + 20;
            this.MaximumSize = new System.Drawing.Size(widthtotal + 40, 396);
            this.MinimumSize = new System.Drawing.Size(widthtotal + 40, 396);
            #endregion

            m_Item = 0;
        }

        #region//==================================================struct CanStruct
        struct CanStruct
        {
            private readonly string label;
            private readonly int width;

            public CanStruct(string label, int width)
            {
                this.label = label;
                this.width = width;
            }

            public string Label { get { return label; } }
            public int Width { get { return width; } }

        }
        #endregion

        #region//==================================================Setup Column Format
        static CanStruct[] myCANColumnFormat =
            new CanStruct[]{
                new CanStruct("Item",50),
                new CanStruct("Date",50),
                new CanStruct("Time",50),
                new CanStruct("",5),
                new CanStruct("ID",30),
                new CanStruct("D0",30),
                new CanStruct("D1",30),
                new CanStruct("D2",30),
                new CanStruct("D3",30),
                new CanStruct("D4",30),
                new CanStruct("D5",30),
                new CanStruct("D6",30),
                new CanStruct("D7",30),
                new CanStruct(" ",5),                   // Position 13
                new CanStruct("DC",30),
                new CanStruct("INDEX",40),
                new CanStruct("SI",30),
                new CanStruct("NOTE / PARA",100),       // Position array 17 (end of request)
                new CanStruct(" ",5),
                new CanStruct("ID",30),
                new CanStruct("D0",30),
                new CanStruct("D1",30),
                new CanStruct("D2",30),
                new CanStruct("D3",30),
                new CanStruct("D4",30),
                new CanStruct("D5",30),
                new CanStruct("D6",30),
                new CanStruct("D7",30),
                new CanStruct(" ",5),                   // Position 28
                new CanStruct("DC",30),
                new CanStruct("INDEX",40),
                new CanStruct("SI",30),
                new CanStruct("NOTE / PARA",100),       // Position 32.
            };
        static CanStruct[] myCANColumnFormatTop =
           new CanStruct[]{
                new CanStruct("",50+50+50+5),
                new CanStruct("CAN REQUEST SECTION (CLIENT)",30+(8*30)+5),
                new CanStruct("SDO DETAILS",30+40+30+100),
                new CanStruct("CAN RESPONSE SECTION (SERVER)",5+30+(8*30)+5),
                new CanStruct("SDO DETAILS",30+40+30+100),
                new CanStruct("",80)
            };
        #endregion

        #region//==================================================clearScreenToolStripMenuItem_Click
        private void clearScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OutputView.Rows.Clear();
            Item = 0;
        }
        #endregion

        #region//==================================================hideViewToolStripMenuItem_Click
        private void hideViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();            // Use hide so it keep on collecting data in case it need to be visible.
        }
        #endregion

        #region//==================================================saveToolStripMenuItem_Click
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd_SaveLinData = new SaveFileDialog();
            sfd_SaveLinData.Filter = "csv files (*.csv)|*.csv";
            sfd_SaveLinData.FileName = "logs";
            sfd_SaveLinData.Title = "Export to Excel";


            StringBuilder sb = new StringBuilder();

            for (int ii = 0; ii < myCANColumnFormat.Length; ii++)
            {
                sb.Append(OutputView.Columns[ii].HeaderText + ',');
            }
            sb.AppendLine();

            foreach (DataGridViewRow row in OutputView.Rows)
            {
                for (int ii = 0; ii < myCANColumnFormat.Length; ii++)
                {
                    if (row.Cells[ii].Value == null)
                    {
                        sb.Append(" ,");
                    }
                    else
                    {
                        sb.Append(row.Cells[ii].Value.ToString() + ",");
                    }
                        
                }
                sb.AppendLine();
            }
            DialogResult dr = sfd_SaveLinData.ShowDialog();
            if (dr == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(sfd_SaveLinData.FileName);
                sw.Write(sb.ToString());
                sw.Close();
            }
        }
        #endregion

        #region//==================================================Serial_Monitor_FormClosing
        private void Serial_Monitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            this.Hide();
        }
        #endregion

        #region//==================================================btn_CloseBox_Click
        private void btn_CloseBox_Click(object sender, EventArgs e) // Close form
        {
            this.Hide();
        }
        #endregion



    }
    #region//=============================================================DoubleBuffered property via reflection for datagridview, improve flickerless image.
    public static class ExtensionMethods2
    {
        public static void DoubleBuffered2(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    #endregion
}
