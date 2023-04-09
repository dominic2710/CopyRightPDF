using CopyRightPDF.Core;
using CopyRightPDF.Core.Models;
using Ionic.Zip;
using Microsoft.Win32;
using PdfiumViewer.Core;
using PdfiumViewer.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
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
using Path = System.IO.Path;

namespace CopyRightPDF
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
        GoogleSheetDataAccess dataAccess;

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    string crPdfFileName;
            //    var saveFileDialog = new SaveFileDialog
            //    {
            //        Title = "Save CrPdf",
            //        FileName = Path.GetFileNameWithoutExtension(txtPdfFile.Text) + ".crpdf",
            //        Filter = "crpdf files (*.crpdf)|*.crpdf",
            //    };

            //    if (saveFileDialog.ShowDialog() != true) return;
            //    crPdfFileName = saveFileDialog.FileName;

            //    var newGuid = Guid.NewGuid();
            //    ZipFile zipFile = new ZipFile
            //    {
            //        Password = ZipPassword
            //    };

            //    using (FileStream fs = new FileStream(txtPdfFile.Text, FileMode.Open, FileAccess.Read))
            //    {
            //        var fileByte = new byte[fs.Length];
            //        fs.Read(fileByte, 0, fileByte.Length);

            //        AddFileToZip(zipFile, "data.dat", fileByte);
            //    }

            //    using (FileStream fs = new FileStream("crpdfapi-2238f50f130f.json", FileMode.Open))
            //    {
            //        var fileByte2 = new byte[fs.Length];
            //        fs.Read(fileByte2, 0, fileByte2.Length);

            //        AddFileToZip(zipFile, "server.dat", fileByte2);
            //    }

            //    var pwBuilder = new StringBuilder();
            //    pwBuilder.AppendLine(newGuid.ToString());
            //    pwBuilder.AppendLine(txtPassword.Text);

            //    AddFileToZip(zipFile, "info.dat", Encoding.ASCII.GetBytes(pwBuilder.ToString()));

            //    zipFile.Save(crPdfFileName);

            //    var document = new DocumentModel
            //    {
            //        DocumentId = newGuid.ToString(),
            //        Password = txtPassword.Text,
            //        NumberOfLimitDevice = "1",
            //        NumberOfActivatedDevice = "0",
            //    };
            //    dataAccess.CreateEntry(document);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "Copy Right PDF");
            //}
        }

        void AddFileToZip(ZipFile zipFile, string entryName, byte[] data)
        {
            try
            {
                var encryptedPdfString = AesOperation.GetInstance.Encrypt(data, EncryptKey);
                zipFile.AddEntry(entryName, encryptedPdfString);
            }
            catch (Exception ex)
            {
                throw ex;
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
            //using (FileStream fs = new FileStream("crpdfapi-2238f50f130f.json", FileMode.Open, FileAccess.Read))
            //{
            //    dataAccess = new GoogleSheetDataAccess(fs);
            //}
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Open PDF File",
                Filter = "pdf files (*.pdf)|*.pdf",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() != true) return;

            txtPdfFile.Text = openFileDialog.FileName;

            FileStream pdfFileStream = new FileStream(txtPdfFile.Text, FileMode.Open, FileAccess.Read, FileShare.Read);
            pdfViewer.OpenPdf(pdfFileStream);
            pdfViewer.SetZoomMode(PdfViewerZoomMode.FitWidth);
        }
    }
}

