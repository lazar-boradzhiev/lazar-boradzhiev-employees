using Employees.Api.Csv;
using Employees.Api.Employees;

namespace Employees.Tests.Employees
{
    public class EmployeeOverlapCalculatorTests
    {
        private EmployeeOverlapCalculator CreateCalculator() => new EmployeeOverlapCalculator();

        private Entry CreateEntry(int empId, int projectId, DateTime from, DateTime to)
        {
            return new Entry
            {
                EmpId = empId,
                ProjectId = projectId,
                DateFrom = from,
                DateTo = to
            };
        }

        [Fact]
        public void CalculateOverlapTotals_NoEntries_ReturnsEmptyDictionary()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>();

            // Act
            var result = calculator.CalculateOverlapTotals(entries);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CalculateOverlapTotals_SingleEntry_ReturnsEmptyDictionary()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 31))
            };

            // Act
            var result = calculator.CalculateOverlapTotals(entries);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CalculateOverlapTotals_TwoEntriesSameProjectNoOverlap_ReturnsEmptyDictionary()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 15)),
                CreateEntry(2, 1001, new DateTime(2021, 1, 16), new DateTime(2021, 1, 31))
            };

            // Act
            var result = calculator.CalculateOverlapTotals(entries);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void CalculateOverlapTotals_TwoEntriesSameProjectWithOverlap_ReturnsOverlapDays()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 31)),
                CreateEntry(2, 1001, new DateTime(2021, 1, 15), new DateTime(2021, 2, 15))
            };
            // Overlap period: Jan 15 - Jan 31 => 17 days (inclusive)
            var expectedKey = (1, 2);
            var expectedDays = 17;

            // Act
            var result = calculator.CalculateOverlapTotals(entries);

            // Assert
            Assert.Single(result);
            Assert.True(result.ContainsKey(expectedKey));
            Assert.Equal(expectedDays, result[expectedKey]);
        }

        [Fact]
        public void CalculateOverlapTotals_MultipleProjectsAccumulatesCorrectly()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                // Project 1001: emp1 & emp2 overlap 10 days
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 20)),
                CreateEntry(2, 1001, new DateTime(2021, 1, 10), new DateTime(2021, 1, 30)),
                // Project 1002: emp1 & emp2 overlap 5 days
                CreateEntry(1, 1002, new DateTime(2021, 2, 1), new DateTime(2021, 2, 10)),
                CreateEntry(2, 1002, new DateTime(2021, 2, 5), new DateTime(2021, 2, 15)),
                // Project 1003: emp2 & emp3 overlap 3 days
                CreateEntry(2, 1003, new DateTime(2021, 3, 1), new DateTime(2021, 3, 5)),
                CreateEntry(3, 1003, new DateTime(2021, 3, 3), new DateTime(2021, 3, 10))
            };

            // Expected overlaps:
            // (1,2): 10 days + 6 days = 16 days (Jan 10-20 inclusive = 11 days? Actually Jan10-20 inclusive = 11. For Feb5-10 inclusive = 6. Total 17.)
            // Correction:
            // Project 1001: overlap Jan10-Jan20 inclusive = 11 days
            // Project 1002: overlap Feb5-Feb10 inclusive = 6 days
            // Sum = 17 days
            var expectedEmp1Emp2 = (1, 2);
            var expectedEmp1Emp2Days = 17;
            var expectedEmp2Emp3 = (2, 3);
            var expectedEmp2Emp3Days = 3; // overlap Mar3-Mar5 inclusive = 3 days

            // Act
            var result = calculator.CalculateOverlapTotals(entries);

            // Assert
            Assert.Equal(2, result.Count);

            Assert.True(result.ContainsKey(expectedEmp1Emp2));
            Assert.Equal(expectedEmp1Emp2Days, result[expectedEmp1Emp2]);

            Assert.True(result.ContainsKey(expectedEmp2Emp3));
            Assert.Equal(expectedEmp2Emp3Days, result[expectedEmp2Emp3]);
        }

        [Fact]
        public void CalculateOverlapTotals_SamePairMultipleEntriesInOneProject_AccumulatesOverlap()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                // Project 2001: emp1 & emp2 have two separate overlapping intervals
                CreateEntry(1, 2001, new DateTime(2021, 4, 1), new DateTime(2021, 4, 10)),
                CreateEntry(2, 2001, new DateTime(2021, 4, 5), new DateTime(2021, 4, 15)),
                CreateEntry(1, 2001, new DateTime(2021, 4, 20), new DateTime(2021, 4, 25)),
                CreateEntry(2, 2001, new DateTime(2021, 4, 22), new DateTime(2021, 4, 30))
            };
            // Overlaps: Apr5-Apr10 = 6 days; Apr22-Apr25 = 4 days; Total = 10 days
            var expectedKey = (1, 2);
            var expectedDays = 10;

            // Act
            var result = calculator.CalculateOverlapTotals(entries);

            // Assert
            Assert.Single(result);
            Assert.Equal(expectedDays, result[expectedKey]);
        }
    }
}
