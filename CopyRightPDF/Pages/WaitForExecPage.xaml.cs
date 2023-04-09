using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
    /// Interaction logic for WaitForExecPage.xaml
    /// </summary>
    public partial class WaitForExecPage : Window
    {
        public WaitForExecPage()
        {
            InitializeComponent();
        }

        Action doWorkAction = null;

        public void DoWork(Action action, string waitingText)
        {
            if (action == null) return;

            txbWaitingText.Text = waitingText;
            doWorkAction = action;

            if (App.Current.MainWindow.IsActive == false)
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            else
            {
                var mainWindow = App.Current.MainWindow;
                this.Left = mainWindow.Left + mainWindow.Width / 2 - this.Width / 2;
                this.Top = mainWindow.Top + mainWindow.Height / 2 - this.Height / 2;
            }


            this.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //If window hiding
            if (!(bool)e.NewValue) return;

            //
            if (doWorkAction == null)
            {
                this.Hide();
                return;
            }
            Task.Run(() =>
            {
                doWorkAction();
            }).GetAwaiter()
            .OnCompleted(() =>
            {
                this.Hide();
            });
        }
    }
}
