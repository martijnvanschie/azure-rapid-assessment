using Azure.Rapid.Assessment.Core.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Azure.Rapid.Assessment.Core
{
    public class QueryFileHandler
    {
        const string FILE_EXTENTION = ".query";
        
        private static readonly ILogger<QueryFileHandler> _logger = LoggerManager.GetLogger<QueryFileHandler>();
        
        static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static async Task<List<QueryInfo>> GetAllQueryFilesAsync(DirectoryInfo directory)
        {
            var queryFiles = new List<QueryInfo>();

            if (!directory.Exists)
            {
                _logger.LogWarning($"The query folder '{directory.FullName}' does not exist.");
                return queryFiles;
            }

            var files = Directory.GetFiles(directory.FullName, $"*{FILE_EXTENTION}", SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                _logger.LogWarning($"The query folder '{directory.FullName}' does not contain any .query files.");
                return queryFiles;
            }

            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                var qf = await ReadFileAsync(fi);
                if (qf != null)
                {
                    queryFiles.Add(qf);
                }
            }

            return queryFiles;
        }

        public static async Task<QueryInfo> ReadFileAsync(FileInfo file)
        {
            using (Stream fileStream = File.Open(file.FullName, FileMode.Open))
            {
                var data = await JsonSerializer.DeserializeAsync<QueryInfo>(fileStream);
                return data;
            }
        }

        public static async Task WriteFileAsync(QueryInfo data, DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }

            string fileName = Path.Combine(directory.FullName, $"{data.Title}.{FILE_EXTENTION}");

            using (Stream fileStream = File.Open(fileName, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fileStream, data, jsonSerializerOptions);
            }
        }   
    }
}
