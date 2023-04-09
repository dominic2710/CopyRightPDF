using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CopyrightPDF.Mobile.ViewModels
{
    public class CreatePDFViewModel : BaseViewModel
    {
        #region Variable
        private string inputFile;
        private string outputFile;
        private string clientSecret;
        private string sheetId;
        private string password;
        private int numberOfActiveDevice;
        private DateTime expireDate;
        private bool preventSameOS;
        private bool preventScreenShot;
        private bool preventPrint;
        #endregion

        #region Property
        public string InputFile
        {
            get { return inputFile; }
            set { inputFile = value; OnPropertyChanged(); }
        }
        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; OnPropertyChanged(); }
        }
        public string ClientSecret
        {
            get { return clientSecret; }
            set { clientSecret = value; OnPropertyChanged(); }
        }
        public string SheetId
        {
            get { return sheetId; }
            set { sheetId = value; OnPropertyChanged(); }
        }
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }
        public int NumberOfActiveDevice
        {
            get { return numberOfActiveDevice; }
            set { numberOfActiveDevice = value; OnPropertyChanged(); }
        }
        public DateTime ExpireDate
        {
            get { return expireDate; }
            set { expireDate = value; OnPropertyChanged(); }
        }
        public bool PreventSameOS
        {
            get { return preventSameOS; }
            set { preventSameOS = value; OnPropertyChanged(); }
        }
        public bool PreventScreenShot
        {
            get { return preventScreenShot; }
            set { preventScreenShot = value; OnPropertyChanged(); }
        }
        public bool PreventPrint
        {
            get { return preventPrint; }
            set { preventPrint = value; OnPropertyChanged(); }
        }
        #endregion

        public CreatePDFViewModel()
        {
            InputFile = "input path";
            OutputFile = "output path";
            ClientSecret = "client_secret.json";
            SheetId = "sheet id";
            Password = "Abc12345";
            NumberOfActiveDevice = 2;
            ExpireDate = DateTime.Now.AddMonths(1);
            PreventSameOS = true;
            PreventScreenShot = true;
            PreventPrint = true;
        }
    }
}
