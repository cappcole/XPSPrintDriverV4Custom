using System;
using System.IO;
using System.Windows.Forms;

namespace EclaimsPrint
{
    public partial class SubmitForm : Form
    {
        string currentFilePath;

        //****************************************************************//
        public SubmitForm(string fileToSend)
        {
            InitializeComponent();
            SubNumTextbox.Focus();
            // Automatically load their login information if available 
            GetSetting();
            currentFilePath = fileToSend;
        }
        //****************************************************************//
        private void SubmitForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.TopMost = true;
        }
        /// <summary>
        /// Load Variables From Settings
        /// </summary>
        /// 
        private void GetSetting()//loads users information
        {
            //check to see if settings have ever been stored and load them if they have
            if(Properties.Settings.Default.UserName.Length >= 3 && Properties.Settings.Default.SubNum.ToString().Length >= 3 
                && Properties.Settings.Default.Password.Length >= 5)
            {
                SubNumTextbox.Text = Properties.Settings.Default.SubNum.ToString();
            }
        }
        //****************************************************************//
        /// <summary>
        /// Saves Users Settings To config
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="username"></param>
        /// <param name="passwordEncrypted"></param>
        private void SaveNewVariableSetting(int sub,string username,string passwordEncrypted)// saves user information
        {
            Properties.Settings.Default.SubNum = sub;
            Properties.Settings.Default.UserName = username;
            Properties.Settings.Default.Password = passwordEncrypted;
            Properties.Settings.Default.Save();
        }
        
        //****************************************************************//
        private void SubNumTextbox_TextChanged(object sender, EventArgs e)
        {

        }
        //****************************************************************//
        protected internal void SubmitXPS_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.SubNum.ToString().Length >= 3 &&
                Properties.Settings.Default.UserName.Length >= 2 &&
                Properties.Settings.Default.Password.Length >= 2) // Checks the user input to match what is expected or ask for more information
            {
                // store Textbox Data from users inside of a variable (destroyed inside of void)
                string subNumber = SubNumTextbox.Text; //to be stored in registry

                //******
                try
                {

                    UploadItem item = new UploadItem();
                    item.SubNumber = subNumber;
                    item.FilePath = currentFilePath;
                    MainForm.window.AddItem(item);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            else
            {
                MessageBox.Show("All Fields Have Not Been Field Out. Please Enter Your Infomation Again");
                ChangeButton_Click(null, null);
            }
        }
        //****************************************************************//
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        //****************************************************************//
        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }
        //****************************************************************//
        /// <summary>
        /// Determine if the user wanted to cancel or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCanel_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Sure", "Are You Sure You Want To Cancel?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                this.Close();
            }
            else if (dialogResult == DialogResult.No)
            {
                
            }
        }
        //****************************************************************//
        private void ChangeButton_Click(object sender, EventArgs e)
        {
            IAPXPSPrintLogin loginForm = new IAPXPSPrintLogin();
            loginForm.ShowDialog();
            SubNumTextbox.Text = Properties.Settings.Default.SubNum.ToString();
        }
        //****************************************************************//
    }


}
