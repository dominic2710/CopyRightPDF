using CopyRightPDF.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

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

        public bool UpdateDocument(List<DocumentModel> documents)
        {
            try
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
                range = $"{licenseSheetName}!A:W";

                dataAccess.CreateEntry(listData, range);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }

        public bool AddLicense(LicenseModel license)
        {
            try
            {
                var listData = new List<IList<object>> { license.ToList };
                var range = $"{licenseSheetName}!A:T";

                dataAccess.CreateEntry(listData, range);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }

        public bool ApproveLicense(LicenseModel license)
        {
            try
            {
                license.Status = "Approved";
                var rowId = license.RowId + 1;
                var listData = new List<IList<object>> { license.ToList };
                var range = $"{licenseSheetName}!A{rowId}:W{rowId}";

                dataAccess.UpdateEntry(listData, range);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }

        public List<DocumentModel> GetDocument()
        {
            try
            {
                var range = $"{documentSheetName}!A:D";
                var documentData = dataAccess.GetEntry(range);

                range = $"{licenseSheetName}!A:W";
                var licensesData = dataAccess.GetEntry(range);

                var licenses = new List<LicenseModel>();
                var rowId = 0;
                foreach (var row in licensesData)
                {
                    rowId++;
                    var status = GetItem<string>(row, (int)LicenseModelEnum.Status);
                    if (String.IsNullOrEmpty(status))
                        status = "Registered";
                    licenses.Add(new LicenseModel
                    {
                        RegisteredEmail = GetItem<string>(row, (int)LicenseModelEnum.RegisteredEmail),
                        RegisteredPhoneNumber = GetItem<string>(row, (int)LicenseModelEnum.RegisteredPhoneNumber),
                        RegisteredCustomerName = GetItem<string>(row, (int)LicenseModelEnum.RegisteredCustomerName),
                        RegisteredFileName = GetItem<string>(row, (int)LicenseModelEnum.RegisteredFileName),
                        RowId = rowId,
                        Status = status,
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
                        SpecifiedExpireDate = GetItem<DateTime?>(row, (int)LicenseModelEnum.SpecifiedExpireDate),
                        ExpireDayCount = GetItem<int?>(row, (int)LicenseModelEnum.ExpireDayCount),
                        IsLocked = GetItem<bool>(row, (int)LicenseModelEnum.IsLocked),
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
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }

        public LicenseModel GetLicense(string fileId, string password)
        {
            try
            {
                var range = $"{licenseSheetName}!A:P";
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
                    IsLocked = GetItem<bool>(row, (int)LicenseModelEnum.IsLocked),
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }

        private T GetItem<T>(IList<object> items, int index)
        {
            if (items.Count > (int)index)
            {
                var value = items[(int)index];

                if (typeof(T) == typeof(string)) return (T)value;
                if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
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

        public bool UpdateLicenseAccessInfo(LicenseModel license)
        {
            try
            {
                var rowId = license.RowId + 1;
                var listData = new List<IList<object>> { license.ToListForReaderUpdate };
                var range = $"{licenseSheetName}!K{rowId}:P{rowId}";

                dataAccess.UpdateEntry(listData, range);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }
        public bool UpdateLicenseLockStatus(LicenseModel license)
        {
            try
            {
                var rowId = license.RowId + 1;
                var listData = new List<IList<object>> { new List<object> { license.IsLocked } };
                var range = $"{licenseSheetName}!P{rowId}:P{rowId}";

                dataAccess.UpdateEntry(listData, range);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while processing.\r\n{ex.Message}");
            }
        }
    }
}
