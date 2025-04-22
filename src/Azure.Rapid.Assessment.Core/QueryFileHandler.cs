using Azure.Rapid.Assessment.Core.Model;
using Parquet.Meta;
using System.Text.Json;

namespace Azure.Rapid.Assessment.Core
{
    public class QueryFileHandler
    {
        const string FILE_EXTENTION = ".query";
        static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static async Task<List<QueryInfo>> GetAllQueryFilesAsync(DirectoryInfo directory)
        {
            var queryFiles = new List<QueryInfo>();

            // Check if folder exists
            if (!directory.Exists)
            {
                Console.WriteLine($"The query folder '{directory.FullName}' does not exist.");
                return queryFiles;
            }

            // iterate over all files in the query folder and return only files with extension .query, including subdirectories
            var files = Directory.GetFiles(directory.FullName, $"*{FILE_EXTENTION}", SearchOption.AllDirectories);

            // Check if there are any files in the query folder
            if (files.Length == 0)
            {
                Console.WriteLine($"The query folder '{directory.FullName}' does not contain any .query files.");
                return queryFiles;
            }

            // Iterate over all files in the query folder
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
            // Check if the directory exists
            if (!directory.Exists)
            {
                directory.Create();
            }

            // Formet the file name from the directory and the title of the data
            string fileName = Path.Combine(directory.FullName, $"{data.Title}.{FILE_EXTENTION}");

            using (Stream fileStream = File.Open(fileName, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fileStream, data, jsonSerializerOptions);
            }
        }   
    }
}
