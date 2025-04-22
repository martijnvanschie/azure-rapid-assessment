using Parquet;
using Parquet.Serialization;

namespace Azure.Rapid.Assessment.Core
{
    public class ParquetHandler
    {
        public static async Task AppendDataAsync<T>(List<T> data)
        {
            var valid = await ValidateSchema();
            if (valid == false)
            {
                Console.WriteLine("Schema validation failed. Exiting.");
                return;
            }

            if (File.Exists("test.parquet") is false)
            {
                using (Stream fileStream = File.Open("test.parquet", FileMode.Create))
                {
                    await ParquetSerializer.SerializeAsync(data, fileStream, new ParquetSerializerOptions { Append = false });
                }
            }
            else
            {
                using (Stream fileStream = File.Open("test.parquet", FileMode.Open))
                {
                    await ParquetSerializer.SerializeAsync(data, fileStream, new ParquetSerializerOptions { Append = true });
                }
            }
        }

        public static async Task<bool> ValidateSchema()
        {
            // Load the schema of the Parquet file
            using (Stream fileStream = File.OpenRead("test.parquet"))
            {
                var schema = await ParquetReader.ReadSchemaAsync(fileStream);

                // For now this is a fixed schema, but in the future we can make this dynamic from the file.
                var columns = new List<string>() { "name", "id", "kind", "subscriptionId", "tenantIds" };

                // Check if the schema contains the expected columns
                foreach (var column in columns)
                {
                    if (schema.Fields.Any(f => f.Name.Equals(column)) == false)
                    {
                        Console.WriteLine($"Column {column} not found in the schema.");
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
