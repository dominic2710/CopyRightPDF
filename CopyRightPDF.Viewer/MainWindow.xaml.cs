using CopyRightPDF.Core;
using Ionic.Zip;
using Microsoft.Win32;
using PdfiumViewer.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CopyRightPDF.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string ZipPassword = "1ZpVSuHWlt50fyLp0jQn59GotXUB_XJ5WyVDt7k";
        string EncryptKey = "b14ca5898a4e4133bbce2ea2315a1916";
        string[] separator = new string[] { "\r\n", "\r", "\n" };

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open Copyright PDF File",
                Filter = "crpdf files (*.crpdf)|*.crpdf",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != true) return;

            var fileName = openFileDialog.FileName;

            using (ZipFile zipFile = ZipFile.Read(fileName))
            {
                zipFile.Password = ZipPassword;

                var server = GetFile(zipFile, "server.dat");
                if (server == null) return;
                var dataAccess = new GoogleSheetDataAccess(new MemoryStream(server));

                var info = GetFile(zipFile, "info.dat");
                if (info == null) return;

                var documentId = Encoding.UTF8.GetString(info).Split(separator, StringSplitOptions.None)[0];
                var password = Encoding.UTF8.GetString(info).Split(separator, StringSplitOptions.None)[1];

                var document = dataAccess.GetEntryByDocumentId(documentId);
                if (document == null)
                {
                    MessageBox.Show("File not exist!!", "Copyright PDF");
                    return;
                }

                if (document.Password != password)
                {
                    MessageBox.Show("Wrong password!!", "Copyright PDF");
                    return;
                }

                //Check MAC Address
                var localMacAddress = GetAllDeviceMacAddress();
                var storedDeviceMAC = document.ActivatedDevicesMAC.Split(separator, StringSplitOptions.None).ToList<string>();

                var exist = IsAllowOpen(localMacAddress, storedDeviceMAC);
                if (!exist)
                {
                    if (int.Parse(document.NumberOfLimitDevice) > int.Parse(document.NumberOfActivatedDevice))
                    {
                        storedDeviceMAC.AddRange(localMacAddress);
                        storedDeviceMAC.Remove("");
                        storedDeviceMAC = storedDeviceMAC.Distinct().ToList();

                        document.ActivatedDevicesMAC = String.Join("\r\n", storedDeviceMAC);
                        document.NumberOfActivatedDevice = (int.Parse(document.NumberOfActivatedDevice) + 1).ToString();
                        document.LatestAccess = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        dataAccess.UpdateEntry(document);
                    }
                    else
                    {
                        MessageBox.Show("File cannot open on this device", "Copyright PDF");
                        return;
                    }
                }
                else
                {
                    document.LatestAccess = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    dataAccess.UpdateEntry(document);
                }

                var pdfFile = GetFile(zipFile, "data.dat");
                var ms = new MemoryStream(pdfFile);
                pdfViewer.OpenPdf(ms);
                pdfViewer.SetZoomMode(PdfViewerZoomMode.FitWidth);
            }
        }

        byte[] GetFile(ZipFile zipFile, string entryName)
        {
            var zipEntry = zipFile[entryName];
            if (zipEntry == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                zipEntry.Extract(ms);
                ms.Position = 0;
                var decryptedFileByte = AesOperation.GetInstance.Decrypt(ms.ToArray(), EncryptKey);
                return decryptedFileByte;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            pdfViewer?.Dispose();
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetWindowDisplayAffinity(IntPtr hwnd, uint affinity);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = (new WindowInteropHelper(this)).Handle;
            bool s = SetWindowDisplayAffinity(handle, 1);
        }

        private List<string> GetAllDeviceMacAddress()
        {
            var result = new List<string>();
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterface)
            {
                var macAddress = BitConverter.ToString(network.GetPhysicalAddress().GetAddressBytes())?.Trim();
                if (macAddress != null && macAddress != "")
                    result.Add(macAddress);
            }

            return result;
        }

        bool IsAllowOpen(List<string> localDeviceMAC, List<string> storedDeviceMAC)
        {
            if (storedDeviceMAC.Count == 0) return false;
            foreach (string mac in localDeviceMAC)
            {
                if (storedDeviceMAC.Contains(mac))
                    return true;
            }

            return false;
        }
    }
}
