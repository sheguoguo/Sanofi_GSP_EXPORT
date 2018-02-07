using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Sanofi_GSP_EXPORT
{
    public partial class excelform : Form
    {
        public excelform()
        {
            InitializeComponent();
        }

        public void listfile(string foldername)
        {

            DirectoryInfo theFolder = new DirectoryInfo(foldername);
            DirectoryInfo[] dirInfo = theFolder.GetDirectories();
            //遍历文件夹
            FileInfo[] fileInfo = theFolder.GetFiles();

            foreach (FileInfo NextFile in fileInfo)  //遍历文件
            {
                if ((theFolder.Name.Contains("药品温度") || theFolder.Name.Contains("冷包温度"))&&(NextFile.Extension==".xls"))
                {
                    int row_index = this.dataGridView1.Rows.Add();
                    dataGridView1.Rows[row_index].Cells[0].Value = NextFile.Name;
                    dataGridView1.Rows[row_index].Cells[1].Value = NextFile.DirectoryName;
                    dataGridView1.Rows[row_index].Cells[2].Value = NextFile.FullName;
                }
            }
            foreach (DirectoryInfo NextFolder in dirInfo)
            {
                // this.listBox1.Items.Add(NextFolder.Name);
                
                
                listfile(NextFolder.FullName);

            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            listfile(folderBrowserDialog1.SelectedPath);
            toolStripStatusLabel2.Text = dataGridView1.Rows.Count.ToString();
            textBox1.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count != 0)
            {
                toolStripProgressBar1.Maximum = dataGridView1.Rows.Count-1;
                for (int row_count = 0; row_count < dataGridView1.Rows.Count; row_count++)
                {
                    List<DataTable> excel_ls = ExcelHepler.GetDataTablesFrom(dataGridView1.Rows[row_count].Cells[2].Value.ToString());
                    DataRow[] dr_arry = excel_ls[0].Select("警报='1'");
                    if (dr_arry.Length != 0)
                    {
                        dataGridView1.Rows[row_count].DefaultCellStyle.BackColor = Color.Red;
                        textBox1.AppendText(dataGridView1.Rows[row_count].Cells[0].Value.ToString()+Environment.NewLine );
                    }

                    toolStripProgressBar1.Value = row_count;
                    Application.DoEvents();
                    toolStripStatusLabel4.Text = row_count.ToString();
                    
                }
            }
        }
    }
}
