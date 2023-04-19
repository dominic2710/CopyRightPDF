using CopyRightPDF.Core;
using CopyRightPDF.Core.Models;
using CopyRightPDF.Pages;
using CopyRightPDF.Utilities;
using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace CopyRightPDF.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Variable
        private List<DocumentModel> documents;
        private ObservableCollection<DocumentModel> documentsObservable;
        private ObservableCollection<LicenseModel> licensesObservable;
        private DocumentModel selectedDocument;
        private LicenseModel selectedLicense;
        private CopyRightPDFDataProvider dataProvider;
        private string filterDocumentName;
        private string filterLicenseName;
        private bool isFilterStatusRegistered;
        private bool isFilterStatusApproved;
        private bool isFilterStatusOpened;
        private bool isFilterStatusExpired;
        #endregion

        #region Properties
        public List<DocumentModel> Documents
        {
            get { return documents; }
            set { documents = value; OnPropertyChanged(); }
        }
        public ObservableCollection<DocumentModel> DocumentsObservable
        {
            get
            {
                return new ObservableCollection<DocumentModel>(
                                    Documents.Where(x => (!String.IsNullOrEmpty(FilterDocumentName)
                                                && (x.FileName.ToUpper().Contains(FilterDocumentName.ToUpper())
                                                    || x.Description.ToUpper().Contains(FilterDocumentName.ToUpper())))
                                                || String.IsNullOrEmpty(FilterDocumentName)));
            }
        }

        public ObservableCollection<LicenseModel> LicensesObservable
        {
            get
            {
                if (SelectedDocument == null) return new ObservableCollection<LicenseModel>();
                return new ObservableCollection<LicenseModel>(
                        SelectedDocument.Licenses.Where(x => (!String.IsNullOrEmpty(FilterLicenseName)
                                                && (x.CustomerName.ToUpper().Contains(FilterLicenseName.ToLower())
                                                    || x.Password.ToUpper().Contains(FilterLicenseName.ToUpper())))
                                                || String.IsNullOrEmpty(FilterLicenseName))
                                                    .Where(x => FilterStatuss.Contains(x.Status)));
            }
        }

        public DocumentModel SelectedDocument
        {
            get { return selectedDocument; }
            set
            {
                selectedDocument = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LicensesObservable));
            }
        }
        public LicenseModel SelectedLicense
        {
            get { return selectedLicense; }
            set { selectedLicense = value; OnPropertyChanged(); }
        }
        public bool IsDataChanged { get; set; }
        public string FilterDocumentName
        {
            get { return filterDocumentName; }
            set
            {
                filterDocumentName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DocumentsObservable));
            }
        }

        public string FilterLicenseName
        {
            get { return filterLicenseName; }
            set
            {
                filterLicenseName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LicensesObservable));
            }
        }

        public bool IsFilterStatusRegistered
        {
            get { return isFilterStatusRegistered; }
            set
            {
                isFilterStatusRegistered = value;

                if (isFilterStatusRegistered)
                    FilterStatuss.Add("Registered");
                else
                    FilterStatuss.Remove("Registered");

                OnPropertyChanged();
                OnPropertyChanged(nameof(LicensesObservable));
            }
        }

        public bool IsFilterStatusApproved
        {
            get { return isFilterStatusApproved; }
            set
            {
                isFilterStatusApproved = value;

                if (isFilterStatusApproved)
                    FilterStatuss.Add("Approved");
                else
                    FilterStatuss.Remove("Approved");

                OnPropertyChanged();
                OnPropertyChanged(nameof(LicensesObservable));
            }
        }

        public bool IsFilterStatusOpened
        {
            get { return isFilterStatusOpened; }
            set
            {
                isFilterStatusOpened = value;

                if (isFilterStatusOpened)
                    FilterStatuss.Add("Opened");
                else
                    FilterStatuss.Remove("Opened");

                OnPropertyChanged();
                OnPropertyChanged(nameof(LicensesObservable));
            }
        }

        public bool IsFilterStatusExpired
        {
            get { return isFilterStatusExpired; }
            set
            {
                isFilterStatusExpired = value;

                if (isFilterStatusExpired)
                    FilterStatuss.Add("Expired");
                else
                    FilterStatuss.Remove("Expired");

                OnPropertyChanged();
                OnPropertyChanged(nameof(LicensesObservable));
            }
        }

        public List<string> FilterStatuss { get; set; }
        #endregion
        #region Command
        public ICommand EditDocumentCommand { get; set; }
        public ICommand AddDocumentCommand { get; set; }
        public ICommand EditLicenseCommand { get; set; }
        public ICommand AddLicenseCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand DeleteDocumentCommand { get; set; }
        public ICommand DeleteLicenseCommand { get; set; }
        public ICommand ApproveLicenseCommand { get; set; }
        #endregion

        public DashboardViewModel()
        {
            using (FileStream fs = new FileStream(Properties.Settings.Default.ClientSecretFileName, FileMode.Open))
            {
                dataProvider = new CopyRightPDFDataProvider(fs
                                        , Properties.Settings.Default.SpreadSheetId
                                        , Properties.Settings.Default.DocumentSheetName
                                        , Properties.Settings.Default.LicenseSheetName);

            }
            Documents = new List<DocumentModel>();
            FilterStatuss = new List<string>();

            IsDataChanged = false;
            IsFilterStatusRegistered = true;
            IsFilterStatusApproved = true;
            IsFilterStatusOpened = true;
            IsFilterStatusExpired = true;

            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));

            //for (int i = 0; i < 10; i++)
            //{
            //    var fileId = Guid.NewGuid().ToString();
            //    var licenses = new List<LicenseModel>();
            //    for (int j = 0; j < 10; j++)
            //    {
            //        licenses.Add(new LicenseModel
            //        {
            //            RowId = i,
            //            Status = "Opened",
            //            Password = $"File {i} - 123456",
            //            ActivatedDeviceMAC = "00-FF-4F-4D-20-9F\r\n18-C0-4D-4B-FD-2B\r\n00-50-56-C0-00-01\r\n00-50-56-C0-00-08\r\n00-50-56-C0-00-03",
            //            ActivatedOS = "Windows | Mac | Android | iOS",
            //            FileId = fileId,
            //            LastAccess = DateTime.Now,
            //            ExpireDate = DateTime.Now,
            //            MinVersion = "1.0",
            //            NumberOfActivatedDevice = 2,
            //            NumberOfLimitDevice = 1,
            //            PreventPrint = true,
            //            PreventSameOS = true,
            //            PreventScreenshot = false
            //        });
            //    }

            //    Documents.Add(new DocumentModel
            //    {
            //        FileId = fileId,
            //        FileName = $"File {i}",
            //        Description = $"Decription {i}",
            //        Licenses = new ObservableCollection<LicenseModel>(licenses),
            //        RowId = i
            //    });
            //}
            //DocumentsObservable = new ObservableCollection<DocumentModel>(Documents);

            LoadData();

            ReloadCommand = new RelayCommand<object>(p => { return true; }, p => LoadData());

            EditDocumentCommand = new RelayCommand<DocumentModel>((p) =>
            {
                if (SelectedDocument == null) return false;
                return true;
            }, p =>
            {
                if (SelectedDocument == null) return;

                DocumentInfoViewModel documentInfoViewModel = new DocumentInfoViewModel(SelectedDocument, false, dataProvider);
                DocumentInfoPage documentInfoPage = new DocumentInfoPage { DataContext = documentInfoViewModel };
                documentInfoPage.ShowDialog();

                if (documentInfoViewModel.ReturnDocument != null)
                {
                    //SelectedDocument.FileName = documentInfoViewModel.ReturnDocument.FileName;
                    //SelectedDocument.Description = documentInfoViewModel.ReturnDocument.FileName;

                    //IsDataChanged = true;
                    LoadData();
                }
            });

            AddDocumentCommand = new RelayCommand<DocumentModel>((p) =>
            {
                return true;
            }, p =>
            {
                //New not existed file id
                var newFileId = Guid.NewGuid().ToString();
                while (Documents.Where(x => x.FileId == newFileId).Any())
                {
                    newFileId = Guid.NewGuid().ToString();
                }
                //Add new document
                DocumentInfoViewModel documentInfoViewModel = new DocumentInfoViewModel(new DocumentModel
                {
                    FileId = newFileId,
                    IsDelete = false
                }, true, dataProvider);
                DocumentInfoPage documentInfoPage = new DocumentInfoPage { DataContext = documentInfoViewModel };
                documentInfoPage.ShowDialog();

                if (documentInfoViewModel.ReturnDocument != null)
                {
                    //var newDocument = new DocumentModel
                    //{
                    //    FileId = documentInfoViewModel.ReturnDocument.FileId,
                    //    FileName = documentInfoViewModel.ReturnDocument.FileName,
                    //    Description = documentInfoViewModel.ReturnDocument.Description,
                    //    Licenses = new ObservableCollection<LicenseModel>()
                    //};
                    //Documents.Add(newDocument);

                    //OnPropertyChanged(nameof(DocumentsObservable));
                    //SelectedDocument = newDocument;

                    //IsDataChanged = true;
                    LoadData();
                }
            });

            EditLicenseCommand = new RelayCommand<LicenseModel>((p) =>
            {
                if (SelectedDocument == null) return false;
                if (SelectedLicense == null) return false;
                return true;
            }, p =>
            {
                if (SelectedLicense == null) return;

                var license = selectedLicense.Clone();
                if (license.Status == "Registered")
                    SetInitValue(license);

                LicenseInfoViewModel licenseInfoViewModel = new LicenseInfoViewModel(license, false, dataProvider);
                LicenseInfoPage licenseInfoPage = new LicenseInfoPage { DataContext = licenseInfoViewModel };
                licenseInfoPage.ShowDialog();

                if (licenseInfoViewModel.ReturnLicense != null)
                {
                    LoadData();
                }
            });

            AddLicenseCommand = new RelayCommand<LicenseModel>((p) =>
            {
                if (SelectedDocument == null) return false;
                return true;
            }, p =>
            {
                LicenseInfoViewModel licenseInfoViewModel = new LicenseInfoViewModel(new LicenseModel
                {
                    FileId = SelectedDocument.FileId,
                    PreventPrint = true,
                    PreventSameOS = true,
                    PreventScreenshot = true,
                    Status = "Registered",
                    NumberOfLimitDevice = 1,
                    ExpireDayCount = 30,
                    MinVersion = "1.0.0",
                    LastAccess = DateTime.MinValue,
                    IsLocked = false,
                    IsDelete = false,
                }, true, dataProvider);
                LicenseInfoPage licenseInfoPage = new LicenseInfoPage { DataContext = licenseInfoViewModel };
                licenseInfoPage.ShowDialog();

                if (licenseInfoViewModel.ReturnLicense != null)
                {
                    LoadData();
                }
            });

            DeleteDocumentCommand = new RelayCommand<object>(p => { return true; }, p =>
            {
                if (SelectedDocument == null) return;

                var result = MessageBox.Show($"Delete document {SelectedDocument.FileId} and all license?", "Copyright PDF - Writer", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;

                //Documents.Remove(SelectedDocument);
                //OnPropertyChanged(nameof(DocumentsObservable));
                //IsDataChanged = true;
                DeleteDocument();
                LoadData();
            });

            DeleteLicenseCommand = new RelayCommand<object>(p => { return true; }, p =>
            {
                if (SelectedDocument == null) return;
                if (SelectedLicense == null) return;

                var result = MessageBox.Show($"Delete license for {SelectedLicense.RegisteredCustomerName}?", "Copyright PDF - Writer", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;

                //SelectedDocument.Licenses.Remove(SelectedLicense);
                //IsDataChanged = true;
                DeleteLicense();
                LoadData();
            });
            ApproveLicenseCommand = new RelayCommand<object>(p => { return true; }, p =>
            {
                if (SelectedLicense == null) return;

                var result = MessageBox.Show($"Approve license for {SelectedLicense.RegisteredCustomerName}?", "Copyright PDF - Writer", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;

                ApproveLicense();
                SendApproveMail();
                LoadData();
            });
        }

        private void LoadData()
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    Documents = dataProvider.GetDocument();
                    OnPropertyChanged(nameof(DocumentsObservable));
                }
                catch (Exception ex)
                {
                    MessageQueue.Enqueue(ex.Message);
                }

            }, "Loading");

            MessageQueue.Enqueue("Reload data completed");
        }

        private void ApproveLicense()
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    var license = SelectedLicense.Clone();

                    SetInitValue(license);
                    license.Status = "Approved";
                    dataProvider.ApproveLicense(license);

                    MessageQueue.Enqueue("Approved license");
                }
                catch (Exception ex)
                {
                    MessageQueue.Enqueue(ex.Message);
                }
            }, "Approving");
        }

        private void SendApproveMail()
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    var mailProvider = new MailProvider();
                    mailProvider.SendApproveMail(Properties.Settings.Default.Host,
                                                 Properties.Settings.Default.Port,
                                                 Properties.Settings.Default.Username,
                                                 Properties.Settings.Default.Password,
                                                 Properties.Settings.Default.Subject,
                                                 Properties.Settings.Default.Body,
                                                 SelectedLicense);
                    MessageQueue.Enqueue("Send mail completed");
                }
                catch (Exception ex)
                {
                    MessageQueue.Enqueue(ex.Message);
                }

            }, "Sending Mail");
        }

        private void DeleteDocument()
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    dataProvider.DeleteDocument(SelectedDocument);
                    MessageQueue.Enqueue("Delete document completed");
                }
                catch (Exception ex)
                {
                    MessageQueue.Enqueue(ex.Message);
                }

            }, "Deleting License");
        }

        private void DeleteLicense()
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    dataProvider.DeleteLicense(SelectedLicense);
                    MessageQueue.Enqueue("Delete license completed");
                }
                catch (Exception ex)
                {
                    MessageQueue.Enqueue(ex.Message);
                }

            }, "Deleting License");
        }

        void SetInitValue(LicenseModel model)
        {
            model.Password = model.RegisteredPhoneNumber;
            model.CustomerName = model.RegisteredCustomerName;
            model.NumberOfLimitDevice = 1;
            model.PreventPrint = true;
            model.PreventSameOS = true;
            model.PreventScreenshot = true;
            model.MinVersion = "1.0.0";
            model.ExpireDayCount = 30;
        }
    }
}
