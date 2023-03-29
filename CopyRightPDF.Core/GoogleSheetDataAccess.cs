using CopyRightPDF.Core.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace CopyRightPDF.Core
{
    public class GoogleSheetDataAccess
    {
        readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        readonly string ApplicationName = "CrPdfApplication";
        readonly string SpreadSheetId = "1ZpVSuHWlt50fyLp0jQn59GotXUB_XJ5WyVDt7k-5MKw";
        readonly string SheetName = "Data";
        SheetsService service;

        enum DocumentEnum
        {
            RowId = 0,
            DocumentId,
            Password,
            NumberOfLimitDevice,
            NumberOfActivatedDevice,
            ActivatedDevicesMAC,
            LatestAccess,
        }

        public GoogleSheetDataAccess(Stream clientSecretStream)
        {
            GoogleCredential credential = GoogleCredential.FromStream(clientSecretStream)
                                .CreateScoped(Scopes);

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }

        public void CreateEntry(DocumentModel document)
        {
            var range = $"{SheetName}!A:G";
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { document.ToList }
            };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void UpdateEntry(DocumentModel document)
        {
            var rowId = int.Parse(document.RowId) + 1;
            var range = $"{SheetName}!A{rowId}:G{rowId}";
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>> { document.ToList }
            };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = updateRequest.Execute();

        }

        public DocumentModel GetEntryByDocumentId(string documentId)
        {
            var range = $"{SheetName}!A:G";
            var request = service.Spreadsheets.Values.Get(SpreadSheetId, range);

            var response = request.Execute();
            var values = response.Values;

            if (values == null || values.Count == 0) return null;

            var rows = values.Where(x => x[1].ToString() == documentId).ToList();
            if (rows == null || rows.Count == 0) return null;

            var row = rows[0];
            return new DocumentModel
            {
                RowId = GetItem(row, DocumentEnum.RowId),
                DocumentId = GetItem(row, DocumentEnum.DocumentId),
                Password = GetItem(row, DocumentEnum.Password),
                NumberOfLimitDevice = GetItem(row, DocumentEnum.NumberOfLimitDevice),
                NumberOfActivatedDevice = GetItem(row, DocumentEnum.NumberOfActivatedDevice),
                ActivatedDevicesMAC = GetItem(row, DocumentEnum.ActivatedDevicesMAC),
                LatestAccess = GetItem(row, DocumentEnum.LatestAccess),
            };
        }

        private string GetItem(IList<object> items, DocumentEnum index)
        {
            if (items.Count > (int)index)
                return items[(int)index].ToString();

            return "";
        }
    }
}
