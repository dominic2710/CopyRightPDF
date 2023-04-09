using CopyRightPDF.Core.Models;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace CopyRightPDF.Core
{
    public class CopyRightPDFDataProvider
    {
        private GoogleSheetDataAccess dataAccess;
        string spreadSheetId, documentSheetName, licenseSheetName;
        public CopyRightPDFDataProvider(Stream fs, string spreadSheetId, string documentSheetName, string licenseSheetName)
        {
            dataAccess = new GoogleSheetDataAccess(fs, spreadSheetId);
            this.documentSheetName = documentSheetName;
            this.licenseSheetName = licenseSheetName;
        }

        public void UpdateDocument(List<DocumentModel> documents)
        {
            dataAccess.ClearAllDataExcludeFirstRow(documentSheetName);
            dataAccess.ClearAllDataExcludeFirstRow(licenseSheetName);

            var listData = documents.Select(x => x.ToList).ToList();
            var range = $"{documentSheetName}!A:D";

            dataAccess.CreateEntry(listData, range);

            listData = new List<IList<object>>();

            foreach (var doc in documents)
            {
                listData.AddRange(doc.Licenses.Select(x => x.ToList).ToList());
            }
            range = $"{licenseSheetName}!A:O";

            dataAccess.CreateEntry(listData, range);

            //return true;
        }

        public List<DocumentModel> GetDocument()
        {
            var range = $"{documentSheetName}!A:D";
            var documentData = dataAccess.GetEntry(range);

            range = $"{licenseSheetName}!A:O";
            var licensesData = dataAccess.GetEntry(range);

            var licenses = new List<LicenseModel>();
            foreach (var row in licensesData)
            {
                licenses.Add(new LicenseModel
                {
                    RowId = GetItem<int>(row, (int)LicenseModelEnum.RowId),
                    Status = GetItem<string>(row, (int)LicenseModelEnum.Status),
                    FileId = GetItem<string>(row, (int)LicenseModelEnum.FileId),
                    CustomerName = GetItem<string>(row, (int)LicenseModelEnum.CustomerName),
                    Password = GetItem<string>(row, (int)LicenseModelEnum.Password),
                    NumberOfLimitDevice = GetItem<int>(row, (int)LicenseModelEnum.NumberOfLimitDevice),
                    PreventPrint = GetItem<bool>(row, (int)LicenseModelEnum.PreventPrint),
                    PreventSameOS = GetItem<bool>(row, (int)LicenseModelEnum.PreventSameOS),
                    PreventScreenshot = GetItem<bool>(row, (int)LicenseModelEnum.PreventScreenshot),
                    NumberOfActivatedDevice = GetItem<int>(row, (int)LicenseModelEnum.NumberOfActivatedDevice),
                    ActivatedDeviceMAC = GetItem<string>(row, (int)LicenseModelEnum.ActivatedDeviceMAC),
                    ActivatedOS = GetItem<string>(row, (int)LicenseModelEnum.ActivatedOS),
                    LastAccess = GetItem<DateTime?>(row, (int)LicenseModelEnum.LastAccess),
                    MinVersion = GetItem<string>(row, (int)LicenseModelEnum.MinVersion),
                    ExpireDate = GetItem<DateTime?>(row, (int)LicenseModelEnum.ExpireDate),
                });
            }

            var documents = new List<DocumentModel>();
            foreach (var row in documentData)
            {
                documents.Add(new DocumentModel
                {
                    RowId = GetItem<int>(row, (int)DocumentModelEnum.RowId),
                    FileId = GetItem<string>(row, (int)DocumentModelEnum.FileId),
                    FileName = GetItem<string>(row, (int)DocumentModelEnum.FileName),
                    Description = GetItem<string>(row, (int)DocumentModelEnum.Description),
                    Licenses = new System.Collections.ObjectModel.ObservableCollection<LicenseModel>(
                                        licenses.Where(x => x.FileId == GetItem<string>(row, (int)DocumentModelEnum.FileId)))
                });
            }

            return documents;
        }

        public LicenseModel GetLicense(string fileId, string password)
        {
            var range = $"{licenseSheetName}!A:O";
            var licensesData = dataAccess.GetEntry(range);

            if (licensesData == null || licensesData.Count == 0) return null;

            var row = licensesData.Where(x => GetItem<string>(x, (int)LicenseModelEnum.FileId) == fileId)
                                    .Where(x => GetItem<string>(x, (int)LicenseModelEnum.Password) == password)
                                    .FirstOrDefault();
            if (row == null || row.Count == 0) return null;

            return new LicenseModel
            {
                RowId = GetItem<int>(row, (int)LicenseModelEnum.RowId),
                Status = GetItem<string>(row, (int)LicenseModelEnum.Status),
                FileId = GetItem<string>(row, (int)LicenseModelEnum.FileId),
                CustomerName = "**********",
                Password = "**********",
                NumberOfLimitDevice = GetItem<int>(row, (int)LicenseModelEnum.NumberOfLimitDevice),
                PreventPrint = GetItem<bool>(row, (int)LicenseModelEnum.PreventPrint),
                PreventSameOS = GetItem<bool>(row, (int)LicenseModelEnum.PreventSameOS),
                PreventScreenshot = GetItem<bool>(row, (int)LicenseModelEnum.PreventScreenshot),
                NumberOfActivatedDevice = GetItem<int>(row, (int)LicenseModelEnum.NumberOfActivatedDevice),
                ActivatedDeviceMAC = GetItem<string>(row, (int)LicenseModelEnum.ActivatedDeviceMAC),
                ActivatedOS = GetItem<string>(row, (int)LicenseModelEnum.ActivatedOS),
                LastAccess = GetItem<DateTime?>(row, (int)LicenseModelEnum.LastAccess),
                MinVersion = GetItem<string>(row, (int)LicenseModelEnum.MinVersion),
                ExpireDate = GetItem<DateTime?>(row, (int)LicenseModelEnum.ExpireDate),
            };

        }

        private T GetItem<T>(IList<object> items, int index)
        {
            if (items.Count > (int)index)
            {
                var value = items[(int)index];

                if (typeof(T) == typeof(string)) return (T)value;
                if (typeof(T) == typeof(int))
                {
                    int.TryParse(value.ToString(), out int intValue);
                    return (T)(object)intValue;
                }
                if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                {
                    if (String.IsNullOrEmpty(value.ToString())) return default;
                    if (DateTime.TryParse(value.ToString(), out DateTime dateValue))
                    {
                        return (T)(object)dateValue;
                    }
                }
                if (typeof(T) == typeof(bool))
                {
                    if (bool.TryParse(value.ToString(), out bool boolValue))
                    {
                        return (T)(object)boolValue;
                    }
                }
                throw new InvalidCastException($"Cannot convert object of type {value.GetType()} to type {typeof(T)}");
            }

            return default;
        }

        public void UpdateLicenseAccess(LicenseModel license)
        {
            var rowId = license.RowId + 1;
            var listData = new List<IList<object>> { license.ToListForReaderUpdate };
            var range = $"{licenseSheetName}!L{rowId}:O{rowId}";

            dataAccess.UpdateEntry(listData, range);
        }
    }
}
