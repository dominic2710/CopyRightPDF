using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CopyRightPDF.Pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Window
    {
        public DashboardPage()
        {
            InitializeComponent();

            string errorMessage = String.Empty;
            string missingParam = String.Empty;

            if (!System.IO.File.Exists("CopyRightPDF.exe.config"))
                errorMessage = "CopyRightPDF.exe.config not found!";

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Copyright PDF - Writer", MessageBoxButton.OK);
                App.Current.Shutdown();
                return;
            }

            if (String.IsNullOrEmpty(Properties.Settings.Default.SpreadSheetId))
                missingParam = "\tSpreadSheetId\r\n";

            if (String.IsNullOrEmpty(Properties.Settings.Default.DocumentSheetName))
                missingParam += "\tDocumentSheetName\r\n";

            if (String.IsNullOrEmpty(Properties.Settings.Default.LicenseSheetName))
                missingParam += "\tLicenseSheetName\r\n";

            if (String.IsNullOrEmpty(Properties.Settings.Default.ClientSecretFileName))
                missingParam += "\tClientSecretFileName\r\n";

            if (String.IsNullOrEmpty(Properties.Settings.Default.OutputPath))
                missingParam += "\tOutputPath\r\n";

            if (!String.IsNullOrEmpty(missingParam))
                errorMessage = $"Value(s) below are not being set!" +
                                $"\r\n{missingParam}\r\n" +
                                $"Please set it in App.config";

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Copyright PDF - Writer", MessageBoxButton.OK);
                App.Current.Shutdown();
                return;
            }

            if (!System.IO.File.Exists(Properties.Settings.Default.ClientSecretFileName))
                errorMessage = $"{Properties.Settings.Default.ClientSecretFileName} not found!";

            if (!String.IsNullOrEmpty(errorMessage))
            {
                MessageBox.Show(errorMessage, "Copyright PDF - Writer", MessageBoxButton.OK);
                App.Current.Shutdown();
                return;
            }
        }

        int LastSelectedDocumentIndex = -1;
        int LastSelectedLicenseIndex = -1;

        private void dtgDocuments_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dtgDocuments.SelectedIndex = LastSelectedDocumentIndex;
        }

        private void dtgDocuments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgDocuments.SelectedIndex < 0) return;
            LastSelectedDocumentIndex = dtgDocuments.SelectedIndex;
        }

        private void dtgLicenses_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            dtgLicenses.SelectedIndex = LastSelectedLicenseIndex;
        }

        private void dtgLicenses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgLicenses.SelectedIndex < 0) return;
            LastSelectedLicenseIndex = dtgLicenses.SelectedIndex;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
