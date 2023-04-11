using CopyRightPDF.Core;
using CopyRightPDF.Core.Models;
using CopyRightPDF.Pages;
using CopyRightPDF.Utilities;
using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Input;

namespace CopyRightPDF.ViewModels
{
    public class DocumentInfoViewModel : BaseViewModel, ICloseable
    {
        public event EventHandler<EventArgs> RequestClose;

        const string ZIP_PASSWORD = "1ZpVSuHWlt50fyLp0jQn59GotXUB_XJ5WyVDt7k";
        const string ENCRYPT_KEY = "b14ca5898a4e4133bbce2ea2315a1916";

        #region Variable
        private string fileId;
        private string fileName;
        private string description;
        private string inputPath;
        private string outputPath;
        private bool isAddNew;
        #endregion

        #region Properties
        public string FileId
        {
            get { return fileId; }
            set { fileId = value; OnPropertyChanged(); }
        }
        public string FileName
        {
            get
            {
                if (String.IsNullOrEmpty(fileName))
                    fileName = System.IO.Path.GetFileNameWithoutExtension(InputPath);
                return fileName;
            }
            set { fileName = value; OnPropertyChanged(); OnPropertyChanged(nameof(OutputPath)); }
        }
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }
        public string InputPath
        {
            get { return inputPath; }
            set { inputPath = value; OnPropertyChanged(); OnPropertyChanged(nameof(FileName)); OnPropertyChanged(nameof(OutputPath)); }
        }
        public string OutputPath
        {
            get
            {
                return System.IO.Path.Combine(Properties.Settings.Default.OutputPath, $"{FileName}.crpdf");
            }
            set { outputPath = value; OnPropertyChanged(); }
        }
        public bool IsAddNew
        {
            get { return isAddNew; }
            set { isAddNew = value; OnPropertyChanged(); OnPropertyChanged("ShowInputPath"); }
        }
        public Visibility ShowInputPath
        {
            get { return IsAddNew ? Visibility.Visible : Visibility.Hidden; }
        }
        public string OKButtonContent
        {
            get { return IsAddNew ? "Encrypt" : "OK"; }
        }
        public string WindowTitle
        {
            get { return IsAddNew ? "New Document" : FileName; }
        }
        public DocumentModel ReturnDocument { get; set; }
        #endregion

        #region Command
        public ICommand ReloadCommand { get; set; }
        public ICommand OKCommand { get; set; }
        public ICommand OpenFileDialogCommand { get; set; }
        #endregion  

        public DocumentInfoViewModel() { }
        public DocumentInfoViewModel(DocumentModel documentModel, bool isAddNew)
        {
            ReturnDocument = null;

            MessageQueue = new MaterialDesignThemes.Wpf.SnackbarMessageQueue();

            IsAddNew = isAddNew;
            FileId = documentModel.FileId;
            FileName = documentModel.FileName;
            Description = documentModel.Description;

            ReloadCommand = new RelayCommand<object>(p =>
            {
                return true;
            }, p =>
            {
                IsAddNew = isAddNew;
                FileId = documentModel.FileId;
                FileName = documentModel.FileName;
                Description = documentModel.Description;
            });

            OKCommand = new RelayCommand<object>(p =>
            {
                return true;
            }, p =>
            {
                //Check input
                var errorMessage = CheckInput();
                if (String.IsNullOrEmpty(errorMessage))
                {
                    MessageQueue.Enqueue(errorMessage);
                    return;
                }

                if (IsAddNew)
                {
                    //Encrypt
                    if (!File.Exists(InputPath))
                    {
                        MessageBox.Show($"The input file does not exist!", "Copyright PDF - Writer", MessageBoxButton.OK);
                        return;
                    }
                    if (!Directory.Exists(System.IO.Path.GetDirectoryName(OutputPath)))
                    {
                        MessageBox.Show($"The output folder not exist!", "Copyright PDF - Writer", MessageBoxButton.OK);
                        return;
                    }
                    if (Directory.Exists(OutputPath))
                    {
                        var result = MessageBox.Show($"The output file already exists. Do you want to override it?", "Copyright PDF - Writer", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.No) return;
                    }

                    WaitingForExecUtility.Instance.DoWork(() => CreateZipFile(), "Creating");
                    MessageQueue.Enqueue("Create file completed");
                }

                //Return document
                ReturnDocument = new DocumentModel
                {
                    FileId = FileId,
                    FileName = FileName,
                    Description = Description,
                };

                RequestClose(this, EventArgs.Empty);
            });

            OpenFileDialogCommand = new RelayCommand<object>(p => { return true; }, p =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Open PDF File",
                    Filter = "pdf files (*.pdf)|*.pdf",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() != true) return;

                InputPath = openFileDialog.FileName;
            });
        }

        private string CheckInput()
        {

            if (IsAddNew && String.IsNullOrEmpty(InputPath))
            {
                return "Input path must be input";
            }
            if (String.IsNullOrEmpty(FileName))
            {
                return "File name must be input";
            }
            return "";
        }

        void AddFileToZip(ZipFile zipFile, string entryName, byte[] data)
        {
            try
            {
                var encryptedPdfString = AesOperation.GetInstance.Encrypt(data, ENCRYPT_KEY);
                zipFile.AddEntry(entryName, encryptedPdfString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void CreateZipFile()
        {
            ZipFile zipFile = new ZipFile
            {
                Password = ZIP_PASSWORD
            };
            using (FileStream fs = new FileStream(InputPath, FileMode.Open, FileAccess.Read))
            {
                var fileByte = new byte[fs.Length];
                fs.Read(fileByte, 0, fileByte.Length);

                AddFileToZip(zipFile, "data.dat", fileByte);
            }

            using (FileStream fs = new FileStream(Properties.Settings.Default.ClientSecretFileName, FileMode.Open))
            {
                var fileByte2 = new byte[fs.Length];
                fs.Read(fileByte2, 0, fileByte2.Length);

                AddFileToZip(zipFile, "server.dat", fileByte2);
            }

            var pwBuilder = new StringBuilder();
            pwBuilder.AppendLine(FileId);
            pwBuilder.AppendLine(Properties.Settings.Default.SpreadSheetId);
            pwBuilder.AppendLine(Properties.Settings.Default.LicenseSheetName);

            AddFileToZip(zipFile, "info.dat", Encoding.ASCII.GetBytes(pwBuilder.ToString()));

            zipFile.Save(OutputPath);
        }
    }
}
