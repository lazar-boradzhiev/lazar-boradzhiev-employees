
namespace Employees.Api.Csv
{
    public interface ICsvLoader
    {
        Task<(List<Entry> Entries, List<string> Errors)> LoadEntriesAsync(Stream stream);
    }
}