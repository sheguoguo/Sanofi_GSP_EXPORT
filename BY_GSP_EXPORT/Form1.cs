using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;


namespace Sanofi_GSP_EXPORT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        DataTable batch_dt = new DataTable();

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btn_query_Click(object sender, EventArgs e)
        {
            string asn_query_txt = "";
            string pkt_query_txt = "";
            if (radioButton1.Checked)//入库单
            {
                using (OracleConnection oraconn = oraclehelper.GetOracleConnectionAndOpen)
                {
                    if (oraconn.State == ConnectionState.Open)
                    {
                        asn_query_txt = @"select ah.shpmt_nbr asn号,ah.manif_nbr 订单号,im.sku_desc 品规,ah.units_shpd 到货数量,count(c.gsp_nbr) 扫码数量,vm.vendor_name 供货单位,ah.create_date_time 下单日期  from asn_hdr ah 
                                        left join asn_dtl ad on ad.shpmt_nbr=ah.shpmt_nbr
                                        left join item_master im on im.sku_id=ad.sku_id
                                         left join vendor_master vm on vm.vendor_master_id=ad.vendor_master_id
                                        left join c_gsp_nbr_trkg c on c.rcvd_shpmt_nbr=ah.shpmt_nbr and c.stat_code<90
                                        where ah.to_whse='S00' and substr(ah.shpmt_nbr,1,2)='" + textBox2.Text + "' and to_char(ah.create_date_time,'yyyymmdd')>='" + dateTimePicker1.Text + "' and to_char(ah.create_date_time,'yyyymmdd')<='" + dateTimePicker2.Text + "' group by ah.shpmt_nbr,ah.manif_nbr,im.sku_desc,ah.units_shpd,vm.vendor_name,ah.create_date_time";
                        DataTable asn_tb = oraclehelper.ExecuteDataTable(asn_query_txt);
                        if (asn_tb.Rows.Count == 0)
                        {
                            MessageBox.Show("未查询到入库单");
                        }
                        dg_order.DataSource = asn_tb;
                        dg_order.Refresh();
                        stats_lb_rowcount.Text ="订单数量："+ dg_order.RowCount.ToString();
                    }
                }
            }

            if (radioButton2.Checked)//出库单
            {
                using (OracleConnection oraconn = oraclehelper.GetOracleConnectionAndOpen)
                {
                    if (oraconn.State == ConnectionState.Open)
                    {
                        pkt_query_txt = @"select ph.pkt_ctrl_nbr 拣货单号,ph.pkt_nbr 订单号,ph.shipto_name 收货单位,phi.total_nbr_of_units 总数量,ph.create_date_time 下单时间 ,phi.ship_wave_nbr 波次号
                                        from pkt_hdr ph left join pkt_hdr_intrnl phi on phi.pkt_ctrl_nbr=ph.pkt_ctrl_nbr 
                                        where ph.whse='S00' and  ph.create_date_time>=to_date('" + dateTimePicker1.Text + "','yyyymmdd') and ph.create_date_time<=to_date('" + dateTimePicker2.Text + "','yyyymmdd') and substr(ph.pkt_ctrl_nbr,1,2)='" + textBox2.Text + "' and phi.stat_code>=40 ";
                        if (textBox1.Text != "")
                        { pkt_query_txt += " and ph.ftsr_nbr='" + textBox1.Text + "'"; }
                        if (textBox3.Text != "")
                        { pkt_query_txt += " and phi.ship_wave_nbr='" + textBox3.Text + "'"; }
                        DataTable pkt_tb = oraclehelper.ExecuteDataTable(pkt_query_txt);
                        if (pkt_tb.Rows.Count == 0)
                        {
                            MessageBox.Show("未查询到出库单"); return;
                        }
                        //string batch_query_txt = @"select distinct pd.batch_nbr from pkt_dtl pd left join pkt_hdr_intrnl phi on phi.pkt_ctrl_nbr=pd.pkt_ctrl_nbr left join pkt_hdr ph on pd.pkt_ctrl_nbr=ph.pkt_ctrl_nbr  where ph.whse='S00' and pd.create_date_time>=to_date('" + dateTimePicker1.Text + "','yyyymmdd') and pd.create_date_time<=to_date('" + dateTimePicker2.Text + "','yyyymmdd') and phi.stat_code>=40 and substr(pd.pkt_ctrl_nbr,1,2) in (";
                        //for (int i = 0; i < dg_order.RowCount; i++)
                        //{
                        //    if (dg_order.Rows[i].Selected)
                        //    {
                        //        batch_query_txt += "'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'";
                                
                            
                        //    }
                        //}

                        //batch_query_txt += ")";
                        
                        //if (textBox1.Text != "")
                        //{ batch_query_txt += " and ph.ftsr_nbr='" + textBox1.Text + "'"; }
                        //if (textBox3.Text != "")
                        //{ batch_query_txt += " and phi.ship_wave_nbr='" + textBox3.Text + "'"; }
                        //batch_dt = oraclehelper.ExecuteDataTable(batch_query_txt);
                        dg_order.DataSource = pkt_tb;
                        dg_order.Refresh();
                        stats_lb_rowcount.Text = "订单数量：" + dg_order.RowCount.ToString()+"   批号数量："+batch_dt.Rows.Count.ToString();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string gsp_query_txt = "";
            string code_query_txt = "";
            string code_query_txt1 = "";
            DataTable dt_gsp_code = new DataTable();
            DataTable dt_gsp_batch = new DataTable();
            int file_count = 0;
            Boolean isfirst = true;
            //获取所有PKT的批号
            string batch_query_txt = @"select distinct pd.batch_nbr from pkt_dtl pd left join pkt_hdr_intrnl phi on phi.pkt_ctrl_nbr=pd.pkt_ctrl_nbr left join pkt_hdr ph on pd.pkt_ctrl_nbr=ph.pkt_ctrl_nbr  where ph.whse='S00' and pd.create_date_time>=to_date('" + dateTimePicker1.Text + "','yyyymmdd') and pd.create_date_time<=to_date('" + dateTimePicker2.Text + "','yyyymmdd') and phi.stat_code>=40 and pd.pkt_ctrl_nbr in (";
            for (int i = 0; i < dg_order.RowCount; i++)
            {
                if (dg_order.Rows[i].Selected)
                {
                    if (isfirst)
                    {
                        batch_query_txt += "'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'";
                        isfirst = false;
                    }
                    else
                    {
                        batch_query_txt += ",'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'";
                    }

                }
            }
            isfirst = true;

            batch_query_txt += ")";

            if (textBox1.Text != "")
            { batch_query_txt += " and ph.ftsr_nbr='" + textBox1.Text + "'"; }
            if (textBox3.Text != "")
            { batch_query_txt += " and phi.ship_wave_nbr='" + textBox3.Text + "'"; }
            batch_dt = oraclehelper.ExecuteDataTable(batch_query_txt);
            //按批号反查PKT号
            if (batch_dt.Rows.Count != 0)
            {
                toolStripProgressBar1.Maximum = batch_dt.Rows.Count;
                foreach(DataRow dr in batch_dt.Rows)//出库单导出
                {
                    gsp_query_txt=@"select sh.batchno,sh.productcode,sh.subtypeno,sh.cascade_txt,sh.packagespec,sh.comment_txt,sh.madedate,sh.validatedate,sh.workshop,sh.linename,sh.linemanager from s00_sa_relation_hdr sh where sh.batchno='"+dr[0].ToString()+"'";
                    dt_gsp_batch = oraclehelper.ExecuteDataTable(gsp_query_txt);

                    if (dt_gsp_batch.Rows.Count == 0) { MessageBox.Show("未查询到批号" + dr[0].ToString()+"的药监码关联关系，请确认是否已经导入。"); return; }

                    XmlHelper gsp_templates_xml = new XmlHelper(@"./batch_templates.XML");

                    Dictionary<string, string> dic_relation = new Dictionary<string, string>();
                    dic_relation.Add("productCode", dt_gsp_batch.Rows[0][1].ToString());
                    dic_relation.Add("subTypeNo", dt_gsp_batch.Rows[0][2].ToString());
                    dic_relation.Add("cascade", dt_gsp_batch.Rows[0][3].ToString());
                    dic_relation.Add("packageSpec", dt_gsp_batch.Rows[0][4].ToString());
                    dic_relation.Add("comment", dt_gsp_batch.Rows[0][5].ToString());
                    gsp_templates_xml.InsertMutiElement("Document/Events/Event", "Relation", dic_relation);

                    Dictionary<string, string> dic_batch = new Dictionary<string, string>();
                    dic_batch.Add("batchNo", dt_gsp_batch.Rows[0][0].ToString());
                    dic_batch.Add("madeDate", dt_gsp_batch.Rows[0][6].ToString());
                    dic_batch.Add("validateDate", dt_gsp_batch.Rows[0][7].ToString());
                    dic_batch.Add("workShop", dt_gsp_batch.Rows[0][8].ToString());
                    dic_batch.Add("lineName", dt_gsp_batch.Rows[0][9].ToString());
                    dic_batch.Add("lineManager", dt_gsp_batch.Rows[0][10].ToString());
                    gsp_templates_xml.InsertMutiElement("Document/Events/Event/Relation", "Batch", dic_batch);

                    //gsp_templates_xml.SaveAs(@".\files\" + DateTime.Now.ToString("yyyyMMdd") + "-" + dr[0].ToString() + ".xml");

                    code_query_txt = "select sd.curcode,sd.packlayer,sd.parentcode,sd.flag from s00_sa_relation_dtl sd left join c_gsp_nbr_trkg c on c.gsp_nbr=sd.curcode and c.batch_nbr=sd.batchno and c.stat_code=0 where c.pkt_ctrl_nbr in (";
                    code_query_txt1 = " union all select sd.curcode,sd.packlayer,sd.parentcode,sd.flag from s00_sa_relation_dtl sd left join c_gsp_nbr_trkg c on c.gsp_nbr=sd.parentcode and c.batch_nbr=sd.batchno and c.stat_code=0 where c.pkt_ctrl_nbr in (";
                    for (int i = 0; i < dg_order.RowCount; i++)
                    {
                        if (dg_order.Rows[i].Selected)
                        {
                            if (isfirst)
                            { 
                                code_query_txt += "'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'";
                                code_query_txt1 += "'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'"; 
                                isfirst = false;
                            }
                            else
                            { 
                                code_query_txt += ",'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'";
                                code_query_txt1 += ",'" + dg_order.Rows[i].Cells[0].Value.ToString() + "'";
                            }
                            
                        }
                    }
                    isfirst = true;
                    code_query_txt += ") and sd.batchno='" + dt_gsp_batch.Rows[0][0].ToString() + "'";
                    code_query_txt1 += ") and sd.batchno='" + dt_gsp_batch.Rows[0][0].ToString() + "'";
                    code_query_txt += code_query_txt1;

                    dt_gsp_code = oraclehelper.ExecuteDataTable(code_query_txt);
                    if (dt_gsp_code.Rows.Count == 0) { MessageBox.Show("no gsp_nbr match"); return; }
                    
                    foreach (DataRow dr_code in dt_gsp_code.Rows)
                    {
                        Dictionary<string, string> dic_code = new Dictionary<string, string>();
                        dic_code.Add("curCode", dr_code[0].ToString());
                        dic_code.Add("packLayer", dr_code[1].ToString());
                        if (dr_code[2].ToString() != "") 
                        {
                            dic_code.Add("parentCode", dr_code[2].ToString()); 
                        }
                        dic_code.Add("flag", dr_code[3].ToString());
                        gsp_templates_xml.InsertMutiElement("Document/Events/Event/Relation/Batch", "Code", dic_code);
                    }
                     gsp_templates_xml.SaveAs(@".\files\" + DateTime.Now.ToString("yyyyMMdd") + "-" + dr[0].ToString() + ".xml");
                    file_count++;
                    toolStripProgressBar1.Value = file_count;
                }
                MessageBox.Show("已生成" + file_count.ToString() + "个出库监管码文件");
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            relations_import import_frm = new relations_import();
            import_frm.ShowDialog(); 
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now.AddDays(-30);
        }
    }
}
