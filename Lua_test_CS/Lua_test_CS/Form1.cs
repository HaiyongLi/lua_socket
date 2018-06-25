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
using System.Threading;


namespace Lua_test_CS
{
    public partial class Form1 : Form
    {
        public string strLuaFilePath;
        private static Process p;

        delegate void SetGridChannel();
        SetGridChannel setGridChannel;
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
            button4.Enabled = true;
            setGridChannel = SetChannelTableToGrid;

            LoadLuaFile(null);
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            netlua.GetFunction(tbAdd.Text.Trim().ToString()).Call();
        }

        public void GetData(int idx,double a)
        {
            string str = a.ToString();

            data[idx] = (float)a;
            //MessageBox.Show(a.ToString());
        }

        public double SetData(int idx)
        {
            double a = 0;
            if (idx>=0)
            {
                
            }
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

            column = new DataColumn();
            column.AllowDBNull = true;
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "函数名称";
            dt.Columns.Add(column);

            for (int i = 0; i < 20; i++)
            {
                string str = string.Format("第{0}个通道", i);
                row = dt.NewRow();
                row["通道序号"] = i;
                Double data = 2 * i;
                row["工程值"] = i+1;           
                row["通道说明"] = str;
                dt.Rows.Add(row);
            }

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
            string[] ss = { "Add", "Mutiply", "Result", "Average", "GetMax", "GetMin" };
            for (int i = 0; i < nJsNums; i++)
            {
                string str = string.Format("第{0}个通道", i);
                row = dt.NewRow();
                row["通道序号"] = i;
                Double data = 2 * i;
                row["工程值"] = 0.0;            //将double类型按照一定的格式转换成string类型
                row["函数名称"] = ss[i];
                dt.Rows.Add(row);
            }

            dataGridViewData.DataSource = dt;
            dataGridViewData.Refresh();

            //int n = dataGridViewData.Rows.Count;
            //dataGridViewData.Refresh();
            //MessageBox.Show("接收通道表完毕！");
            //timer1.Enabled = true;
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
            netlua.GetFunction(tbResult.Text.Trim().ToString()).Call();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            netlua.GetFunction(tbAverage.Text.Trim().ToString()).Call();
        }

        //加载lua文件
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

            LoadLuaFile(strLuaFilePath);
        }

        private void LoadLuaFile(string strPath)
        {
            if (strPath == null || strPath == "")
            {
                netlua = new Lua();
                netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
                netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
                netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));
                netlua.DoFile(@"F:\2-lua\Lua_test_CS\Lua_test_CS\add.lua");

                //netlua.DoFile(strLuaFilePath);
                //timer1.Enabled = true;
            }
            else
            {
                netlua = new Lua();
                netlua.RegisterFunction("SetData", this, this.GetType().GetMethod("SetData"));
                netlua.RegisterFunction("GetData", this, this.GetType().GetMethod("GetData"));
                netlua.RegisterFunction("SetDataByIdx", this, this.GetType().GetMethod("SetDataByIdx"));

                netlua.DoFile(strPath);
                timer1.Enabled = true;
            }
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
            //netlua.GetFunction("mysum").Call();
            object[] obj = netlua.GetFunction("math.min").Call(33, 55);

            object[] obj1 = netlua.GetFunction("math.max").Call(33,55,66,44,77);
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
        const int nJsNums = 6;
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < dataNums + nJsNums; i++)
            {
                dataGridViewData.Rows[i].Cells[1].Value = data[i].ToString("F");
            }

            netlua.GetFunction("Add").Call();   
            netlua.GetFunction("Mutiply").Call();
            netlua.GetFunction("Result").Call();
            netlua.GetFunction("Average").Call();
            netlua.GetFunction("GetMax").Call();
            netlua.GetFunction("GetMin").Call();
            //SetDataByIdx(1000);
            dataGridViewData.Refresh();
        }
    }
}
