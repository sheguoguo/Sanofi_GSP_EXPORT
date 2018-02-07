using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sanofi_GSP_EXPORT
{
    public partial class gsp_verify : Form
    {
        public gsp_verify()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] cookies = webBrowser1.Document.Cookie.Split(';');
            textBox1.Text += cookies[0].Trim() + Environment.NewLine;
            textBox1.Text += cookies[1].Trim() + Environment.NewLine;
            
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //if (webBrowser1.Document.Url.ToString().Substring(0,83) == "http://traceentservice.mashangfangxin.com/ssoAction!getUserInfo.action;jsessionid=")
            if (webBrowser1.DocumentText.Trim()=="key_ver2")
            {
                compare_form compare_frm = new compare_form();
                string[] cookies = webBrowser1.Document.Cookie.Split(';');
                string[] jsesion = cookies[2].Trim().Split('=');
                string[] serverid = cookies[3].Trim().Split('=');
                compare_frm.cookie_jseesion = jsesion[1].Trim();
                compare_frm.cookie_serverid = serverid[1].Trim();

                compare_frm.ShowDialog();
                
                this.Close();
            }
        }

        private void webBrowser1_LocationChanged(object sender, EventArgs e)
        {
            
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
