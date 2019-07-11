using System;
using System.IO;
using System.Windows.Forms;

namespace EclaimsPrint
{
    public partial class IAPXPSPrintLogin : Form
    {

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
                UserNameTextBox.Text = Properties.Settings.Default.UserName;
                PasswordTextBox.Text = Properties.Settings.Default.Password;
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
        public IAPXPSPrintLogin()
        {
            InitializeComponent();
            SubNumTextbox.Focus();
            // Automatically load their login information if available 
            GetSetting();
        }
        //****************************************************************//
        private void SubNumTextbox_TextChanged(object sender, EventArgs e)
        {

        }
        //****************************************************************//
        protected internal void LoginButton_Click(object sender, EventArgs e)
        {
            if (SubNumTextbox.TextLength >= 3
                && UserNameTextBox.TextLength >= 2
                && PasswordTextBox.TextLength >= 2) // Checks the user input to match what is expected or ask for more information
            {
                // store Textbox Data from users inside of a variable (destroyed inside of void)
                string subNumber = SubNumTextbox.Text; //to be stored in registry
                string userName = UserNameTextBox.Text; //to be stored in registry
                string password = PasswordTextBox.Text; //to be stored in registry


                //******
                try
                {
                    


                    if (Properties.Settings.Default.Password != password)
                    {
                        string apiKey = APICalls.GetKey(subNumber);
                        File.WriteAllText(Program.tempDirectory + "\\IAPubkey.pem", apiKey);
                        password = Crypto.Encrypt(password);
                    }
                    SaveNewVariableSetting(Convert.ToInt32(subNumber),
                                            userName,
                                            password);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            else MessageBox.Show("All Fields Have Not Been Field Out. Please Enter Your Infomation Again");
            SubNumTextbox.Focus();
        }
        //****************************************************************//
        private void IAPXPSPrintLogin_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.TopMost = true;
        }
        //****************************************************************//
        private void UserNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }
        //****************************************************************//
        /// <summary>
        /// Checks to see if 'Enter" is pressed and fires Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckKeys(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                SubmitXPS.PerformClick();
            }
        }
        //****************************************************************//
        private void PasswordTextBox_TextChanged(object sender, EventArgs e)
        {
            this.PasswordTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckKeys);
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
    }


}
