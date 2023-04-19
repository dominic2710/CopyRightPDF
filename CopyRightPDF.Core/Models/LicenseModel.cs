using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyRightPDF.Core.Models
{
    public class LicenseModel : BaseModel
    {
        // Private fields
        private string _registeredEmail;
        private string _registeredPhoneNumber;
        private string _registeredCustomerName;
        private string _registeredFileName;
        private int _rowId;
        private string _status;
        private string _fileId;
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
        private DateTime? _expireDate;
        private DateTime? _specifiedExpireDate;
        private int? _expireDayCount;
        private bool _isLocked;
        private bool _isDelete;
        // Public properties
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

        public string FileId
        {
            get => _fileId;
            set { _fileId = value; OnPropertyChanged(nameof(FileId)); }
        }

        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(nameof(CustomerName)); }
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

        public DateTime? SpecifiedExpireDate
        {
            get { return _specifiedExpireDate; }
            set
            {
                _specifiedExpireDate = value; OnPropertyChanged(nameof(SpecifiedExpireDate));
            }
        }

        public int? ExpireDayCount
        {
            get { return _expireDayCount; }
            set
            {
                _expireDayCount = value; OnPropertyChanged(nameof(ExpireDayCount));
            }
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

        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; OnPropertyChanged(nameof(IsLocked)); }
        }
        public bool IsDelete
        {
            get { return _isDelete; }
            set { _isDelete = value; OnPropertyChanged(nameof(IsDelete)); }
        }
        public IList<object> ToList
        {
            get
            {
                return new List<object>() { RegisteredEmail,
                                            RegisteredPhoneNumber,
                                            RegisteredCustomerName,
                                            RegisteredFileName,
                                            FileId,
                                            "=ROW() - 1",
                                            CustomerName,
                                            Password,
                                            NumberOfLimitDevice,
                                            PreventPrint,
                                            PreventSameOS,
                                            PreventScreenshot,
                                            MinVersion,
                                            SpecifiedExpireDate,
                                            ExpireDayCount,
                                            ExpireDate,
                                            Status,
                                            ActivatedDate,
                                            NumberOfActivatedDevice,
                                            ActivatedDeviceMAC,
                                            ActivatedOS,
                                            LastAccess,
                                            IsLocked,
                                            IsDelete,
                                        };
            }
        }

        public IList<object> ToListForReaderUpdate
        {
            get
            {
                return new List<object>() { ExpireDate,
                                            Status,
                                            ActivatedDate,
                                            NumberOfActivatedDevice,
                                            ActivatedDeviceMAC,
                                            ActivatedOS,
                                            LastAccess,
                                            IsLocked
                                        };
            }
        }

        public LicenseModel Clone()
        {
            return new LicenseModel()
            {
                ActivatedDate = ActivatedDate,
                NumberOfActivatedDevice = NumberOfActivatedDevice,
                ActivatedDeviceMAC = ActivatedOS,
                ActivatedOS = ActivatedOS,
                LastAccess = LastAccess,
                IsLocked = IsLocked,
                CustomerName = CustomerName,
                ExpireDate = ExpireDate,
                ExpireDayCount = ExpireDayCount,
                FileId = FileId,
                MinVersion = MinVersion,
                NumberOfLimitDevice = NumberOfLimitDevice,
                Password = Password,
                PreventPrint = PreventPrint,
                PreventSameOS = PreventSameOS,
                PreventScreenshot = PreventScreenshot,
                RegisteredCustomerName = RegisteredCustomerName,
                RegisteredEmail = RegisteredEmail,
                RegisteredFileName = RegisteredFileName,
                RegisteredPhoneNumber = RegisteredPhoneNumber,
                RowId = RowId,
                SpecifiedExpireDate = SpecifiedExpireDate,
                Status = Status,
                IsDelete = IsDelete,
            };
        }
    }

    public enum LicenseModelEnum
    {
        RegisteredEmail = 0,
        RegisteredPhoneNumber,
        RegisteredCustomerName,
        RegisteredFileName,
        FileId,
        RowId,
        CustomerName,
        Password,
        NumberOfLimitDevice,
        PreventPrint,
        PreventSameOS,
        PreventScreenshot,
        MinVersion,
        SpecifiedExpireDate,
        ExpireDayCount,
        ExpireDate,
        Status,
        ActivatedDate,
        NumberOfActivatedDevice,
        ActivatedDeviceMAC,
        ActivatedOS,
        LastAccess,
        IsLocked,
        IsDelete
    }
}
