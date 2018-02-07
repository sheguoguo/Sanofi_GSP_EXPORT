using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Sanofi_GSP_EXPORT
{
    public partial class mainform : Form
    {
        public mainform()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            relations_import relation_import_form = new relations_import();
            relation_import_form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            excelform excel_form = new excelform();
            excel_form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1 relation_export_form = new Form1();
            relation_export_form.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            gsp_verify gsp_veriry_form = new gsp_verify();
            gsp_veriry_form.ShowDialog();
        }
    }
}
