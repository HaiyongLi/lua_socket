using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LuaInterface;
using System.Globalization;
using System.Diagnostics;


namespace Lua_test_CS
{
    public partial class Form1 : Form
    {
        public string strLuaFilePath;
        private static Process p;

        public Form1()
        {
            InitializeComponent();
        }

        private bool g_bIsConnectOk = false;
        Lua netlua = null;
        private void Form1_Load(object sender, EventArgs e)
        {
            SetDataGridViewData();
            //dataGridViewData.DefaultCellStyle.Font = new Font(dataGridViewData.DefaultCellStyle.Font, FontStyle.Bold);
            tbAdd.Text = "Add";
            tbMutiply.Text = "Mutiply";
            tbResult.Text = "Result";
            tbAverage.Text = "Average";

            btn_test.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;

        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            //Lua netlua = new Lua();
            //netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
            //netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
            //netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));
            //netlua.DoFile(strLuaFilePath);
            //SetData();
            // netlua.GetFunction("Add").Call();
            netlua.GetFunction(tbAdd.Text.Trim().ToString()).Call();
        }

        public void GetData(double a)
        {
            string str = a.ToString();
            MessageBox.Show(str);
        }

        public double SetData()
        {
            double a = 0;
            //if (tbInput.Text.Trim() != "")
            //{
            //    a = Convert.ToDouble(tbInput.Text.Trim());
            //}
            return a;
        }
        public double SetDataByIdx(int i)
        {
            if (i > dataGridViewData.Rows.Count)
            {
                MessageBox.Show("索引超出最大值！请重新输入！");
                return -1;
            }
            DataGridViewRow dgv = new DataGridViewRow();
            dgv = dataGridViewData.Rows[i];
            double a = Convert.ToDouble(dgv.Cells["工程值"].Value);
            return a;
        }

        DataTable dt = new DataTable();
        DataColumn column;
        DataRow row;
        public void SetDataGridViewData()
        {
            column = new DataColumn();
            column.AllowDBNull = true;
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "通道序号";
            dt.Columns.Add(column);

            // Create second column.
            column = new DataColumn();
            column.AllowDBNull = true;
            column.DataType = Type.GetType("System.String");   //string类型
            column.ColumnName = "工程值";
            dt.Columns.Add(column);

            column = new DataColumn();
            column.AllowDBNull = true;
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "通道说明";
            dt.Columns.Add(column);

            
            //for (int i = 0; i < 20; i++)
            //{
            //    string str = string.Format("第{0}个通道", i);
            //    row = dt.NewRow();
            //    row["通道序号"] = i;
            //    row["工程值"] = i;            //将double类型按照一定的格式转换成string类型
            //    row["通道说明"] = str;
            //    dt.Rows.Add(row);
            //}
             
            //MessageBox.Show(2.ToString("N", myNumFormatInfo));
            dataGridViewData.DataSource = dt;
             
        }

        private void SetChannelTableToGrid()
        {
            //dataGridViewData.DataSource = null;
            dt.Rows.Clear();
            int a = dt.Rows.Count;
            _SENDTAB tab;
            for (int i = 0; i < dataNums; i++)
            {
                string str = string.Format("第{0}个通道", i);
                row = dt.NewRow();
                row["通道序号"] = i;
                Double data = 2 * i;
                row["工程值"] = 0.0;            //将double类型按照一定的格式转换成string类型
                tab = (_SENDTAB)arraylist[i];
                row["通道说明"] = tab.ChName_CHN;
                dt.Rows.Add(row);
            }
            
            //MessageBox.Show(2.ToString("N", myNumFormatInfo));
            dataGridViewData.DataSource = dt;

            int n = dataGridViewData.Rows.Count;
            dataGridViewData.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (tbInput.Text == "")
            //{
            //    MessageBox.Show("请先输入通道索引！");
            //    return;
            //}
            //int i = Convert.ToInt32(tbInput.Text);
            int i = 0;
            if (i > dataGridViewData.Rows.Count)
            {
                MessageBox.Show("索引超出最大值20！请重新输入！");
                return;
            }
            DataGridViewRow dgv = new DataGridViewRow();
            dgv = dataGridViewData.Rows[i];
            double d = Convert.ToDouble(dgv.Cells["工程值"].Value);
            string s = Convert.ToString(dgv.Cells["通道说明"].Value);
            string str = string.Format("{0}的值是{1}", s, d);
            MessageBox.Show(str);
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            
            //SetData();
            //netlua.GetFunction("Mutiply").Call();
            netlua.GetFunction(tbMutiply.Text.Trim().ToString()).Call();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Lua netlua = new Lua();
            //netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
            //netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
            //netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));
            ////netlua.DoFile(@"E:\2-lua\Lua_test_CS\Lua_test_CS\add.lua");
            //netlua.DoFile(strLuaFilePath);
            //SetData();
            //netlua.GetFunction("Result").Call();
            netlua.GetFunction(tbResult.Text.Trim().ToString()).Call();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Lua netlua = new Lua();
            //netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
            //netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
            //netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));
            ////netlua.DoFile(@"E:\2-lua\Lua_test_CS\Lua_test_CS\add.lua");
            //netlua.DoFile(strLuaFilePath);
            //SetData();
            //netlua.GetFunction("Average").Call();
            netlua.GetFunction(tbAverage.Text.Trim().ToString()).Call();
        }

        private void btnLoadLuaFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            //openFileDialog1.InitialDirectory = "e:\\";
            openFileDialog1.Filter = "Lua files (*.lua)|*.lua";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                tbLuaFilePath.Text = openFileDialog1.FileName.ToString();
                strLuaFilePath = openFileDialog1.FileName.ToString();
                btn_test.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }

            netlua = new Lua();
            netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
            netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
            netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));
            //netlua.DoFile(@"E:\2-lua\Lua_test_CS\Lua_test_CS\add.lua");

            netlua.DoFile(strLuaFilePath);



        }

        private void button5_Click(object sender, EventArgs e)  //启动Lua编辑器
        {
            if (p == null)
            {
                p = new Process();
                p.StartInfo.FileName = @"F:\2-lua\luaeditoets\LuaEditor.exe";
                p.Start();
            }
            else
            { 
                if(p.HasExited)
                {
                    p.Start();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Lua netlua = new Lua();
            //netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
            //netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
            //netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));
            ////netlua.DoFile(@"E:\2-lua\Lua_test_CS\Lua_test_CS\add.lua");
            //netlua.DoFile(strLuaFilePath);
            //SetData();
            //netlua.GetFunction("Average").Call();
            netlua.GetFunction("mysum").Call();
        }

        private void btn_ConnSvc_Click(object sender, EventArgs e)
        {
            string strIp = tb_IP.Text.Trim();
            int nPort = Convert.ToInt32(tb_Port.Text.Trim());

            g_bIsConnectOk = Connect(strIp, nPort);

            if (g_bIsConnectOk)
            {
                MessageBox.Show("连接成功！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("连接失败！", "提示：", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }
    }
}
