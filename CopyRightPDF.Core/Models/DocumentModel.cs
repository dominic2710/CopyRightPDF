using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyRightPDF.Core.Models
{
    public class DocumentModel : BaseModel
    {
        private int _rowId;
        private string _fileId;
        private string _fileName;
        private string _description;
        private ObservableCollection<LicenseModel> _licenses;
        private bool _isDelete;

        public int RowId
        {
            get => _rowId;
            set { _rowId = value; OnPropertyChanged(nameof(RowId)); }
        }

        public string FileId
        {
            get => _fileId;
            set { _fileId = value; OnPropertyChanged(nameof(FileId)); }
        }

        public string FileName
        {
            get => _fileName;
            set { _fileName = value; OnPropertyChanged(nameof(FileName)); }
        }

        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        public ObservableCollection<LicenseModel> Licenses
        {
            get => _licenses;
            set { _licenses = value; OnPropertyChanged(nameof(Licenses)); }
        }

        public bool IsDelete
        {
            get => _isDelete;
            set
            {
                _isDelete = value; OnPropertyChanged(nameof(IsDelete));
            }
        }

        public IList<object> ToList
        {
            get
            {
                return new List<object> { "=ROW() - 1", FileId, FileName, Description, IsDelete };
            }
        }
    }

    public enum DocumentModelEnum
    {
        RowId = 0,
        FileId,
        FileName,
        Description,
        IsDelete
    }
}
