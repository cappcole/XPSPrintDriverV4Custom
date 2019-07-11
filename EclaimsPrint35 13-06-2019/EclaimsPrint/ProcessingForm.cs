using System;
using System.IO;
using System.Windows.Forms;

namespace EclaimsPrint
{
    public partial class ProcessingForm : Form
    {
        public static ProcessingForm window = new ProcessingForm();

        public ProcessingForm()
        {
            InitializeComponent();
        }

        private void IAPXPSPrintLogin_Load(object sender, EventArgs e)
        {
            this.BringToFront();
            this.TopMost = true;
        }

        private void ProcessingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            window = new ProcessingForm();
        }
    }


}
