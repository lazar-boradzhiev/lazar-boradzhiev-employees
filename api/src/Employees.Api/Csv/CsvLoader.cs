using CsvHelper;
using System.Globalization;

namespace Employees.Api.Csv
{
    public class CsvLoader : ICsvLoader
    {
        private readonly ILogger<CsvLoader> logger;

        public CsvLoader(ILogger<CsvLoader> logger)
        {
            this.logger = logger;
        }

        public async Task<(List<Entry> Entries, List<string> Errors)> LoadEntriesAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<EntryMap>();

            var records = new List<Entry>();
            var errors = new List<string>();
            var index = -1;
            while (await csv.ReadAsync())
            {
                index++;
                try
                {
                    var record = csv.GetRecord<Entry>();

                    if (record.DateFrom > record.DateTo)
                    {
                        errors.Add($"Date from must be after date to for record at index: {index}");
                        logger.LogWarning("Date from must be after date to for record at index: {Index}", csv.CurrentIndex);
                        continue;
                    }

                    records.Add(record);
                }
                catch (CsvHelperException ex)
                {
                    errors.Add($"Could not load record at index: {index}");
                    logger.LogWarning(ex, "Could not load record at index: {Index}", index);
                    continue;
                }
            }

            return (records, errors);
        }
    }
}
