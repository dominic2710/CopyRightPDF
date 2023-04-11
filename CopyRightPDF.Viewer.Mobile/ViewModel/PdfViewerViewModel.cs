using CopyRightPDF.Core;
using CopyRightPDF.Viewer.Mobile.ViewModel;
using Ionic.Zip;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows.Input;

namespace CopyRightPDF.Viewer.Mobile
{
    internal class PdfViewerViewModel : BaseViewModel
    {
        const string ZIP_PASSWORD = "1ZpVSuHWlt50fyLp0jQn59GotXUB_XJ5WyVDt7k";
        const string ENCRYPT_KEY = "b14ca5898a4e4133bbce2ea2315a1916";
        const string DATA_FILE_NAME = "data.dat";
        const string SERVER_FILE_NAME = "server.dat";
        const string INFO_FILE_NAME = "info.dat";
        const string SEPARATOR = "\r\n";


        private int _pageNumber;
        private ICommand _openFileCommand;
        private ICommand _zoomInCommand;
        private ICommand _zoomOutCommand;
        private double _currentZoom = 1;
        private double _minZoom = 0.25;
        private double _maxZoom = 4;
        private bool _canZoomIn = true;
        private bool _canZoomOut = true;
        private bool _canGoToPreviousPage = true;
        private bool _canGoToNextPage = true;
        private bool _showPasswordDialog = false;
        private Stream _documentStream;
        private int _pageCount;

        public string CurrentOS { get; set; }
        public string CurrentVersion { get; set; }

        /// <summary>
        /// Gets or set the value that determines whether zoom in is allowed.
        /// </summary>
        public bool CanZoomIn
        {
            get => _canZoomIn;
            set
            {
                _canZoomIn = value;
                OnPropertyChanged("CanZoomIn");
            }
        }

        /// <summary>
        /// Gets or sets the value to show or hide the password dialog.
        /// </summary>
        public bool ShowPasswordDialog
        {
            get => _showPasswordDialog;
            set
            {
                _showPasswordDialog = value;
                OnPropertyChanged("ShowPasswordDialog");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether zoom out is allowed.
        /// </summary>
        public bool CanZoomOut
        {
            get => _canZoomOut;
            set
            {
                _canZoomOut = value;
                OnPropertyChanged("CanZoomOut");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether go to previous page is allowed.
        /// </summary>
        public bool CanGoToPreviousPage
        {
            get => _canGoToPreviousPage;
            set
            {
                _canGoToPreviousPage = value;
                OnPropertyChanged("CanGoToPreviousPage");
            }
        }

        /// <summary>
        /// Gets or sets the value that determines whether go to next page is allowed.
        /// </summary>
        public bool CanGoToNextPage
        {
            get => _canGoToNextPage;
            set
            {
                _canGoToNextPage = value;
                OnPropertyChanged("CanGoToNextPage");
            }
        }

        /// <summary>
        /// Gets or sets the current zoom value.
        /// </summary>
        public double CurrentZoom
        {
            get => _currentZoom;
            set
            {
                _currentZoom = value;
                OnPropertyChanged("CurrentZoom");
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                _pageNumber = value;
                OnPropertyChanged("PageNumber");
                ValidatePageNumber();
            }
        }

        /// <summary>
        /// Gets or sets the minimum zoom allowed.
        /// </summary>
        public double MinZoom
        {
            get => _minZoom;
            set
            {
                _minZoom = value;
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Gets or sets the maximum zoom allowed.
        /// </summary>
        public double MaxZoom
        {
            get => _maxZoom;
            set
            {
                _maxZoom = value;
                ValidateZoomChange();
            }
        }

        /// <summary>
        /// Stores the PDF document stream.
        /// </summary>
        public Stream PdfDocumentStream
        {
            get => _documentStream;
            set
            {
                _documentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }

        /// <summary>
        /// Stores the total page count information.
        /// </summary>
        public int PageCount
        {
            get => _pageCount;
            set
            {
                _pageCount = value;
                OnPropertyChanged("PageCount");
            }
        }

        /// <summary>
        /// Gets the command to browse file in the disk.
        /// </summary>
        public ICommand OpenFileCommand => _openFileCommand;

        /// <summary>
        /// Gets or sets the command to increase the zoom.
        /// </summary>
        public ICommand ZoomInCommand => _zoomInCommand;

        /// <summary>
        /// Gets or sets the command to decrease the zoom.
        /// </summary>
        public ICommand ZoomOutCommand => _zoomOutCommand;

        public PdfViewerViewModel()
        {
            _openFileCommand = new Command<object>(OpenFile);
            _zoomInCommand = new Command<object>(ZoomIn);
            _zoomOutCommand = new Command<object>(ZoomOut);

#if WINDOWS
            CurrentOS = "Windows";
#elif ANDROID
            CurrentOS= "Android";
#elif MACCATALYST
            CurrentOS = "MacOS";
#elif IOS
            CurrentOS = "iOS";
#endif

            CurrentVersion = "1.0.0";
        }

        /// <summary>
        /// Validates the current zoom with minimum/maximum limit and determines whether further zoom is possible.
        /// </summary>
        public void ValidateZoomChange()
        {
            if (_currentZoom > _minZoom && _currentZoom < _maxZoom)
            {
                CanZoomIn = true;
                CanZoomOut = true;
            }
            else if (_currentZoom <= _minZoom && _currentZoom >= _maxZoom)
            {
                CanZoomIn = false;
                CanZoomOut = false;
            }
            else if (_currentZoom <= _minZoom)
                CanZoomOut = false;
            else if (_currentZoom >= _maxZoom)
                CanZoomIn = false;
        }

        /// <summary>
        /// Validates the current page number with the tolal page count and determines whether further page navigation is possible.
        /// </summary>
        public void ValidatePageNumber()
        {
            if (PageCount <= 1)
            {
                CanGoToPreviousPage = false;
                CanGoToNextPage = false;
            }
            if (_pageNumber <= 1)
                CanGoToPreviousPage = false;
            else
                CanGoToPreviousPage = true;
            if (_pageNumber >= PageCount)
                CanGoToNextPage = false;
            else
                CanGoToNextPage = true;
        }

        /// <summary>
        /// Increases the current zoom by 25%.
        /// </summary>
        void ZoomIn(object commandParameter)
        {
            CurrentZoom += 0.25;
        }

        /// <summary>
        /// Decreases the current zoom by 25%.
        /// </summary>
        void ZoomOut(object commandParameter)
        {
            CurrentZoom -= 0.25;
        }

        private bool showRunning;
        public bool ShowRunning
        {
            get { return showRunning; }
            set
            {
                showRunning = value;
                OnPropertyChanged();
            }
        }

        async void OpenFile(object commandParameter)
        {
            //Create file picker with file type as PDF.
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                        { DevicePlatform.iOS, new[] { "public.crpdf" } },
                        { DevicePlatform.Android, new[] { "crpdf"} },
                        { DevicePlatform.WinUI, new[] { "crpdf" } },
                        { DevicePlatform.MacCatalyst, new[] { "crpdf" } },
                    });

            //Provide picker title if required.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType,
            };
            await PickAndShow(options);
        }

        /// <summary>
        /// Picks the file from local storage and store as stream.
        /// </summary>
        public async Task PickAndShow(PickOptions options)
        {
            try
            {
                //Pick the file from local storage.
                var result = await FilePicker.Default.PickAsync(options);
                if (result == null) return;
                if (System.IO.Path.GetExtension(result.FileName) != "crpdf")
                {
                    await App.Current.MainPage.DisplayAlert("Your *.CrPdf file was broken or invalid", "Please contact admin for new file", "OK");
                    return;
                }
                ShowRunning = true;

                using (ZipFile zipFile = ZipFile.Read(result.FullPath))
                {
                    zipFile.Password = ZIP_PASSWORD;

                    var info = GetFile(zipFile, "info.dat");
                    if (info == null) return;

                    var documentId = Encoding.UTF8.GetString(info).Split(SEPARATOR, StringSplitOptions.None)[0];
                    var spreadSheetId = Encoding.UTF8.GetString(info).Split(SEPARATOR, StringSplitOptions.None)[1];
                    var licenseSheetName = Encoding.UTF8.GetString(info).Split(SEPARATOR, StringSplitOptions.None)[2];

                    var server = GetFile(zipFile, "server.dat");
                    if (server == null) return;
                    var serverFileStream = new MemoryStream(server);

                    var dataProvider = new CopyRightPDFDataProvider(serverFileStream, spreadSheetId, "", licenseSheetName);

                    bool isUnlocked = false;
                    while (!isUnlocked)
                    {
                        var enteredPassword = await App.Current.MainPage.DisplayPromptAsync("Your file was protected", "Enter password to unlock file");

                        if (enteredPassword == null)
                        {
                            ShowRunning = false;
                            return;
                        }

                        var license = dataProvider.GetLicense(documentId, enteredPassword);
                        if (license == null)
                        {
                            await App.Current.MainPage.DisplayAlert("License is not exist or invalid!", "Re-input or contact admin for password", "OK");
                            continue;
                        }

                        //If another client verifying
                        int retryCount = 1;
                        int maxRetryCount = 12;
                        while (license.IsLocked)
                        {
                            retryCount++;
                            if (retryCount > maxRetryCount)
                            {
                                await App.Current.MainPage.DisplayAlert("Request timeout", "Please try again later", "OK");
                                break;
                            }

                            Thread.Sleep(500);
                            license = dataProvider.GetLicense(documentId, enteredPassword);
                        }
                        if (license.IsLocked) continue;

                        //Update status to verifying
                        license.IsLocked = true;
                        dataProvider.UpdateLicenseLockStatus(license);

                        //Check Reader version
                        if (String.Compare(CurrentVersion, license.MinVersion) < 0)
                        {
                            await App.Current.MainPage.DisplayAlert("Your reader is out of update", "Please contact admin for new version", "OK");
                            continue;
                        }

                        //Check Expiration
                        if (license.ExpireDate < DateTime.Now)
                        {
                            await App.Current.MainPage.DisplayAlert("Your license was expired!", "Please contact admin for new license", "OK");

                            license.LastAccess = DateTime.Now;
                            license.Status = "Expired";
                            license.IsLocked = false;
                            dataProvider.UpdateLicenseAccessInfo(license);

                            continue;
                        }

                        //Check MAC Address
                        var localMacAddress = GetAllDeviceMacAddress();
                        var storedDeviceMAC = license.ActivatedDeviceMAC.Split(SEPARATOR, StringSplitOptions.None).ToList<string>();

                        var exist = IsAllowOpen(localMacAddress, storedDeviceMAC);
                        if (!exist)
                        {
                            if (license.NumberOfLimitDevice > license.NumberOfActivatedDevice)
                            {
                                //Check same OS
                                if (license.ActivatedOS.Contains(CurrentOS))
                                {
                                    if (license.PreventSameOS)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Copyright PDF", "File cannot open on this device", "OK");
                                        continue;
                                    }
                                }
                                else
                                {
                                    //Update ActivatedOS
                                    if (String.IsNullOrEmpty(license.ActivatedOS))
                                        license.ActivatedOS = $"{CurrentOS}";
                                    else
                                        license.ActivatedOS += $" | {CurrentOS}";
                                }

                                storedDeviceMAC.AddRange(localMacAddress);
                                storedDeviceMAC.Remove("");
                                storedDeviceMAC = storedDeviceMAC.Distinct().ToList();

                                license.Status = "Opened";
                                license.ActivatedDeviceMAC = String.Join("\r\n", storedDeviceMAC);
                                license.NumberOfActivatedDevice = license.NumberOfActivatedDevice + 1;
                                license.LastAccess = DateTime.Now;
                                license.IsLocked = false;
                                dataProvider.UpdateLicenseAccessInfo(license);
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Copyright PDF", "File cannot open on this device", "OK");
                                continue;
                            }
                        }
                        else
                        {
                            license.Status = "Opened";
                            license.LastAccess = DateTime.Now;
                            license.IsLocked = false;
                            dataProvider.UpdateLicenseAccessInfo(license);
                        }

                        var pdfFile = GetFile(zipFile, "data.dat");
                        var ms = new MemoryStream(pdfFile);

                        PdfDocumentStream = ms;
                        isUnlocked = true;
                    }
                }
                ShowRunning = false;
            }
            catch (ZipException ex)
            {
                await App.Current.MainPage.DisplayAlert("Your *.CrPdf file was broken or invalid", "Please contact admin for new file", "OK");
            }
            catch (Exception ex)
            {
                //Display error when file picker failed to open files.
                string message;
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                    message = ex.Message;
                else
                    message = "File open failed.";
                Application.Current?.MainPage?.DisplayAlert("Error", message, "OK");
            }
            finally
            {
                ShowRunning = false;
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
                var decryptedFileByte = AesOperation.GetInstance.Decrypt(ms.ToArray(), ENCRYPT_KEY);
                return decryptedFileByte;
            }
        }

        private List<string> GetAllDeviceMacAddress()
        {
            var result = new List<string>();

#if WINDOWS || MACCATALYST
            var networkInterface = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var network in networkInterface)
            {
                var macAddress = BitConverter.ToString(network.GetPhysicalAddress().GetAddressBytes())?.Trim();
                if (macAddress != null && macAddress != "")
                    result.Add(macAddress);
            }
#endif

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