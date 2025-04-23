using Microsoft.Extensions.Logging;
using Parquet;
using Parquet.Serialization;

namespace Azure.Rapid.Assessment.Core
{
    public class ParquetHandler
    {
        private static readonly ILogger<ParquetHandler> _logger = LoggerManager.GetLogger<ParquetHandler>();

        private static readonly List<string> EXPECTED_FIELDS = new() { "name", "id", "type", "kind", "subscriptionId", "tenantId" };

        public static async Task AppendDataAsync<T>(List<T> data)
        {
            var valid = await ValidateSchema();
            if (valid == false)
            {
                _logger.LogWarning("Schema validation failed. Exiting.");
                return;
            }

            if (File.Exists("test.parquet"))
            {
                _logger.LogInformation("Parquet file already exists. Appending data to the existing file.");
                using (Stream fileStream = File.Open("test.parquet", FileMode.Open))
                {
                    await ParquetSerializer.SerializeAsync(data, fileStream, new ParquetSerializerOptions { Append = true });
                }

                return;
            }

            _logger.LogInformation("Parquet file does not exist. Creating a new file and writing data to it.");
            using (Stream fileStream = File.Open("test.parquet", FileMode.Create))
            {
                await ParquetSerializer.SerializeAsync(data, fileStream, new ParquetSerializerOptions { Append = false });
            }
        }

        public static async Task<bool> ValidateSchema()
        {
            if (File.Exists("test.parquet") is false)
            {
                _logger.LogWarning("Parquet file does not exist. Skipping schema validation and implicitly returning [true].");
                return true;
            }

            // Load the schema of the Parquet file  
            using (Stream fileStream = File.OpenRead("test.parquet"))
            {
                var schema = await ParquetReader.ReadSchemaAsync(fileStream);

                // For now this is a fixed schema, but in the future we can make this dynamic from the file.  
                // Check if the schema contains the expected columns  
                foreach (var field in EXPECTED_FIELDS)
                {
                    if (schema.Fields.Any(f => f.Name.Equals(field)) == false)
                    {
                        _logger.LogWarning($"Field [{field}] not found in the schema.");
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
