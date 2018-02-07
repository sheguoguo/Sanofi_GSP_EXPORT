using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Oracle.DataAccess.Client;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;


namespace Sanofi_GSP_EXPORT
{
    public partial class compare_form : Form
    {
        public compare_form()
        {
            InitializeComponent();

            
        }

        private string shp_mnt_nbr="";
        
        DataTable gsp_local_dt = null;
        //string gsp_key = "4C10B363C125B6B0AF4DACADDC493E82";
        public string cookie_jseesion = "";
        public string cookie_serverid = "";
        //string test_str = "{'chkInOutRequest':{'entSeqNo':'178ef21c96034398b06a90fefc68ef42','beginDate':'20170801','endDate':'20180130','partnerIdSend':'','partnerIdRecv':'','dataType':'A','entStoreInoutId':'WSASNP1709080287428','physicInfo':'','pkgSpec':'','produceBatchNo':'','drugType':'A','processFlag':'3','partnerIdRecvName':'','partnerIdSendName':'','corpSeqNo':'320000000000141855','agentEntId':'','agentEntName':'','isContainAgentBill':'否'},'pageResquest':{'curPage':1,'pageSize':20}}";

        public static string StrToJsonstr(string theString)
        {
            theString = theString.Replace(">", "&gt;");
            theString = theString.Replace("<", "&lt;");
            theString = theString.Replace(" ", "&nbsp;");
            theString = theString.Replace("\"", "&quot;");
            theString = theString.Replace("\'", "&#39;");
            theString = theString.Replace("\\", "\\\\");//对斜线的转义  
            theString = theString.Replace("\n", "\\n");  //注意php中替换的时候只能用双引号"\n"
            theString = theString.Replace("\r", "\\r");
            return theString;
        }

        /// <summary>
        /// httpPost请求--参数为object
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postObject">Post参数传输为对象</param>
        /// <returns></returns>
        public static string HttpPost(string url, object postObject, string JSESSIONID = "", string SERVERID = "",string ServiceName="")
        {
            string result = string.Empty;

            try
            {
                Uri target_url=new Uri(url);
                var request = (HttpWebRequest)WebRequest.Create(target_url);
                //string jsonstr=StrToJsonstr(postObject.ToString());
                var postData = JsonConvert.SerializeObject(postObject);

                var data = Encoding.UTF8.GetBytes(postObject.ToString());  //uft-8支持中文
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentType = "application/json;charset=UTF-8";
                request.ContentLength = data.Length;
                request.Headers.Add("ServiceName", ServiceName);
                
                //request.Host = "traceentservice.mashangfangxin.com";
                

                //这里使用了coolie容器，用来模拟向服务器发送cookie信息
                CookieContainer zl_Cookie = new CookieContainer();
                zl_Cookie.Add(new Cookie("JSESSIONID", JSESSIONID) {Domain=target_url.Host });
                zl_Cookie.Add(new Cookie("SERVERID", SERVERID) { Domain = target_url.Host });
                request.CookieContainer = zl_Cookie;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            
            return result;
        }
        /// <summary>
        /// httpPost请求--参数为string
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="postString">post参数参数为字符串</param>
        /// <returns></returns>
        public static string HttpPost(string url, string postString)
        {
            string result = string.Empty;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                var data = Encoding.UTF8.GetBytes(postString);  //uft-8支持中文
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                request.ContentType = "application/json;charset=UTF-8";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                result = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                result = ex.Message;
                
            }

            return result;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            ////DataTable owner_dt = SPPHelper.SPP_list_owner("FCA75D6E3C7F53635B8EE46B3E641A52");
            //string order_nbr = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            //string owner_name = dataGridView1.CurrentRow.Cells[1].Value.ToString();

            //DataTable order_dt = SPPHelper.SPP_list_order(gsp_key, order_nbr, SPPHelper.owner_name_to_nbr(owner_name));
            //if (order_dt != null)
            //{
            //    dataGridView1.DataSource = order_dt;
                
            //}
            //else
            //{ 
            //    MessageBox.Show("药监网上未有此单信息，请确认是否已上传"); 
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (OracleConnection conn = oraclehelper.GetOracleConnectionAndOpen)
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        string sql_text = "select ah.shpmt_nbr as 入库单号,substr(ah.shpmt_nbr,1,2) as 货主,ah.create_date_time as 创建时间,' ' as 是否上传, ' ' as 药监网编号,' ' as 监管码校验情况  from asn_hdr ah where ah.to_whse='S00' and ah.stat_code=90 and to_char(ah.create_date_time,'yyyymmdd') >=" + dateTimePicker1.Value.ToString("yyyyMMdd") + " and to_char(ah.create_date_time,'yyyymmdd')<=" + dateTimePicker2.Value.ToString("yyyyMMdd");
                        if (textBox1.Text != "")
                        {
                            sql_text = sql_text + " and substr(ah.shpmt_nbr,1,2)='" + textBox1.Text + "'";
                        }
                        if (textBox2.Text != "")
                        {
                            sql_text = sql_text + " and ah.shpmt_nbr='" + textBox2.Text + "'";
                        }
                        dataGridView1.DataSource = oraclehelper.ExecuteDataTable(sql_text);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("出现异常, 异常信息: " + ex.Message);
            } 
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        

        private void button4_Click(object sender, EventArgs e)
        {
            //string asn_order = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            //string asn_nbr = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            //shp_mnt_nbr = dataGridView1.CurrentRow.Cells[2].Value.ToString();

            //DataTable order_dt = SPPHelper.SPP_list_gsp_nbr(gsp_key, asn_nbr, asn_order);
            //if (order_dt != null)
            //{
            //    dataGridView1.DataSource = order_dt;
            //    gsp_online_dt = order_dt;

            //}
            //else
            //{
            //    MessageBox.Show("药监网上未有此单信息，请确认是否已上传");
            //}
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            //DataTable gsp_local_dt = null;
            //try
            //{
            //    using (OracleConnection conn = oraclehelper.GetOracleConnectionAndOpen)
            //    {
            //        if (conn.State == ConnectionState.Open)
            //        {
            //            string sql_text = "";
            //            if (shp_mnt_nbr != "")
            //            {
            //                sql_text = "select c.batch_nbr as batch_nbr,c.gsp_nbr as gsp_nbr from c_gsp_nbr_trkg c where c.stat_code=0 and c.rcvd_shpmt_nbr='"+shp_mnt_nbr+"'";
            //            }
            //            dataGridView1.DataSource = null;
            //            gsp_local_dt = oraclehelper.ExecuteDataTable(sql_text);
                        
            //        }
            //    }
            //    IEnumerable<DataRow> query = gsp_online_dt.AsEnumerable().Except(gsp_local_dt.AsEnumerable(), DataRowComparer.Default);
            //    if (query.Count() != 0)
            //        dataGridView1.DataSource = query.CopyToDataTable();
                    
            //    else
            //        MessageBox.Show("不存在差异");
            //}
                

            //catch (Exception ex)
            //{
            //    MessageBox.Show("出现异常, 异常信息: " + ex.Message);
            //} 
            

        }

        private void button6_Click(object sender, EventArgs e)
        {
            
            Dictionary<string, string> season = new Dictionary<string, string>();
            season.Add("WS", "178ef21c96034398b06a90fefc68ef42");
            season.Add("BS", "320000000000002503");
            season.Add("SW", "74286cc8857b4ea29a81a28b4d90416a");
            string order_nbr = "";
            string owner_name = "";
            DataTable order_dt = null;
            int count_fail = 0;

            if (dataGridView1.SelectedRows.Count!=0)
            {
                foreach (DataGridViewRow order_dgvr in dataGridView1.SelectedRows)
                {
                    try
                    {
                        //order_dt = SPPHelper.SPP_list_order(gsp_key, order_dgvr.Cells["入库单号"].Value.ToString(), SPPHelper.owner_name_to_nbr(order_dgvr.Cells["货主"].Value.ToString()));
                        string begin_date = dateTimePicker1.Value.ToString("yyyyMMdd");
                        begin_date = "";
                        string end_date = dateTimePicker2.Value.ToString("yyyyMMdd");
                        end_date = "";
                        string post_json = "{'chkInOutRequest':{'entSeqNo':'" + season[order_dgvr.Cells["货主"].Value.ToString()] + "','beginDate':'" + begin_date + "','endDate':'" + end_date + "','partnerIdSend':'','partnerIdRecv':'','dataType':'A','entStoreInoutId':'" + order_dgvr.Cells["入库单号"].Value.ToString() + "','physicInfo':'','pkgSpec':'','produceBatchNo':'','drugType':'A','processFlag':'3','partnerIdRecvName':'','partnerIdSendName':'','corpSeqNo':'320000000000141855','agentEntId':'','agentEntName':'','isContainAgentBill':'否'},'pageResquest':{'curPage':1,'pageSize':20}}";
                        //提交查询
                        string json_str = HttpPost("http://traceentservice.mashangfangxin.com/requestEntrance", post_json, cookie_jseesion, cookie_serverid,"piats.superpass.bill.entQueryChkInOutListsService");
                        //List<Newtonsoft.Json.Converters.JObject> test_list = JsonConvert.DeserializeObject<List<Array>>(json_str);
                        JObject jo_result = JObject.Parse(json_str);
                        if (jo_result["retMessage"].ToString() == "调用成功")
                        {
                            order_dgvr.Cells["是否上传"].Value = "已上传";
                            
                            JArray ja_result=JArray.Parse(jo_result["retObj"]["chkInOutLists"]["chkInOutResponse"].ToString());
                            order_dgvr.Cells["药监网编号"].Value = ja_result[0]["storeInOutSeqNo"].ToString();
                            dataGridView1.Refresh();
                        }
                        else
                        {
                            order_dgvr.Cells["是否上传"].Value = "未上传";
                            order_dgvr.DefaultCellStyle.ForeColor = Color.Red;
                            dataGridView1.Refresh();
                            count_fail++;
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("药监网登录超时，请退出后重新登录！");
                        return;
                    }
                   
                   

                }
                if (count_fail == 0) MessageBox.Show("上传查询已完成");
                else
                MessageBox.Show("上传查询已完成,共有"+count_fail.ToString()+"张订单未上传,请注意!");
            }

            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ////this.UseWaitCursor = true;
            this.Cursor = Cursors.WaitCursor;//等待
            DataTable gsp_online_dt = new DataTable();
            gsp_online_dt.Columns.Add("batch_nbr", typeof(String));
            gsp_online_dt.Columns.Add("gsp_nbr", typeof(String));
            if (dataGridView1.Rows.Count != 0)
            {
                toolStripProgressBar1.Value = 0;
                toolStripProgressBar1.Maximum = dataGridView1.Rows.Count;

                foreach (DataGridViewRow order_dgvr in dataGridView1.SelectedRows)
                {
                    if (order_dgvr.Cells["是否上传"].Value.ToString() == "已上传" && order_dgvr.Cells["药监网编号"].Value.ToString() != "")
                    {


                        try
                        {
                            
                            //获取药监网监管码批号表
                            string post_json = "{'storeInoutSeqNo':'" + order_dgvr.Cells["药监网编号"].Value.ToString() + "','entStoreInId':'320000000000141855','isFlag':'1','billType':'102'}";
                            string json_str = HttpPost("http://traceentservice.mashangfangxin.com/requestEntrance", post_json, cookie_jseesion, cookie_serverid, "piats.superpass.bill.entQueryChkInInfoService");
                            JObject jo_result = JObject.Parse(json_str);
                            if (jo_result["retMessage"].ToString() == "调用成功")
                            {
                                JArray ja_result=JArray.Parse(jo_result["retObj"]["chkInPhysicInfos"]["chkInPhysicInfo"].ToString());
                                foreach (JObject tmp_jo in ja_result)
                                {
                                    string tmp_batch_nbr = tmp_jo["produceBatchNo"].ToString();
                                    JArray ja_gsp_nbr=JArray.Parse(tmp_jo["codLists"].ToString());
                                    foreach (JObject jo_gsp_nbr in ja_gsp_nbr)
                                    {
                                        gsp_online_dt.Rows.Add(tmp_batch_nbr,jo_gsp_nbr["code"].ToString());
                                    }
                                }

                            }


                            //查询数据库监管码批号表
                            using (OracleConnection conn = oraclehelper.GetOracleConnectionAndOpen)
                            {
                                if (conn.State == ConnectionState.Open)
                                {
                                    string sql_text = "select distinct c.batch_nbr as batch_nbr,c.gsp_nbr as gsp_nbr from c_gsp_nbr_trkg c where c.stat_code<90 and c.rcvd_shpmt_nbr='" + order_dgvr.Cells["入库单号"].Value.ToString() + "'";


                                    gsp_local_dt = oraclehelper.ExecuteDataTable(sql_text);

                                }
                            }
                            IEnumerable<DataRow> query = gsp_online_dt.AsEnumerable().Except(gsp_local_dt.AsEnumerable(), DataRowComparer.Default);
                            if (query.Count() != 0)
                            {
                                order_dgvr.Cells["监管码校验情况"].Value = "共" + query.Count().ToString() + "条药监码批号校验有误";
                                order_dgvr.DefaultCellStyle.BackColor = Color.Red;
                                dataGridView1.Refresh();

                            }

                            else
                            {
                                order_dgvr.Cells["监管码校验情况"].Value = "共校验"+gsp_local_dt.Rows.Count.ToString()+"条药监码,全部匹配无误!";
                                order_dgvr.DefaultCellStyle.BackColor = Color.Green;
                                dataGridView1.Refresh();
                            }
                            gsp_online_dt.Rows.Clear();
                        }


                        catch (Exception ex)
                        {
                            MessageBox.Show("出现异常, 异常信息: " + ex.Message);
                        }

                    }
                    else
                    {
                        MessageBox.Show("请先查询上传情况再进行校验");
                        return;
                    }

                    toolStripProgressBar1.Value += 1;
                }

            }
            //this.UseWaitCursor = false;
            gsp_online_dt = null;
            this.Cursor = Cursors.Default;//正常状态
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;//等待
            DataTable gsp_online_dt = new DataTable();
            gsp_online_dt.Columns.Add("batch_nbr", typeof(String));
            gsp_online_dt.Columns.Add("gsp_nbr", typeof(String));
            if (dataGridView2.DataSource != null)
            {
                DataTable dt = (DataTable)dataGridView2.DataSource;
                dt.Rows.Clear();
                dataGridView2.DataSource = dt;
            }
            if (dataGridView1.Rows.Count != 0)
            {
                if (dataGridView1.CurrentRow.Cells["监管码校验情况"].Value.ToString() != "")
                {

                    try
                    {
                        //获取药监网监管码批号表
                        //gsp_online_dt = SPPHelper.SPP_list_gsp_nbr(gsp_key, dataGridView1.CurrentRow.Cells["入库单号"].Value.ToString(), dataGridView1.CurrentRow.Cells["药监网编号"].Value.ToString());
                        string post_json = "{'storeInoutSeqNo':'" + dataGridView1.CurrentRow.Cells["药监网编号"].Value.ToString() + "','entStoreInId':'320000000000141855','isFlag':'1','billType':'102'}";
                        string json_str = HttpPost("http://traceentservice.mashangfangxin.com/requestEntrance", post_json, cookie_jseesion, cookie_serverid, "piats.superpass.bill.entQueryChkInInfoService");
                        JObject jo_result = JObject.Parse(json_str);
                        if (jo_result["retMessage"].ToString() == "调用成功")
                        {
                            JArray ja_result = JArray.Parse(jo_result["retObj"]["chkInPhysicInfos"]["chkInPhysicInfo"].ToString());
                            foreach (JObject tmp_jo in ja_result)
                            {
                                string tmp_batch_nbr = tmp_jo["produceBatchNo"].ToString();
                                JArray ja_gsp_nbr = JArray.Parse(tmp_jo["codLists"].ToString());
                                foreach (JObject jo_gsp_nbr in ja_gsp_nbr)
                                {
                                    gsp_online_dt.Rows.Add(tmp_batch_nbr, jo_gsp_nbr["code"].ToString());
                                }
                            }

                        }

                        //查询数据库监管码批号表
                        using (OracleConnection conn = oraclehelper.GetOracleConnectionAndOpen)
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                string sql_text = "select c.batch_nbr as batch_nbr,c.gsp_nbr as gsp_nbr from c_gsp_nbr_trkg c where c.stat_code<90 and c.rcvd_shpmt_nbr='" + dataGridView1.CurrentRow.Cells["入库单号"].Value.ToString() + "'";


                                gsp_local_dt = oraclehelper.ExecuteDataTable(sql_text);

                            }

                        }
                        IEnumerable<DataRow> query = gsp_online_dt.AsEnumerable().Except(gsp_local_dt.AsEnumerable(), DataRowComparer.Default);
                        if (query.Count() != 0)
                        {
                            dataGridView2.DataSource = query.CopyToDataTable();

                            using (OracleConnection conn = oraclehelper.GetOracleConnectionAndOpen)
                            {
                                if (conn.State == ConnectionState.Open)
                                {
                                    foreach (DataGridViewRow temp_dgvr in dataGridView2.Rows)
                                    {
                                        string sql_text = "select ch.case_nbr,lh.locn_brcd locn_nbr from c_gsp_nbr_trkg c left join case_hdr ch on ch.case_nbr=c.cntr_nbr left join locn_hdr lh on lh.locn_id=ch.locn_id and lh.whse='S00' where c.gsp_nbr='" + temp_dgvr.Cells["监管码"].ToString() + "'";
                                        DataTable temp_dt = oraclehelper.ExecuteDataTable(sql_text);
                                        if (temp_dt.Rows.Count != 0)
                                        {
                                            temp_dgvr.Cells["所在货箱号"].Value = temp_dt.Rows[0][0].ToString();
                                            temp_dgvr.Cells["所在库位号"].Value = temp_dt.Rows[0][1].ToString();
                                            dataGridView2.Refresh();
                                        }
                                    }

                                }

                            }


                        }

                        else
                        {
                            MessageBox.Show("无异常信息");

                        }
                    }


                    catch (Exception ex)
                    {
                        MessageBox.Show("出现异常, 异常信息: " + ex.Message);
                    }
                }


                else
                {
                    MessageBox.Show("请先进行该单据的批号校验，谢谢！");
                    return;
                }
            }
            gsp_online_dt = null;
            this.Cursor = Cursors.Default;//正常状态
        }

        private void compare_form_Load(object sender, EventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel3.Text = DateTime.Now.ToString();
        } 


        
    }
}
