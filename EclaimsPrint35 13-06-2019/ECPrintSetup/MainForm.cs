using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;

namespace ECPrintSetup
{
    public partial class MainForm : Form
    {
        string programDirectory = "C:\\CustomXPSPrinter";

        public MainForm()
        {
            InitializeComponent();
        }
        //****************************************************************//
        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        //****************************************************************//
        private void InstallButton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            InstallButton.Enabled = false;
            UnInstallButton.Enabled = false;
            try
            {
                EclaimsPrint.PrinterInstaller installer = new EclaimsPrint.PrinterInstaller();
                bool result = installer.InstallPrinterWMI();

                InstallApplication();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            InstallButton.Enabled = true;
            UnInstallButton.Enabled = true;
            this.UseWaitCursor = false;
        }
        //****************************************************************//
        private void UnInstallButton_Click(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            InstallButton.Enabled = false;
            UnInstallButton.Enabled = false;
            try
            {
                EclaimsPrint.PrinterInstaller installer = new EclaimsPrint.PrinterInstaller();
                bool result = installer.UnInstallPrinter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while uninstalling printer: " + ex.Message);
            }

            try
            {
                UnInstallApplication();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while uninstalling application: " + ex.Message);
            }

            InstallButton.Enabled = true;
            UnInstallButton.Enabled = true;
            this.UseWaitCursor = false;
        }
        //****************************************************************//
        private void InstallApplication()
        {

            ProcessStartInfo info = new ProcessStartInfo("Taskkill", "/IM EclaimsPrint.exe /F");
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            Process.Start(info);

            Thread.Sleep(2000);

            if (!Directory.Exists(programDirectory))
                Directory.CreateDirectory(programDirectory);
            if (Directory.Exists(programDirectory))
            {
                byte[] fileBytes = Properties.Resources.BouncyCastle_Crypto;
                File.WriteAllBytes(programDirectory + "\\BouncyCastle.Crypto.dll", fileBytes);

                fileBytes = null;
                fileBytes = Properties.Resources.RestSharp;
                File.WriteAllBytes(programDirectory + "\\RestSharp.dll", fileBytes);

                fileBytes = null;
                fileBytes = Properties.Resources.EclaimsPrint;
                File.WriteAllBytes(programDirectory + "\\CustomXPSPrinter.exe", fileBytes);

                SetAutoStart();
            }
            else
                MessageBox.Show("Failed to create program directory");
        }
        //****************************************************************//
        private void UnInstallApplication()
        {
            ProcessStartInfo info = new ProcessStartInfo("Taskkill", "/IM EclaimsPrint.exe /F");
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            Process.Start(info);

            Thread.Sleep(2000);
            if (Directory.Exists(programDirectory))
            {
                Directory.Delete(programDirectory, true);
            }

            SetAutoStart(false);
        }
        //****************************************************************//
        internal void SetAutoStart(bool install = true)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey
                        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (install)
                {
                    string currentPath = programDirectory + "\\EclaimsPrint.exe";
                    rk.SetValue("CustomXPSPrinter", currentPath);//Application.ExecutablePath.ToString());
                    MessageBox.Show("Installation Complete\n\rPlease allow Access When Prompted");

                    System.Diagnostics.Process.Start(currentPath);

                    this.Hide();
                    Thread.Sleep(500);
                    Environment.Exit(0);
                }
                else
                {
                    rk.DeleteValue("CustomXPSPrinter");
                }
            }
            catch (Exception)
            {
            }
        }
        //****************************************************************//

    }
}
