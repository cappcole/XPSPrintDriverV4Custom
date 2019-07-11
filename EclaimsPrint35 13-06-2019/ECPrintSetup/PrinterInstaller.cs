using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Management;
using System.Runtime.InteropServices;

using System.IO;

namespace EclaimsPrint
{
    public class PrinterInstaller
    {
        private string printerIP = "127.0.0.1";
        private string driverName = "Microsoft XPS Document Writer";
        private string printerName = "CustomXPSPrinter";
        private string PrinterPortName = "CustomXPSPrinter";
        private int listeningPort = 9200;

        ManagementScope managementScope;

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);

        private void CreateManagementScope(string computerName)
        {
            var wmiConnectionOptions = new ConnectionOptions();
            wmiConnectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            wmiConnectionOptions.Authentication = AuthenticationLevel.Default;
            wmiConnectionOptions.EnablePrivileges = true; // required to load/install the driver.
                                                          // Supposed equivalent to VBScript objWMIService.Security_.Privileges.AddAsString "SeLoadDriverPrivilege", True 

            string path = "\\\\" + computerName + "\\root\\cimv2";
            managementScope = new ManagementScope(path, wmiConnectionOptions);
            managementScope.Connect();
        }
        //*******************************************************************************//
        private bool CheckPrinterPort()
        {
            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_TCPIPPrinterPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(managementScope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                if (m["Name"].ToString() == PrinterPortName)
                    return true;
            }
            return false;
        }
        //*******************************************************************************//
        private bool CreatePrinterPort()
        {
            if (CheckPrinterPort())
                return true;

            var printerPortClass = new ManagementClass(managementScope, new ManagementPath("Win32_TCPIPPrinterPort"), new ObjectGetOptions());
            printerPortClass.Get();
            var newPrinterPort = printerPortClass.CreateInstance();
            newPrinterPort.SetPropertyValue("Name", PrinterPortName);
            newPrinterPort.SetPropertyValue("Protocol", 1);
            newPrinterPort.SetPropertyValue("HostAddress", printerIP);
            newPrinterPort.SetPropertyValue("PortNumber", listeningPort);    // default=9100
            newPrinterPort.SetPropertyValue("SNMPEnabled", false);  // true?
            newPrinterPort.Put();
            return true;
        }
        //*******************************************************************************//
        private bool CreatePrinterDriver(string printerDriverFolderPath, out string message)
        {
            string printerDriverInfPath = Path.GetFileName(printerDriverFolderPath);
            message = "";
            var endResult = false;
            
            var printerDriverClass = new ManagementClass(managementScope, new ManagementPath("Win32_PrinterDriver"), new ObjectGetOptions());
            var printerDriver = printerDriverClass.CreateInstance();
            printerDriver.SetPropertyValue("Name", driverName);
            printerDriver.SetPropertyValue("FilePath", printerDriverFolderPath);
            printerDriver.SetPropertyValue("InfName", printerDriverInfPath);

            // Obtain in-parameters for the method
            using (ManagementBaseObject inParams = printerDriverClass.GetMethodParameters("AddPrinterDriver"))
            {
                inParams["DriverInfo"] = printerDriver;
                // Execute the method and obtain the return values.            

                using (ManagementBaseObject result = printerDriverClass.InvokeMethod("AddPrinterDriver", inParams, null))
                {
                    // result["ReturnValue"]
                    uint errorCode = (uint)result.Properties["ReturnValue"].Value;  // or directly result["ReturnValue"]
                                                                                    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms681386(v=vs.85).aspx
                    switch (errorCode)
                    {
                        case 0:
                            message = "Successfully connected printer.";
                            endResult = true;
                            break;
                        case 5:
                            message = "Access Denied.";
                            break;
                        case 123:
                            message = "The filename, directory name, or volume label syntax is incorrect.";
                            break;
                        case 1801:
                            message = "Invalid Printer Name.";
                            break;
                        case 1930:
                            message = "Incompatible Printer Driver.";
                            break;
                        case 3019:
                            message = "The specified printer driver was not found on the system and needs to be downloaded.";
                            break;
                    }
                }
            }
            return endResult;
        }
        //*******************************************************************************//
        private bool CreatePrinter()
        {
            var printerClass = new ManagementClass(managementScope, new ManagementPath("Win32_Printer"), new ObjectGetOptions());
            printerClass.Get();
            var printer = printerClass.CreateInstance();
            printer.SetPropertyValue("DriverName", driverName);
            printer.SetPropertyValue("PortName", PrinterPortName);
            printer.SetPropertyValue("Name", printerName);
            printer.SetPropertyValue("DeviceID", printerName);
            printer.SetPropertyValue("Location", "Front Office");
            printer.SetPropertyValue("Network", true);
            printer.SetPropertyValue("Shared", false);
            printer.Put();
            return true;
        }
        //*******************************************************************************//
        internal bool InstallPrinterWMI()
        {
            bool retVal = false;
            string printerDriverPath = "";
            bool printePortCreated = false, printeDriverCreated = false, printeCreated = false;
            try
            {

                printePortCreated = CreatePrinterPort();
                string error = "";

                var osName = GetOSName();
                //bool is64Bit = Environment.Is64BitOperatingSystem;
                
                //if(is64Bit)
                {
                    driverName = "Microsoft XPS Document Writer";
                    printerDriverPath = @"ntprint.inf";

                    if (osName.ToLower().Contains("windows 7"))
                    {
                        driverName = "Microsoft XPS Document Writer";
                        printerDriverPath = @"ntprint.inf";
                    }
                    else if (osName.ToLower().Contains("windows 10") || osName.ToLower().Contains("windows 8"))
                    {
                        driverName = "Microsoft XPS Document Writer v4";
                        printerDriverPath = @"ntprint.inf";
                    }
                }

                printeDriverCreated = CreatePrinterDriver(printerDriverPath, out error);
                printeCreated = CreatePrinter();
                //SetDefaultPrinter(printerName);
                retVal = true;
            }
            catch (Exception err)
            {
                MessageBox.Show("Error while installing printer: " + err.Message);
                //if (printePortCreated)
                //{
                //    // RemovePort
                //}
            }

            return retVal;
        }
        //*******************************************************************************//
        internal bool UnInstallPrinter()
        {
            bool retVal = false;
            managementScope = new ManagementScope(ManagementPath.DefaultPath);
            managementScope.Connect();

            SelectQuery oSelectQuery = new SelectQuery();
            oSelectQuery.QueryString = @"SELECT * FROM Win32_Printer WHERE Name = '" +
               printerName.Replace("\\", "\\\\") + "'";

            ManagementObjectSearcher oObjectSearcher =
               new ManagementObjectSearcher(managementScope, oSelectQuery);
            ManagementObjectCollection oObjectCollection = oObjectSearcher.Get();

            if (oObjectCollection.Count != 0)
            {
                foreach (ManagementObject oItem in oObjectCollection)
                {
                    oItem.Delete();
                    retVal = true;
                }
            }

            DeletePrinterPort();


            return retVal;
        }
        //*******************************************************************************//
        private void DeletePrinterPort()
        {
            //Query system for Operating System information
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_TCPIPPrinterPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(managementScope, query);

            ManagementObjectCollection queryCollection = searcher.Get();
            foreach (ManagementObject m in queryCollection)
            {
                if (m["Name"].ToString() == PrinterPortName)
                    m.Delete();
            }
        }
        //*******************************************************************************//
        private string GetOSName()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null ? name.ToString() : "Unknown";
        }
        //*******************************************************************************//
    }
}