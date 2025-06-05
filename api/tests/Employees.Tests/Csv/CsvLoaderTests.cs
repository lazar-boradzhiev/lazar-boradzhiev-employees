using Employees.Api.Csv;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Employees.Tests.Csv
{
    public class CsvLoaderTests
    {
        private CsvLoader CreateLoader() => new CsvLoader(new NullLogger<CsvLoader>());

        private Stream GenerateStreamFromString(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }

        [Fact]
        public async Task LoadEntriesAsync_ValidRecords_ReturnsEntries()
        {
            // Arrange: two valid records
            var csv = new StringBuilder();
            csv.AppendLine("EmpID,ProjectID,DateFrom,DateTo");
            csv.AppendLine("1,1001,2020-01-01,2020-04-01");
            csv.AppendLine("2,1002,2020-02-01,2020-05-01");

            var loader = CreateLoader();
            using var stream = GenerateStreamFromString(csv.ToString());

            // Act
            var (entries, errors) = await loader.LoadEntriesAsync(stream);

            // Assert
            Assert.Empty(errors);
            Assert.Equal(2, entries.Count);
            Assert.Equal(1, entries[0].EmpId);
            Assert.Equal(1001, entries[0].ProjectId);
            Assert.Equal(new DateTime(2020, 1, 1), entries[0].DateFrom);
            Assert.Equal(new DateTime(2020, 4, 1), entries[0].DateTo);
            Assert.Equal(2, entries[1].EmpId);
            Assert.Equal(1002, entries[1].ProjectId);
        }

        [Fact]
        public async Task LoadEntriesAsync_DateFromAfterDateTo_RecordSkippedAndErrorAdded()
        {
            // Arrange: one record with DateFrom > DateTo
            var csv = new StringBuilder();
            csv.AppendLine("EmpID,ProjectID,DateFrom,DateTo");
            csv.AppendLine("1,1001,2020-05-01,2020-04-01"); // invalid date order

            var loader = CreateLoader();
            using var stream = GenerateStreamFromString(csv.ToString());

            // Act
            var (entries, errors) = await loader.LoadEntriesAsync(stream);

            // Assert: entries should be empty, errors should contain one message
            Assert.Empty(entries);
            Assert.Single(errors);
            Assert.Contains("Date from must be after date to for record at index: 0", errors[0]);
        }

        [Fact]
        public async Task LoadEntriesAsync_MalformedRecord_RecordSkippedAndErrorAdded()
        {
            // Arrange: one header, one malformed record (non-numeric EmpID)
            var csv = new StringBuilder();
            csv.AppendLine("EmpID,ProjectID,DateFrom,DateTo");
            csv.AppendLine("abc,1001,2020-01-01,2020-02-01"); // EmpID invalid

            var loader = CreateLoader();
            using var stream = GenerateStreamFromString(csv.ToString());

            // Act
            var (entries, errors) = await loader.LoadEntriesAsync(stream);

            // Assert: entries should be empty, errors should contain one message about loading record
            Assert.Empty(entries);
            Assert.Single(errors);
            Assert.Contains("Could not load record at index: 0", errors[0]);
        }

        [Fact]
        public async Task LoadEntriesAsync_MixedValidAndInvalidRecords_ProcessesCorrectly()
        {
            // Arrange: mixed valid, invalid order, and malformed
            var csv = new StringBuilder();
            csv.AppendLine("EmpID,ProjectID,DateFrom,DateTo");
            csv.AppendLine("1,1001,2020-01-01,2020-04-01"); // valid
            csv.AppendLine("2,1002,2020-05-01,2020-03-01"); // DateFrom > DateTo
            csv.AppendLine("abc,1003,2020-02-01,2020-03-01"); // malformed
            csv.AppendLine("3,1004,2020-03-01,2020-06-01"); // valid

            var loader = CreateLoader();
            using var stream = GenerateStreamFromString(csv.ToString());

            // Act
            var (entries, errors) = await loader.LoadEntriesAsync(stream);

            // Assert: two valid entries, two errors
            Assert.Equal(2, entries.Count);
            Assert.Equal(2, errors.Count);
            Assert.Equal(1, entries[0].EmpId);
            Assert.Equal(3, entries[1].EmpId);
            Assert.Contains("Date from must be after date to for record at index: 1", errors[0]);
            Assert.Contains("Could not load record at index: 2", errors[1]);
        }
    }
}
