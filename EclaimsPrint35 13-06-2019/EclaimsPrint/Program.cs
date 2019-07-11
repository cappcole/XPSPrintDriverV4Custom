using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using System.Threading;
//using System.Threading.Tasks;

using System.Diagnostics;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Management;
using System.Drawing;

namespace EclaimsPrint
{
    static class Program
    {
        internal static Mutex mutex = new Mutex(true, ""); //needs a GUID

        private static NotifyIcon trayIcon;
        private static ContextMenu trayMenu;

        static bool isRunning = false;
        static Socket ServerMainSocket = null;
        static Socket currentClient = null;
        static Thread ServerThread;
        static object progressLock = new object();

        static byte[] showWindow = new byte[] { 0x02, 0x73, 0x68, 0x6f, 0x77, 0x70, 0x72, 0x69, 0x6e, 0x74, 0x2e, 0x73, 0x61, 0x03 };

        internal static bool isActivated = false;

        internal static string tempDirectory = Path.GetTempPath() + "CustomXPSPrint\\";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            isActivated = true;

            if (isActivated)
            {
                Thread.Sleep(500);

                try
                {

                    if (mutex.WaitOne(TimeSpan.Zero, true))
                    {

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        if (!Directory.Exists(tempDirectory))
                            Directory.CreateDirectory(tempDirectory);
                        //CleanTempFolder();

                        MainForm.window = new MainForm();
                        LoadTrayOptions();

#if !DEBUG
                        SetAutoStart();
#endif
                        Start();

                        MainForm.window.Opacity = 0;
                        Application.Run(MainForm.window);
                    }
                    else
                    {
                        try
                        {
                            Socket pSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                            pSocket.SendTimeout = 1500;
                            pSocket.ReceiveTimeout = 1500;
                            pSocket.Connect("127.0.0.1", 9200);
                            Thread.Sleep(200);
                            pSocket.Send(showWindow);
                            Thread.Sleep(1000);
                            
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        //****************************************************************//
        #region Tray Icon stuff
        private static void LoadTrayOptions()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("View Current Tasks", TrayIcon_Click);
#if DEBUG
            trayMenu.MenuItems.Add("Exit", OnExit);
#endif

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "CustomXPSPrint";
            //trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            trayIcon.Icon = new Icon(MainForm.window.Icon, 40, 40);
            trayIcon.Click += TrayIcon_Click;

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private static void TrayIcon_Click(object sender, EventArgs e)
        {

            MainForm.window.Invoke(new Action(() =>
            {
                if (MainForm.window.Visible)
                {
                    MainForm.window.BringToFront();
                    MainForm.window.WindowState = FormWindowState.Normal;
                    MainForm.window.MaximizeButton.Image = Properties.Resources.Maximize;
                }
                else
                {
                    MainForm.window.Show();
                    MainForm.window.BringToFront();
                    MainForm.window.WindowState = FormWindowState.Normal;
                    MainForm.window.MaximizeButton.Image = Properties.Resources.Maximize;
                }
            }));
        }


        private static void OnExit(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this application?", "WARNING: Closing Print.sa", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    trayIcon.Visible = false;
                    Thread.Sleep(100);
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                }
            }
        }
        #endregion
        //****************************************************************//
        private static void Start()
        {
            if (!isRunning)
            {
                if (ServerMainSocket == null)
                {
                    ServerMainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    ServerMainSocket.Bind(new IPEndPoint(IPAddress.Any, 9200));
                }

                ServerThread = new Thread(ServerThreadFunction);
                isRunning = true;
                ServerThread.Start();

                //StartButton.Text = "Stop Server";
                Console.WriteLine("Server started, listening on port " + 9200);
            }
            else
            {
                isRunning = false;
                ServerMainSocket.Close();
                //ServerMainSocket.Disconnect(false);
                ServerMainSocket = null;
                //StartButton.Text = "Start Server";
            }
        }
        //****************************************************************//
        private static void ServerThreadFunction()
        {
            #region Server thread
            while (isRunning)
            {
                try
                {
                    if (ServerMainSocket != null)
                    {
                        ServerMainSocket.Listen(10);
                        Socket TempSock = ServerMainSocket.Accept();
                        currentClient = TempSock;

                        ThreadPool.QueueUserWorkItem(delegate
                        {
                            ClientFunction(TempSock);
                        });

                        //Task.Factory.StartNew(() => { ClientFunction(TempSock); });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("in server function: " + ex.Message);
                }
            }
            #endregion
        }
        //****************************************************************//
        internal static void ClientFunction(Socket ClientSocket)
        {
            int completeDataLength = 0;
            bool progressShown = false;
            bool cancelled = false;
            try
            {

                byte[] buffer = new byte[1000000];
                int loopCounter = 0;

                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                ClientSocket.Send(new byte[] { 0x06 });

                string xpsFileName = tempDirectory + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss");
                xpsFileName += ".xps";

                using (var stream = new FileStream(xpsFileName, FileMode.Append))
                {
                    while (ClientSocket.IsConnected() && isRunning)
                    {
                        //Thread.Sleep(1);
                        loopCounter++;
                        int count = ClientSocket.Receive(buffer);

                        if (cancelled)
                            break;
                        else
                            stream.Write(buffer, 0, count);

                        completeDataLength += count;

                        #region Show Window command
                        if (count == showWindow.Length)
                        {
                            string cmd = Encoding.ASCII.GetString(buffer, 0, count);
                            string showWnd = Encoding.ASCII.GetString(showWindow);

                            if (cmd.ToLower() == showWnd.ToLower())
                            {
                                {
                                    MainForm.window.Invoke(new Action(() =>
                                    {
                                        MainForm.window.Show();
                                        MainForm.window.BringToFront();
                                    }));
                                }

                                ClientSocket.Close();
                                ClientSocket = null;
                                return;
                            }
                        }
                        #endregion

                        #region show progress window
                        else
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {
                                try
                                {
                                    lock (progressLock)
                                    {
                                        if (!progressShown)
                                        {
                                            progressShown = true;
                                            ProcessingForm.window = new ProcessingForm();
                                            Application.Run(ProcessingForm.window);
                                            if (ProcessingForm.window.DialogResult == DialogResult.Cancel)
                                                cancelled = true;
                                        }
                                   
                                catch (Exception)
                                {
                                }
                            });

                        }
                        #endregion
                    }
                }
                stopWatch.Stop();

                if (cancelled)
                {
                    var printJobs = GetPrintJobsCollection("CustomXPSPrint");
                    CancelPrintJob("CustomXPSPrint", printJobs[0]);
                }

                ClientSocket.Close();
                ClientSocket = null;

                Thread.Sleep(100);
                ProcessingForm.window.Invoke(new Action(() =>
                {
                    ProcessingForm.window.TopMost = false;
                    ProcessingForm.window.Close();
                }));
                ProcessingForm.window = null;

                SubmitForm submitForm = new SubmitForm(xpsFileName);
                submitForm.ShowDialog();

                MainForm.window.Invoke(new Action(() =>
                {
                    if (submitForm.DialogResult == DialogResult.OK)
                    {
                        MainForm.window.TopMost = true;
                        TrayIcon_Click(null, null);

                        MainForm.window.Focus();
                        MainForm.window.BringToFront();
                    }
                }));
            }
            catch (Exception ex)
            {
                try
                {
                    ProcessingForm.window.Invoke(new Action(() =>
                    {
                        ProcessingForm.window.TopMost = false;
                        ProcessingForm.window.Close();
                    }));
                    ProcessingForm.window = null;
                }
                catch (Exception)
                {
                }
                Console.WriteLine(String.Format("in ClientFunction: {0} **** {1} **** {2}", ex.Message, ex.InnerException, ex.StackTrace));
            }
            finally
            {
                GC.Collect();
            }
           
        }
        //****************************************************************//
        internal static void CleanTempFolder()
        {
            string[] filePaths = Directory.GetFiles(tempDirectory);
            foreach (string filePath in filePaths)
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception)
                {
                }
            }
        }
        //****************************************************************//
        internal static void SetAutoStart(bool install = true)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (install)
                {
                    string currentPath = Application.ExecutablePath.ToString();
                    rk.SetValue("CustomXPSPrint", currentPath);//Application.ExecutablePath.ToString());
                }
                else
                {
                    rk.DeleteValue("CustomXPSPrint");
                }
            }
            catch (Exception)
            {
            }
        }
        //****************************************************************//
        public static StringCollection GetPrintJobsCollection(string printerName)
        {
            StringCollection printJobCollection = new StringCollection();
            string searchQuery = "SELECT * FROM Win32_PrintJob";

            /*searchQuery can also be mentioned with where Attribute,
                but this is not working in Windows 2000 / ME / 98 machines 
                and throws Invalid query error*/
            ManagementObjectSearcher searchPrintJobs =
                      new ManagementObjectSearcher(searchQuery);
            ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
            foreach (ManagementObject prntJob in prntJobCollection)
            {
                System.String jobName = prntJob.Properties["Name"].Value.ToString();

                //Job name would be of the format [Printer name], [Job ID]
                char[] splitArr = new char[1];
                splitArr[0] = Convert.ToChar(",");
                string prnterName = jobName.Split(splitArr)[0];
                string documentName = prntJob.Properties["Document"].Value.ToString();
                if (String.Compare(prnterName, printerName, true) == 0)
                {
                    printJobCollection.Add(jobName);
                }
            }
            return printJobCollection;
        }
        //****************************************************************//
        public static bool CancelPrintJob(string printerName, string printJobName)
        {
            bool isActionPerformed = false;
            string searchQuery = "SELECT * FROM Win32_PrintJob";
            ManagementObjectSearcher searchPrintJobs =
                   new ManagementObjectSearcher(searchQuery);
            ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
            foreach (ManagementObject prntJob in prntJobCollection)
            {
                System.String jobName = prntJob.Properties["Name"].Value.ToString();
                //Job name would be of the format [Printer name], [Job ID]
                char[] splitArr = new char[1];
                splitArr[0] = Convert.ToChar(",");
                string prnterName = jobName.Split(splitArr)[0];
                string documentName = prntJob.Properties["Document"].Value.ToString();
                if (String.Compare(prnterName, printerName, true) == 0)
                {
                    if (jobName == printJobName)
                    {
                        //performs a action similar to the cancel 
                        //operation of windows print console
                        prntJob.Delete();
                        isActionPerformed = true;
                        break;
                    }
                }
            }
            return isActionPerformed;
        }
        //****************************************************************//
    }
}
