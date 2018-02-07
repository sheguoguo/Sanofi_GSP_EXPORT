using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Oracle.DataAccess.Client;

namespace Sanofi_GSP_EXPORT
{
    
    public partial class relations_import : Form
    {
        public relations_import()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            openFileDialog1.ShowDialog();
            
            XmlHelper gsp_xml = new XmlHelper(openFileDialog1.FileName);
            dataGridView1.DataSource = gsp_xml.GetNodesData("Document/Events/Event/Relation/Batch");

            XmlNode relation_node=gsp_xml.GetSingleNode("Document/Events/Event/Relation");
            textBox1productCode.Text = relation_node.Attributes[0].Value.ToString();
            textBox2subTypeNo.Text = relation_node.Attributes[1].Value.ToString();
            textBox3cascade.Text = relation_node.Attributes[2].Value.ToString();
            textBox4packageSpec.Text = relation_node.Attributes[3].Value.ToString();
            textBox5comment.Text = relation_node.Attributes[4].Value.ToString();

            XmlNode Batch_node = gsp_xml.GetSingleNode("Document/Events/Event/Relation/Batch");
            textBox7batchNo.Text = Batch_node.Attributes[0].Value.ToString();
            textBox8madeDate.Text = Batch_node.Attributes[1].Value.ToString();
            textBox9validateDate.Text = Batch_node.Attributes[2].Value.ToString();
            textBox10workShop.Text = Batch_node.Attributes[3].Value.ToString();
            textBox11lineName.Text = Batch_node.Attributes[4].Value.ToString();
            textBox12lineManager.Text = Batch_node.Attributes[5].Value.ToString();

           //XmlDocument xmlDoc = new XmlDocument();
           // XmlReaderSettings settings = new XmlReaderSettings();
           // settings.IgnoreComments = true;//忽略文档里面的注释
           // XmlReader reader = XmlReader.Create(openFileDialog1.FileName, settings);
           // xmlDoc.Load(reader);

            //OracleCommand oracommand = new OracleCommand();

            //string code_insert_sql_text = "insert into s00_sa_relation_dtl values(:batchno,:currcode,:packLayer,:parentCode,:flag)";

            
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int result_count = 0;
            if (dataGridView1.Rows.Count > 0)
            {
                string batch_query_sql_text = "select count(*) from s00_sa_relation_hdr where batchno='" + textBox7batchNo.Text + "' and linename='"+textBox11lineName.Text+"'";
                result_count = int.Parse(oraclehelper.ExecuteScalar(batch_query_sql_text).ToString());
                if (result_count == 0)
                {
                    string code_insert_sql_text = "insert into s00_sa_relation_hdr(batchno,productcode,subtypeno,cascade_txt,packagespec,comment_txt,madedate,validatedate,workshop,linename,linemanager) values('" + textBox7batchNo.Text + "','" + textBox1productCode.Text + "','" + textBox2subTypeNo.Text + "','" + textBox3cascade.Text + "','" + textBox4packageSpec.Text + "','" + textBox5comment.Text + "','" + textBox8madeDate.Text + "','" + textBox9validateDate.Text + "','" + textBox10workShop.Text + "','" + textBox11lineName.Text + "','" + textBox12lineManager.Text + "')";
                    result_count = oraclehelper.ExecuteNonQuery(code_insert_sql_text);
                }
                string dtl_query_sql_text = "select count(*) from s00_sa_relation_dtl where curcode='" + dataGridView1.Rows[0].Cells[0].Value.ToString() + "'";
                result_count = int.Parse(oraclehelper.ExecuteScalar(dtl_query_sql_text).ToString());
                if (result_count != 0)
                {
                    MessageBox.Show("所要导入的监管码已经存在，请核实！ ");
                    return;
                }

                Dictionary<string, object> datas = new Dictionary<string, object>();
                string[] batchno_array = new string[dataGridView1.Rows.Count];
                string[] curCode_array = new string[dataGridView1.Rows.Count];
                string[] packLayer_array = new string[dataGridView1.Rows.Count];
                string[] parentCode_array = new string[dataGridView1.Rows.Count];
                string[] flag_array = new string[dataGridView1.Rows.Count];

                for(int i=0;i<dataGridView1.Rows.Count;i++)
                {
                    
                    batchno_array[i]= textBox7batchNo.Text;
                    curCode_array[i]=dataGridView1.Rows[i].Cells[0].Value.ToString();
                    packLayer_array[i]=dataGridView1.Rows[i].Cells[1].Value.ToString();
                    parentCode_array[i]=dataGridView1.Rows[i].Cells[3].Value.ToString();
                    flag_array[i]=dataGridView1.Rows[i].Cells[2].Value.ToString();
                    

                }

                datas.Add("batchNo", batchno_array);
                datas.Add("curCode", curCode_array);
                datas.Add("packLayer", packLayer_array);
                datas.Add("parentCode", parentCode_array);
                datas.Add("flag", flag_array);
                int result = oraclehelper.BatchInsert("s00_sa_relation_dtl", datas, curCode_array.Length);
                MessageBox.Show("已经导入监管码："+result.ToString()+"条");
            }
        }
    }
}
