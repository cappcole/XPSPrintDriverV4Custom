using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;




namespace EclaimsPrint
{
    public partial class MainForm : Form
    {
        string language = "";

        

        public static MainForm window;

        internal BindingSource bindingSource1 = new BindingSource();
        internal List<UploadItem> itemsList = new List<UploadItem>();

        System.Windows.Forms.Timer uploadTimer = new System.Windows.Forms.Timer();
        bool isUploading = false;
        const int WS_MINIMIZEBOX = 0x20000;
        const int CS_DBLCLKS = 0x8;
        const int CS_DROPSHADOW = 0x20000;
        public const int GCL_STYLE = -26;

        [DllImport("user32.dll", EntryPoint = "GetClassLong")]
        public static extern int GetClassLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "SetClassLong")]
        public static extern int SetClassLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public MainForm()
        {
            try
            {

            }
            catch (Exception)
            {
            }

            InitializeComponent();
        }
        //****************************************************************//
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }
        //****************************************************************//
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                                          Color.SteelBlue, 1, ButtonBorderStyle.Outset,
                                          Color.SteelBlue, 1, ButtonBorderStyle.Outset,
                                          Color.SteelBlue, 1, ButtonBorderStyle.Inset,
                                          Color.SteelBlue, 1, ButtonBorderStyle.Inset);

        }
        //****************************************************************//
        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //****************************************************************//
        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                MaximizeButton.Image = Properties.Resources.Maximize;
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                MaximizeButton.Image = Properties.Resources.Restore;
            }
        }
        //****************************************************************//
        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        //****************************************************************//
        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Release the mouse capture started by the mouse down.
                TitleBar.Capture = false; //select control

                // Create and send a WM_NCLBUTTONDOWN message.
                const int WM_NCLBUTTONDOWN = 0x00A1;
                const int HTCAPTION = 2;
                Message msg =
                    Message.Create(this.Handle, WM_NCLBUTTONDOWN,
                        new IntPtr(HTCAPTION), IntPtr.Zero);
                this.DefWndProc(ref msg);
            }
        }
        //****************************************************************//
        private void TitleBar_DoubleClick(object sender, EventArgs e)
        {
            MaximizeButton_Click(null, null);
        }
        //****************************************************************//
        private void MainForm_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.Sleep(30);
                this.Invoke(new Action(() =>
                {
//#if !DEBUG
                    this.Hide();
//#endif
                    MainForm.window.Opacity = 100;
                    
                }));
            });

          

            dataGridView1.EnableHeadersVisualStyles = false;
            LoadDataGridView();

            LoadSigninInfo();

            if (Program.isActivated)
            {
                try
                {
                    uploadTimer.Interval = 2000;
                    uploadTimer.Tick += UploadTimer_Tick;
                    uploadTimer.Start();
                }
                catch (Exception)
                {
                }
            }
        }
       
        //****************************************************************//
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        //****************************************************************//
        internal void LoadDataGridView()
        {
            try
            {
                //this.dataGridView1.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
                //this.dataGridView1.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;

                dataGridView1.RowTemplate.Height = 25;
                dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.None;

                dataGridView1.DataSource = bindingSource1;
                bindingSource1.DataSource = itemsList;
                //dataGridView1.DataSource = itemsList;
                LoadEditOptionColumns();
                dataGridView1.Refresh();

                dataGridView1.Columns["SubNumber"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["FilePath"].Visible = false;
                dataGridView1.Columns["reUpload"].Visible = false;
                //dataGridView1.Columns["PrintOptionsStr"].Visible = false;

                dataGridView1.Columns["Pages"].Visible = false;
                //dataGridView1.Columns["Pages"].Width = 60;
                //dataGridView1.Columns["Pages"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Status"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                //dataGridView1.Columns["Pages"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Status"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["Delete"].Width = 80;

                for (int i = 0; i < dataGridView1.RowCount; i++)
                    dataGridView1.Rows[i].Height = 40;


                dataGridView1.ShowCellToolTips = false;
                toolTip1.AutomaticDelay = 0;

                LoadDataGridViewLanguage();
                dataGridView1.Refresh();

                //dataGridView1_SelectionChanged(null, null);

                //statusLabel.Text = String.Format("{0} users", dataGridView1.RowCount);
                //dataGridView1.Refresh();
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //****************************************************************//
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.ClearSelection();
                
            }
            catch (Exception)
            {
			  LogBuild log = New LogBuild();
			  
			  log.Logging(Exception + "MainForm, Line 224");
            }
        }
        //****************************************************************//
        private void LoadEditOptionColumns()
        {
            

                TextAndImageColumn textImageCol = new TextAndImageColumn();
                textImageCol.HeaderText = "Tools";
                if(language.Contains("ar"))
                    textImageCol.HeaderText = "خيارات";
                textImageCol.Name = "Delete";
                textImageCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //textImageCol.DefaultCellStyle.Padding = new Padding(10, 0, 0, 0);
                //textImageCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (!dataGridView1.Columns.Contains("Delete"))
                    dataGridView1.Columns.Add(textImageCol);
            }

           
            dataGridView1.Refresh();
        }
        //****************************************************************//
        private void LoadDataGridViewLanguage()
        {

            LoadTools();
			
        }
        //****************************************************************//
       
        private void dataGridView1_MouseLeave(object sender, EventArgs e)
        {
            this.toolTip1.Hide(dataGridView1);
        }
        //****************************************************************//
        private void toolTip_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            e.Graphics.FillRectangle(Brushes.Transparent, e.Bounds);
            //e.Graphics.DrawImage(Properties.Resources.ToolTip, e.Bounds);
            e.Graphics.DrawRectangle(Pens.SteelBlue, new Rectangle(0, 0, e.Bounds.Width - 1, e.Bounds.Height - 1));
            Font tooltipFont = new Font("Segoe UI", 9.0f);
            e.Graphics.DrawString(e.ToolTipText, tooltipFont, Brushes.Black, e.Bounds);
        }
        //****************************************************************//
       
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                if (dataGridView1.Columns[e.ColumnIndex].Name == "Rename")
                {
                    dataGridView1.Columns[e.ColumnIndex].Name.Save();
                }
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete")
                {
                    if (((UploadItem)bindingSource1[e.RowIndex]).Status == UploadStatus.Failed)
                        RemoveItem(e.RowIndex);
                }

                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Status")
                {
                    if (((UploadItem)bindingSource1[e.RowIndex]).Status == UploadStatus.Failed)
                    {
                        DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells["Status"];

                        if (cell.ToolTipText.Length > 0)
                        {
                            MessageBox.Show(cell.ToolTipText, Messages.Error);
                        }
                    }
                }
                
                else if (dataGridView1.Columns[e.ColumnIndex].Name == "Preview")
                {
                    try
                    {
                        System.Diagnostics.Process.Start(itemsList[e.RowIndex].FilePath);
                    }
                    catch (Exception)
                    {
                    }
                }
        }
        //****************************************************************//
        private void AddButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "XPS files (*.xps)|*.oxps";//|Word Documents(*.doc;*.docx)|*.doc;*.docx", pdf (*.pdf; .opdf;);
            openFile.FilterIndex = 0;
            openFile.Multiselect = true;


            openFile.Title = "Add file";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                
                    foreach (string fileName in openFile.FileNames)
                    {
                        AddFile(fileName);
                    }
            }
        }
        //****************************************************************//
        public static bool IsPasswordProtected(string pdfFile)
        {
            // always set false, unless you change to PDF and a PW is required
            return false;
        }
        //****************************************************************//
        public static bool IsPDFGood(string pdfFile)
        {
          //set to false, not using pdf , keep for later 
            return false;
        }
        //****************************************************************//
        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            

            if (e.RowIndex >= 0) return;  // only the column headers!
                                          // the hard work still can be done by the system:
            if (e.ColumnIndex < 4) return;

            {
                e.PaintBackground(e.CellBounds, true);
                e.PaintContent(e.CellBounds);
                // now for the lines in the header..
                Rectangle r = e.CellBounds;
                using (Pen pen0 = new Pen(dataGridView1.ColumnHeadersDefaultCellStyle.BackColor, 1))
                {
                    // first vertical grid line:
                    if (e.ColumnIndex < 0) e.Graphics.DrawLine(pen0, r.X, r.Y, r.X, r.Bottom);
                    // right border of each cell:
                    e.Graphics.DrawLine(pen0, r.Right - 1, r.Y + 1, r.Right - 1, r.Bottom - 2);
                    //e.Graphics.DrawLine(pen0, r.Right - 1, r.Y, r.Right - 1, r.Bottom);
                }
                //panelMove();
            }

            e.Handled = true;  // stop the system from any further work on the headers
        }
        //****************************************************************//
        void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > 0)
                if (e.ColumnIndex == 4)
                {

                    DataGridViewCell cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    cell.ToolTipText = "Delete";
                }
        }
        //****************************************************************//
        void panelMove() //Make larger for 4k resolution screens
        {
            int rh = dataGridView1.RowTemplate.Height;
            int rowCount = 2;
            if (dataGridView1.RowCount > rowCount)
            		rowCount = dataGridView1.RowCount;
            		panel1.Location = new Point(dataGridView1.Width/2-panel1.Width/2,
                dataGridView1.ColumnHeadersHeight + 1 + (rowCount) * rh);
           		panel1.Location = new Point(dataGridView1.Width / 2 - panel1.Width / 2,
            		dataGridView1.Height / 2 - panel1.Height / 2);
        }
        //****************************************************************//
        private void dataGridView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }
        //****************************************************************//
        private void dataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            foreach (string file in files)
            {
                if (file.ToLower().EndsWith(".xps") || file.ToLower().EndsWitch(".oxps") // add docx or pdf to accept those
               
                {
                    AddFile(file);
                }
                else
                {
                    MessageBox.Show(Messages.FileType, Messages.Error);
                }
            }
        }
        //****************************************************************//
        public void RemoveItem(int index)
        {
            this.Invoke(new Action(() =>
            {
                try
                {
                    File.Delete(itemsList[index].FilePath);
                    bindingSource1.RemoveAt(index);
                }
                catch (Exception)
                {
				  log.Logging(Exception + "Line 433, Permisions issue?")
                }
            }));
        }
        //****************************************************************//
        public void AddItem(UploadItem item)
        {
            
                this.Invoke(new Action(() =>
                {
                    bindingSource1.Add(item);
                    LoadTools();
                    if (!uploadTimer.Enabled)
                        uploadTimer.Enabled = true;

                }));
            
        }
        //****************************************************************//
        private void LoadTools()
        {
           

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (((UploadItem)bindingSource1[i]).Status == UploadStatus.Failed)
                {
                    ((TextAndImageCell)dataGridView1.Rows[i].Cells["Delete"]).Image = (Image)Properties.Resources.delete;

                    ((TextAndImageCell)dataGridView1.Rows[i].Cells["Delete"]).Value = "Delete";
                }
                else
                {
                    ((TextAndImageCell)dataGridView1.Rows[i].Cells["Delete"]).Image = null;
                    ((TextAndImageCell)dataGridView1.Rows[i].Cells["Delete"]).Value = null;
                }
            }

            dataGridView1.Refresh();
        }
        //****************************************************************//
        private void AddFile(string filePath)
        {
            string FileExtension = Path.GetExtension(filePath);
            string tempFile = Program.tempDirectory + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xps";
            if (FileExtension.ToLower() == ".xps")
            {
               
                
                File.Copy(filePath, tempFile);

                UploadItem item = new UploadItem();
                item.FilePath = tempFile;

                SubmitForm submitForm = new SubmitForm(tempFile);
                submitForm.ShowDialog();
                //AddItem(item);
            }
            else if (FileExtension.ToLower() == ".doc" || FileExtension.ToLower() == ".docx")
            {
               MessageBox.Show(".xps or .oxps file types only. File will be disguarded.")
			   this.close(); 
            }
        }
        //****************************************************************//
        private int GetPageCount(string pdfFile)
        {
            int pageCount = 0;

            try
            {
                using (FileStream fs = new FileStream(pdfFile, FileMode.Open, FileAccess.Read))
                {
                    StreamReader r = new StreamReader(fs);
                    string pdfText = r.ReadToEnd();

                    Regex rx1 = new Regex(@"/Type\s*/Page[^s]");
                    MatchCollection matches = rx1.Matches(pdfText);

                    pageCount = matches.Count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("{0}", ex));
            }

            return pageCount;
        }
        //****************************************************************//
        private void UploadButton_Click(object sender, EventArgs e)
        {
            UploadButton.Visible = false;
            ThreadPool.QueueUserWorkItem(delegate
        {
            for (int i = 0; i < bindingSource1.Count; i++)
            {
                try
                {
                    
                        var item = (UploadItem)bindingSource1[i];

                        if (!item.reUpload)
                            continue;

                        item.reUpload = false;
                        item.Status = UploadStatus.Uploading;

                        this.Invoke(new Action(() =>
                        {
                            dataGridView1.Refresh();
                        }));

                        bool reUpload = false;
                        bool result = ProcessUpload(item, out reUpload);

                        item.reUpload = reUpload;
                        if (result)
                        {
                            item.Status = UploadStatus.Uploaded;
                        }
                        else
                        {
                            item.Status = UploadStatus.Failed;
                        }

                        this.Invoke(new Action(() =>
                        {
                            dataGridView1.Refresh();
                        }));
                    }
                
                catch (Exception)
                {
				     log.Logging(Exception + "MainForm Line 534, Status failure.")
                }
            }
        });
        }
        //****************************************************************//
        private void UploadTimer_Tick(object sender, EventArgs e)
        {
            bool enableReUploadButton = false;
            if(!isUploading)
            ThreadPool.QueueUserWorkItem(delegate
            {
                isUploading = true;
                try
                {
                    
                    //foreach (UploadItem item in bindingSource1)
                    for(int i=0;i<bindingSource1.Count;i++)
                    {

                        try
                        {
                            UploadItem item = (UploadItem)bindingSource1[i];
                            //if (Settings.CurrentSettings.signedIn)
                            {

                                if (item.reUpload)
                                {
                                    enableReUploadButton = true;
                                }


                                this.Invoke(new Action(() =>
                                {
                                    Font cellFont = new Font("Segoe UI", 8.0f);
                                    if (item.Status == UploadStatus.Failed)
                                        dataGridView1.Rows[i].Cells["Status"].Style.Font = new Font(cellFont, FontStyle.Underline);

                                    else
                                        dataGridView1.Rows[i].Cells["Status"].Style.Font = new Font(cellFont, FontStyle.Regular);

                                }));
                                if (item.Status == UploadStatus.Uploaded ||
                                    item.Status == UploadStatus.Uploading ||
                                    item.Status == UploadStatus.Failed)
                                    continue;


                                item.Status = UploadStatus.Uploading;

                                this.Invoke(new Action(() =>
                                {
                                    dataGridView1.Refresh();
                                }));

                                bool reUpload = false;
                                bool result = ProcessUpload(item, out reUpload);

                                item.reUpload = reUpload;
                                if (result)
                                {
                                    item.Status = UploadStatus.Uploaded;
                                }
                                else
                                {

                                    item.Status = UploadStatus.Failed;

                                    this.Invoke(new Action(() =>
                                    {
                                        Font cellFont = new Font("Segoe UI", 8.0f);
                                        dataGridView1.Rows[i].Cells["Status"].Style.Font = new Font(cellFont, FontStyle.Underline);

                                    }));
                                }

                                this.Invoke(new Action(() =>
                                {
                                    LoadTools();
                                    dataGridView1.Refresh();
                                }));
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                catch (Exception)
                {

                }

                isUploading = false;
                this.Invoke(new Action(() =>
                {
                    //uploadTimer.Enabled = true;
                    UploadButton.Visible = enableReUploadButton;
                }));
            });
        }
        //****************************************************************//
        private bool ProcessUpload(UploadItem item, out bool reUpload)
        {
            bool retVal = false;
            reUpload = false;

            string toolTip = "";

            try
            {
                // Call Getkey to get Publickey 
                string apiKey = APICalls.GetKey(item.SubNumber);
                File.WriteAllText(Program.tempDirectory + "\\IAPubkey.pem", apiKey); //store in registry 

                if (apiKey.Length > 10) // if its shorter than 10 character than the key failed and was not returned 
                {

                    string whatHappened = APICalls.SendXPSFile(item.SubNumber, File.ReadAllBytes(item.FilePath));
                    LogBuild.WriteLog(whatHappened);
                    int respMsg = whatHappened.Length;
                    if (whatHappened.Length >= 10)
                    {
                        retVal = true;
                        toolTip = "Success! Your Claim Was Sent.";
                    }
                    else if (whatHappened.Length <= 9 & whatHappened.Length >= 4)
                    {
                        toolTip = Messages.invalidCred;
                        reUpload = true;
                    }
                    else
                    {
                        reUpload = true;
                        toolTip = "The Claim Has Failed. Please Try Your Claim Again. Contact IAPlus"
                                        + " If The Issue Persist.";
                    }
                }
                else
                {
                    reUpload = true;
                    toolTip = Messages.invalidCred;
                }
            }
            catch(Exception)
            {
                toolTip = Messages.connectionString;
            }

            this.Invoke(new Action(() =>
            {
                try
                {
                    DataGridViewCell cell = this.dataGridView1.Rows[bindingSource1.IndexOf(item)].Cells["Status"];
                    cell.ToolTipText = toolTip;
                }
                catch (Exception) { }
            }));


            return retVal;
        }
        //****************************************************************//
        internal void SignInLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IAPXPSPrintLogin loginForm = new IAPXPSPrintLogin();
            loginForm.ShowDialog();
            this.Hide();
        }
        //****************************************************************//
        
        //****************************************************************//
        private void MainForm_Activated(object sender, EventArgs e)
        {
            this.TopMost = false;
        }
        //****************************************************************//
    }
}
