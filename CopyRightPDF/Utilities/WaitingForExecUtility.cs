using CopyRightPDF.Pages;
using CopyRightPDF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyRightPDF.Utilities
{
    public class WaitingForExecUtility
    {
        private WaitingForExecUtility()
        {
            WaitForExecPage = new WaitForExecPage();
        }
        private WaitForExecPage WaitForExecPage { get; set; }
        private static WaitingForExecUtility instance;
        public static WaitingForExecUtility Instance
        {
            get
            {
                if (instance == null)
                    instance = new WaitingForExecUtility();

                return instance;
            }
        }

        public void DoWork(Action action, string waitingText = "Loading")
        {
            WaitForExecPage.DoWork(action, waitingText);
        }
    }
}

