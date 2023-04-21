using CopyRightPDF.Core;
using CopyRightPDF.Core.Models;
using CopyRightPDF.Utilities;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;

namespace CopyRightPDF.ViewModels
{
    public class LicenseInfoViewModel : BaseViewModel, ICloseable
    {
        public event EventHandler<EventArgs> RequestClose;
        #region Variable
        // Private fields
        private string _registeredEmail;
        private string _registeredPhoneNumber;
        private string _registeredCustomerName;
        private string _registeredFileName;
        private string _fileId;
        private bool _isAddNew;
        private int _rowId;
        private string _status;
        private string _customerName;
        private string _password;
        private int _numberOfLimitDevice;
        private bool _preventPrint;
        private bool _preventSameOS;
        private bool _preventScreenshot;
        private DateTime? _activatedDate;
        private int _numberOfActivatedDevice;
        private string _activatedDeviceMAC;
        private string _activatedOS;
        private DateTime? _lastAccess;
        private string _minVersion;
        private int? _expireDayCount;
        private DateTime? _expireDate;
        private DateTime? _specifiedExpireDate;
        private bool _isNeverExpire;
        private bool _isSpecifiedDate;
        private bool _isExpireDayCount;
        #endregion

        #region Properties
        public string RegisteredEmail
        {
            get { return _registeredEmail; }
            set { _registeredEmail = value; OnPropertyChanged(nameof(RegisteredEmail)); }
        }
        public string RegisteredPhoneNumber
        {
            get { return _registeredPhoneNumber; }
            set { _registeredPhoneNumber = value; OnPropertyChanged(nameof(RegisteredPhoneNumber)); }
        }
        public string RegisteredCustomerName
        {
            get { return _registeredCustomerName; }
            set { _registeredCustomerName = value; OnPropertyChanged(nameof(RegisteredCustomerName)); }
        }
        public string RegisteredFileName
        {
            get { return _registeredFileName; }
            set { _registeredFileName = value; OnPropertyChanged(nameof(RegisteredFileName)); }
        }

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

        public DateTime? ActivatedDate
        {
            get { return _activatedDate; }
            set { _activatedDate = value; OnPropertyChanged(nameof(ActivatedDate)); }
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

        public int? ExpireDayCount
        {
            get { return _expireDayCount; }
            set { _expireDayCount = value; OnPropertyChanged(nameof(ExpireDayCount)); }
        }

        public DateTime? ExpireDate
        {
            get => _expireDate;
            set { _expireDate = value; OnPropertyChanged(nameof(ExpireDate)); }
        }

        public DateTime? SpecifiedExpireDate
        {
            get => _specifiedExpireDate;
            set { _specifiedExpireDate = value; OnPropertyChanged(nameof(SpecifiedExpireDate)); }
        }

        public bool IsNeverExpire
        {
            get { return _isNeverExpire; }
            set
            {
                _isNeverExpire = value;
                OnPropertyChanged(nameof(IsNeverExpire));

                if (_isNeverExpire)
                {
                    IsExpireDayCount = false;
                    IsSpecifiedDate = false;
                    ExpireDayCount = null;
                    SpecifiedExpireDate = null;
                }
            }
        }

        public bool IsSpecifiedDate
        {
            get { return _isSpecifiedDate; }
            set
            {
                _isSpecifiedDate = value;
                OnPropertyChanged(nameof(IsSpecifiedDate));

                if (_isSpecifiedDate)
                {
                    IsExpireDayCount = false;
                    IsNeverExpire = false;
                    ExpireDayCount = null;
                    SpecifiedExpireDate = DateTime.Now.AddDays(30);
                }
            }
        }

        public bool IsExpireDayCount
        {
            get { return _isExpireDayCount; }
            set
            {
                _isExpireDayCount = value;
                OnPropertyChanged(nameof(IsExpireDayCount));

                if (_isExpireDayCount)
                {
                    IsNeverExpire = false;
                    IsSpecifiedDate = false;
                    SpecifiedExpireDate = null;
                    ExpireDayCount = 30;
                }
            }
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
        public CopyRightPDFDataProvider DataProvider { get; set; }
        public string OKButtonContent
        {
            get { return String.IsNullOrEmpty(Status) || Status == "Registered" ? "Approve and Send mail" : "Save"; }
        }
        #endregion

        #region Command
        public ICommand ReloadCommand { get; set; }
        public ICommand OKCommand { get; set; }
        public ICommand ResendMailCommand { get; set; }

        public LicenseInfoViewModel() { }
        #endregion
        public LicenseInfoViewModel(LicenseModel license, bool isAddNew, CopyRightPDFDataProvider dataProvider)
        {
            InputLicense = license;
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));
            ErrorMessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(3));
            ReturnLicense = null;
            IsAddNew = isAddNew;
            DataProvider = dataProvider;

            Init(license);

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

                Init(license);

                MessageQueue.Enqueue("Reloaded");
            });

            OKCommand = new RelayCommand<object>(p =>
            {
                return true;
            }, p =>
            {
                var errorMessage = CheckInput();
                if (!String.IsNullOrEmpty(errorMessage))
                {
                    MessageQueue.Enqueue(errorMessage);
                    return;
                }

                var result = MessageBox.Show("Save changes and approve license?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;

                var newLicense = new LicenseModel()
                {
                    ActivatedDate = ActivatedDate,
                    NumberOfActivatedDevice = NumberOfActivatedDevice,
                    ActivatedDeviceMAC = ActivatedDeviceMAC,
                    ActivatedOS = ActivatedOS,
                    LastAccess = LastAccess,
                    IsLocked = false,
                    CustomerName = RegisteredCustomerName,
                    ExpireDate = ExpireDate,
                    ExpireDayCount = ExpireDayCount,
                    FileId = FileId,
                    MinVersion = MinVersion,
                    NumberOfLimitDevice = NumberOfLimitDevice,
                    Password = RegisteredPhoneNumber,
                    PreventPrint = PreventPrint,
                    PreventSameOS = PreventSameOS,
                    PreventScreenshot = PreventScreenshot,
                    RegisteredCustomerName = RegisteredCustomerName,
                    RegisteredEmail = RegisteredEmail,
                    RegisteredFileName = RegisteredFileName,
                    RegisteredPhoneNumber = RegisteredPhoneNumber,
                    RowId = RowId,
                    SpecifiedExpireDate = SpecifiedExpireDate,
                    Status = String.IsNullOrEmpty(Status) || Status == "Registered" ? "Approved" : Status,
                    IsDelete = false,
                };

                //Approve license
                ApproveLicense(newLicense);

                //Send mail here
                if (String.IsNullOrEmpty(Status) || Status == "Registered")
                    SendApproveMail(newLicense);

                ReturnLicense = newLicense;

                RequestClose(this, EventArgs.Empty);
            });

            ResendMailCommand = new RelayCommand<object>(p =>
            {
                if (String.IsNullOrEmpty(Status) || Status == "Registered")
                    return false;

                return true;
            }, p =>
            {
                SendApproveMail(license);
            });
        }

        private string CheckInput()
        {
            if (String.IsNullOrEmpty(RegisteredCustomerName))
                return "Customer name must be input";

            if (String.IsNullOrEmpty(RegisteredPhoneNumber))
                return "Phone number must be input";

            if (String.IsNullOrEmpty(RegisteredEmail))
                return "Email must be input";

            if (String.IsNullOrEmpty(MinVersion))
                return "Min version must be input";

            return "";
        }

        private void Init(LicenseModel license)
        {
            RegisteredCustomerName = license.RegisteredCustomerName;
            RegisteredEmail = license.RegisteredEmail;
            RegisteredFileName = license.RegisteredFileName;
            RegisteredPhoneNumber = license.RegisteredPhoneNumber;
            Status = license.Status;
            FileId = license.FileId;
            CustomerName = license.CustomerName;
            Password = license.Password;
            NumberOfLimitDevice = license.NumberOfLimitDevice;
            PreventPrint = license.PreventPrint;
            PreventSameOS = license.PreventSameOS;
            PreventScreenshot = license.PreventScreenshot;
            ActivatedDate = license.ActivatedDate;
            NumberOfActivatedDevice = license.NumberOfActivatedDevice;
            ActivatedDeviceMAC = license.ActivatedDeviceMAC;
            ActivatedOS = license.ActivatedOS;
            LastAccess = license.LastAccess;
            MinVersion = license.MinVersion;
            ExpireDate = license.ExpireDate;
            SpecifiedExpireDate = license.SpecifiedExpireDate;
            ExpireDayCount = license.ExpireDayCount;
            if (SpecifiedExpireDate != null)
                IsSpecifiedDate = true;
            else if (ExpireDayCount != null && ExpireDayCount != 0)
                IsExpireDayCount = true;
            else
                IsNeverExpire = true;
            RowId = license.RowId;
        }
        private void ApproveLicense(LicenseModel license)
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    if (IsAddNew)
                    {
                        DataProvider.AddLicense(license);
                    }
                    else
                    {
                        DataProvider.ApproveLicense(license);
                    }
                    MessageQueue.Enqueue("Approved license");
                }
                catch (Exception ex)
                {
                    ErrorMessageQueue.Enqueue(ex.Message);
                    return;
                }
            }, "Approving");
        }
        private void SendApproveMail(LicenseModel license)
        {
            WaitingForExecUtility.Instance.DoWork(() =>
            {
                try
                {
                    var mailProvider = new MailProvider();
                    mailProvider.SendApproveMail(Properties.Settings.Default.Host,
                                                 Properties.Settings.Default.Port,
                                                 Properties.Settings.Default.Username,
                                                 Properties.Settings.Default.DisplayName,
                                                 Properties.Settings.Default.Password,
                                                 Properties.Settings.Default.Subject,
                                                 Properties.Settings.Default.Body,
                                                 license);
                    MessageQueue.Enqueue("Send mail completed");
                }
                catch (Exception ex)
                {
                    ErrorMessageQueue.Enqueue(ex.Message);
                }

            }, "Sending Mail");
        }
    }
}
