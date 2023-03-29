using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyRightPDF.Core.Models
{
    public class DocumentModel
    {
        public string RowId { get; set; }
        public string DocumentId { get; set; }
        public string Password { get; set; }
        public string NumberOfLimitDevice { get; set; }
        public string NumberOfActivatedDevice { get; set; }
        public string ActivatedDevicesMAC { get; set; }
        public string LatestAccess { get; set; }

        public List<object> ToList
        {
            get
            {
                return new List<object>() { "=ROW() - 1", DocumentId, Password, NumberOfLimitDevice, NumberOfActivatedDevice, ActivatedDevicesMAC, LatestAccess };
            }
        }
    }
}
