using CopyRightPDF.Core.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CopyRightPDF.ViewModels
{
    public class LicenseInfoViewModel : BaseViewModel, ICloseable
    {
        public event EventHandler<EventArgs> RequestClose;
        #region Variable
        // Private fields
        private bool _isAddNew;
        private int _rowId;
        private string _status;
        private string _fileId;
        private string _customerName;
        private string _password;
        private int _numberOfLimitDevice;
        private bool _preventPrint;
        private bool _preventSameOS;
        private bool _preventScreenshot;
        private int _numberOfActivatedDevice;
        private string _activatedDeviceMAC;
        private string _activatedOS;
        private DateTime? _lastAccess;
        private string _minVersion;
        private DateTime? _expireDate;
        #endregion

        #region Properties
        // Public properties
        public int RowId
        {
            get => _rowId;
            set { _rowId = value; OnPropertyChanged(nameof(RowId)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }
        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); }
        }

        public string FileId
        {
            get => _fileId;
            set { _fileId = value; OnPropertyChanged(nameof(FileId)); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        public int NumberOfLimitDevice
        {
            get => _numberOfLimitDevice;
            set { _numberOfLimitDevice = value; OnPropertyChanged(nameof(NumberOfLimitDevice)); }
        }

        public bool PreventPrint
        {
            get => _preventPrint;
            set { _preventPrint = value; OnPropertyChanged(nameof(PreventPrint)); }
        }

        public bool PreventSameOS
        {
            get => _preventSameOS;
            set { _preventSameOS = value; OnPropertyChanged(nameof(PreventSameOS)); }
        }

        public bool PreventScreenshot
        {
            get => _preventScreenshot;
            set { _preventScreenshot = value; OnPropertyChanged(nameof(PreventScreenshot)); }
        }

        public int NumberOfActivatedDevice
        {
            get => _numberOfActivatedDevice;
            set { _numberOfActivatedDevice = value; OnPropertyChanged(nameof(NumberOfActivatedDevice)); }
        }

        public string ActivatedDeviceMAC
        {
            get => _activatedDeviceMAC;
            set { _activatedDeviceMAC = value; OnPropertyChanged(nameof(ActivatedDeviceMAC)); }
        }

        public string ActivatedOS
        {
            get => _activatedOS;
            set { _activatedOS = value; OnPropertyChanged(nameof(ActivatedOS)); }
        }

        public DateTime? LastAccess
        {
            get => _lastAccess;
            set { _lastAccess = value; OnPropertyChanged(nameof(LastAccess)); }
        }

        public string MinVersion
        {
            get => _minVersion;
            set { _minVersion = value; OnPropertyChanged(nameof(MinVersion)); }
        }

        public DateTime? ExpireDate
        {
            get => _expireDate;
            set { _expireDate = value; OnPropertyChanged(nameof(ExpireDate)); }
        }

        public LicenseModel ReturnLicense { get; set; }
        public bool IsAddNew
        {
            get { return _isAddNew; }
            set { _isAddNew = value; OnPropertyChanged(); OnPropertyChanged(nameof(WindowTitle)); }
        }
        public string WindowTitle
        {
            get { return IsAddNew ? "New license" : $"Edit license for {CustomerName}"; }
        }
        public LicenseModel InputLicense { get; set; }
        public bool IsEdited
        {
            get
            {
                return Status != InputLicense.Status
                    || FileId != InputLicense.FileId
                    || CustomerName != InputLicense.CustomerName
                    || Password != InputLicense.Password
                    || NumberOfLimitDevice != InputLicense.NumberOfLimitDevice
                    || PreventPrint != InputLicense.PreventPrint
                    || PreventSameOS != InputLicense.PreventSameOS
                    || PreventScreenshot != InputLicense.PreventScreenshot
                    || NumberOfActivatedDevice != InputLicense.NumberOfActivatedDevice
                    || ActivatedDeviceMAC != InputLicense.ActivatedDeviceMAC
                    || ActivatedOS != InputLicense.ActivatedOS
                    || MinVersion != InputLicense.MinVersion
                    || ExpireDate != InputLicense.ExpireDate;
            }
        }
        #endregion

        #region Command
        public ICommand ReloadCommand { get; set; }
        public ICommand OKCommand { get; set; }
        #endregion  

        public LicenseInfoViewModel() { }
        public LicenseInfoViewModel(LicenseModel license, bool isAddNew)
        {
            InputLicense = license;
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
            ReturnLicense = null;
            IsAddNew = isAddNew;

            Status = license.Status;
            FileId = license.FileId;
            CustomerName = license.CustomerName;
            Password = license.Password;
            NumberOfLimitDevice = license.NumberOfLimitDevice;
            PreventPrint = license.PreventPrint;
            PreventSameOS = license.PreventSameOS;
            PreventScreenshot = license.PreventScreenshot;
            NumberOfActivatedDevice = license.NumberOfActivatedDevice;
            ActivatedDeviceMAC = license.ActivatedDeviceMAC;
            ActivatedOS = license.ActivatedOS;
            LastAccess = license.LastAccess;
            MinVersion = license.MinVersion;
            ExpireDate = license.ExpireDate;

            ReloadCommand = new RelayCommand<object>(p =>
            {
                return true;
            }, p =>
            {
                if (IsEdited)
                {
                    var result = MessageBox.Show(App.Current.MainWindow, "Discard changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) return;
                }

                Status = license.Status;
                FileId = license.FileId;
                CustomerName = license.CustomerName;
                Password = license.Password;
                NumberOfLimitDevice = license.NumberOfLimitDevice;
                PreventPrint = license.PreventPrint;
                PreventSameOS = license.PreventSameOS;
                PreventScreenshot = license.PreventScreenshot;
                NumberOfActivatedDevice = license.NumberOfActivatedDevice;
                ActivatedDeviceMAC = license.ActivatedDeviceMAC;
                ActivatedOS = license.ActivatedOS;
                LastAccess = license.LastAccess;
                MinVersion = license.MinVersion;
                ExpireDate = license.ExpireDate;

                MessageQueue.Enqueue("Reloaded");
            });

            OKCommand = new RelayCommand<object>(p =>
            {
                return true;
            }, p =>
            {
                var result = MessageBox.Show(App.Current.MainWindow, "Save changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;

                ReturnLicense = new LicenseModel
                {
                    Status = Status,
                    FileId = FileId,
                    CustomerName = CustomerName,
                    Password = Password,
                    NumberOfLimitDevice = NumberOfLimitDevice,
                    PreventPrint = PreventPrint,
                    PreventSameOS = PreventSameOS,
                    PreventScreenshot = PreventScreenshot,
                    NumberOfActivatedDevice = NumberOfActivatedDevice,
                    ActivatedDeviceMAC = ActivatedDeviceMAC,
                    ActivatedOS = ActivatedOS,
                    LastAccess = LastAccess,
                    MinVersion = MinVersion,
                    ExpireDate = ExpireDate
                };

                RequestClose(this, EventArgs.Empty);
            });
        }
    }
}
