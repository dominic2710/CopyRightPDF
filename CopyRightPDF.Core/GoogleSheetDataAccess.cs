using CopyRightPDF.Core.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections;
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
        SheetsService service;

        public GoogleSheetDataAccess(Stream clientSecretStream, string spreadSheetId)
        {
            GoogleCredential credential = GoogleCredential.FromStream(clientSecretStream)
                                .CreateScoped(Scopes);

            service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            SpreadSheetId = spreadSheetId;
        }

        public void CreateEntry(List<IList<object>> listData, string range)
        {
            var valueRange = new ValueRange
            {
                Values = listData
            };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }

        public void UpdateEntry(List<IList<object>> listData, string range)
        {
            var valueRange = new ValueRange
            {
                Values = listData
            };

            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadSheetId, range);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = updateRequest.Execute();
        }

        public void DeleteEntry(string range)
        {
            SpreadsheetsResource.ValuesResource.ClearRequest clearRequest =
                                        service.Spreadsheets.Values.Clear(new ClearValuesRequest(), SpreadSheetId, range);
            ClearValuesResponse clearResponse = clearRequest.Execute();
        }

        public void ClearAllDataExcludeFirstRow(string sheetName)
        {
            string range = $"{sheetName}!A2:ZZ";

            // Create the ClearValuesRequest object to clear the values in the specified range.
            SpreadsheetsResource.ValuesResource.ClearRequest clearRequest =
                                        service.Spreadsheets.Values.Clear(new ClearValuesRequest(), SpreadSheetId, range);
            ClearValuesResponse clearResponse = clearRequest.Execute();
        }

        public IList<IList<object>> GetEntry(string range)
        {
            var request = service.Spreadsheets.Values.Get(SpreadSheetId, range);

            var response = request.Execute();
            response.Values.RemoveAt(0);
            return response.Values;
        }

    }
}
