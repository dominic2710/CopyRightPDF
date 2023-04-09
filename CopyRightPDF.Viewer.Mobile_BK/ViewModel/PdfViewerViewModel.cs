using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using Microsoft.Maui.Storage;
//using Ionic.Zip;
//using CopyRightPDF.Core;
//using CopyRightPDF.Viewer.Mobile.Pages;
using System.Net.NetworkInformation;

namespace CopyRightPDF.Viewer.Mobile.ViewModel
{
    public class PdfViewerViewModel : BaseViewModel
    {
        private Stream m_pdfDocumentStream;
        private ICommand m_openDocumentCommand;

        const string ZIP_PASSWORD = "1ZpVSuHWlt50fyLp0jQn59GotXUB_XJ5WyVDt7k";
        const string ENCRYPT_KEY = "b14ca5898a4e4133bbce2ea2315a1916";
        const string DATA_FILE_NAME = "data.dat";
        const string SERVER_FILE_NAME = "server.dat";
        const string INFO_FILE_NAME = "info.dat";
        const string SEPARATOR = "\r\n";

        /// <summary>
        /// Command to open document using a file picker.
        /// </summary>
        public ICommand OpenDocumentCommand
        {
            get
            {
                if (m_openDocumentCommand == null)
                    m_openDocumentCommand = new Command<object>(OpenDocument);
                return m_openDocumentCommand;
            }
        }

        /// <summary>
        /// The PDF document stream that is loaded into the instance of the PDF viewer. 
        /// </summary>
        public Stream PdfDocumentStream
        {
            get
            {
                return m_pdfDocumentStream;
            }
            set
            {
                m_pdfDocumentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }

        async void OpenDocument(object commandParameter)
        {
            //Create file picker with file type as PDF.
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "public.pdf" } },
                        { DevicePlatform.Android, new[] { "application/pdf" } },
                        { DevicePlatform.WinUI, new[] { "pdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                    });

            //Provide picker title if required.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType,
            };
            //await PickAndShow(options);
        }

        /// <summary>
        /// Picks the file from local storage and store as stream.
        /// </summary>
        //public async Task PickAndShow(PickOptions options)
        //{
        //    try
        //    {
        //        //Pick the file from local storage.
        //        var result = await FilePicker.Default.PickAsync(options);
        //        if (result == null) return;

        //        using (ZipFile zipFile = ZipFile.Read(result.FileName))
        //        {
        //            zipFile.Password = ZIP_PASSWORD;

        //            var info = GetFile(zipFile, "info.dat");
        //            if (info == null) return;

        //            var documentId = Encoding.UTF8.GetString(info).Split(SEPARATOR, StringSplitOptions.None)[0];
        //            var spreadSheetId = Encoding.UTF8.GetString(info).Split(SEPARATOR, StringSplitOptions.None)[1];
        //            var licenseSheetName = Encoding.UTF8.GetString(info).Split(SEPARATOR, StringSplitOptions.None)[3];

        //            var server = GetFile(zipFile, "server.dat");
        //            if (server == null) return;
        //            var serverFileStream = new MemoryStream(server);

        //            var dataProvider = new CopyRightPDFDataProvider(serverFileStream, spreadSheetId, "", licenseSheetName);
        //            string enteredPassword = "";

        //            var inputDialog = new InputDialog
        //            {
        //                Title = "Enter a value",
        //            };

        //            inputDialog.OkCommand = new Command(() => { enteredPassword = inputDialog.InputText; });
        //            await inputDialog.ShowAsync();

        //            var license = dataProvider.GetRequiredPassword(documentId, enteredPassword);
        //            if (license == null)
        //            {
        //                await App.Current.MainPage.DisplayAlert("License is not exist or invalid password!", "Message", "OK");
        //                return;
        //            }

        //            //Check MAC Address
        //            var localMacAddress = GetAllDeviceMacAddress();
        //            var storedDeviceMAC = license.ActivatedDeviceMAC.Split(SEPARATOR, StringSplitOptions.None).ToList<string>();

        //            var exist = IsAllowOpen(localMacAddress, storedDeviceMAC);
        //            if (!exist)
        //            {
        //                if (license.NumberOfLimitDevice > license.NumberOfActivatedDevice)
        //                {
        //                    storedDeviceMAC.AddRange(localMacAddress);
        //                    storedDeviceMAC.Remove("");
        //                    storedDeviceMAC = storedDeviceMAC.Distinct().ToList();

        //                    license.ActivatedDeviceMAC = String.Join("\r\n", storedDeviceMAC);
        //                    license.NumberOfActivatedDevice = license.NumberOfActivatedDevice + 1;
        //                    license.LastAccess = DateTime.Now;
        //                    dataProvider.UpdateLicenseAccess(license);
        //                }
        //                else
        //                {
        //                    await App.Current.MainPage.DisplayAlert("File cannot open on this device", "Copyright PDF", "OK");
        //                    return;
        //                }
        //            }
        //            else
        //            {
        //                license.LastAccess = DateTime.Now;
        //                dataProvider.UpdateLicenseAccess(license);
        //            }

        //            var pdfFile = GetFile(zipFile, "data.dat");
        //            var ms = new MemoryStream(pdfFile);

        //            PdfDocumentStream = ms;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //Display error when file picker failed to open files.
        //        string message;
        //        if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
        //            message = ex.Message;
        //        else
        //            message = "File open failed.";
        //        Application.Current?.MainPage?.DisplayAlert("Error", message, "OK");
        //    }
        //}
        //byte[] GetFile(ZipFile zipFile, string entryName)
        //{
        //    var zipEntry = zipFile[entryName];
        //    if (zipEntry == null) return null;

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        zipEntry.Extract(ms);
        //        ms.Position = 0;
        //        var decryptedFileByte = AesOperation.GetInstance.Decrypt(ms.ToArray(), ENCRYPT_KEY);
        //        return decryptedFileByte;
        //    }
        //}

        //private List<string> GetAllDeviceMacAddress()
        //{
        //    var result = new List<string>();
        //    var networkInterface = NetworkInterface.GetAllNetworkInterfaces();

        //    foreach (var network in networkInterface)
        //    {
        //        var macAddress = BitConverter.ToString(network.GetPhysicalAddress().GetAddressBytes())?.Trim();
        //        if (macAddress != null && macAddress != "")
        //            result.Add(macAddress);
        //    }

        //    return result;
        //}

        //bool IsAllowOpen(List<string> localDeviceMAC, List<string> storedDeviceMAC)
        //{
        //    if (storedDeviceMAC.Count == 0) return false;
        //    foreach (string mac in localDeviceMAC)
        //    {
        //        if (storedDeviceMAC.Contains(mac))
        //            return true;
        //    }

        //    return false;
        //}
    }
}
