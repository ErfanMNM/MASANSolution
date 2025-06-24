using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpT
{
    public class GoogleSheetConfigHelper
    {
        private static GoogleSheetConfigHelper _instance;
        private static readonly object _lock = new object();

        private readonly SheetsService service;
        private readonly string spreadsheetId;
        private readonly string range;
        private readonly Dictionary<string, string> settings;

        // Private constructor — chỉ khởi tạo qua Init
        private GoogleSheetConfigHelper(string credentialsPath, string spreadsheetId, string range)
        {
            this.spreadsheetId = spreadsheetId;
            this.range = range;
            this.settings = new Dictionary<string, string>();

            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                                             .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }

            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoogleSheetConfigHelper",
            });

            LoadSettings();
        }

        private void LoadSettings()
        {
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();

            if (response.Values != null && response.Values.Count > 0)
            {
                foreach (var row in response.Values)
                {
                    if (row.Count >= 3)
                    {
                        string name = row[0]?.ToString()?.Trim();
                        string value = row[2]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(name))
                        {
                            settings[name] = value;
                        }
                    }
                }
            }
        }

        // Singleton Init (chỉ gọi 1 lần, VD trong Program.cs hoặc FormMain)
        public static void Init(string credentialsPath, string spreadsheetId, string range)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GoogleSheetConfigHelper(credentialsPath, spreadsheetId, range);
                }
            }
        }

        // Singleton Instance
        public static GoogleSheetConfigHelper Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("GoogleSheetConfigHelper chưa được Init!");
                return _instance;
            }
        }

        // Các hàm lấy setting
        public string GetSetting(string name)
        {
            if (settings.ContainsKey(name))
                return settings[name];
            else
                throw new KeyNotFoundException($"Không tìm thấy key: {name}");
        }

        public bool TryGetSetting(string name, out string value)
        {
            return settings.TryGetValue(name, out value);
        }

        public IReadOnlyDictionary<string, string> GetAllSettings()
        {
            return settings;
        }
    }

}
