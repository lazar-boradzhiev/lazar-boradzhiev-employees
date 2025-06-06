using System;
using System.Collections.Generic;
using System.Linq;
using Employees.Api.Csv;
using Employees.Api.Employees;
using Xunit;

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
        public void CalculateBestOverlap_NoEntries_ThrowsInvalidOperationException()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => calculator.CalculateBestOverlap(entries));
        }

        [Fact]
        public void CalculateBestOverlap_SingleEntry_ThrowsInvalidOperationException()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 31))
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => calculator.CalculateBestOverlap(entries));
        }

        [Fact]
        public void CalculateBestOverlap_TwoEntriesSameProjectNoOverlap_ThrowsInvalidOperationException()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 15)),
                CreateEntry(2, 1001, new DateTime(2021, 1, 16), new DateTime(2021, 1, 31))
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => calculator.CalculateBestOverlap(entries));
        }

        [Fact]
        public void CalculateBestOverlap_TwoEntriesSameProjectWithOverlap_ReturnsEmployeeResult()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 31)),
                CreateEntry(2, 1001, new DateTime(2021, 1, 15), new DateTime(2021, 2, 15))
            };
            // Overlap period: Jan 15 - Jan 31 => 17 days inclusive

            // Act
            var result = calculator.CalculateBestOverlap(entries);

            // Assert
            Assert.Equal(1, result.Emp1);
            Assert.Equal(2, result.Emp2);
            Assert.NotNull(result.Projects);
            Assert.Single(result.Projects);
            var projectResult = result.Projects.First();
            Assert.Equal(1001, projectResult.ProjectId);
            Assert.Equal(17, projectResult.DaysWorked);
        }

        [Fact]
        public void CalculateBestOverlap_MultipleProjects_ReturnsBestPairOnly()
        {
            // Arrange
            var calculator = CreateCalculator();
            var entries = new List<Entry>
            {
                // Project 1001: emp1 & emp2 overlap Jan10-Jan20 inclusive = 11 days
                CreateEntry(1, 1001, new DateTime(2021, 1, 1), new DateTime(2021, 1, 20)),
                CreateEntry(2, 1001, new DateTime(2021, 1, 10), new DateTime(2021, 1, 30)),
                // Project 1002: emp1 & emp2 overlap Feb5-Feb10 inclusive = 6 days
                CreateEntry(1, 1002, new DateTime(2021, 2, 1), new DateTime(2021, 2, 10)),
                CreateEntry(2, 1002, new DateTime(2021, 2, 5), new DateTime(2021, 2, 15)),
                // Project 1003: emp2 & emp3 overlap Mar3-Mar5 inclusive = 3 days
                CreateEntry(2, 1003, new DateTime(2021, 3, 1), new DateTime(2021, 3, 5)),
                CreateEntry(3, 1003, new DateTime(2021, 3, 3), new DateTime(2021, 3, 10))
            };
            // Total overlaps: (1,2) = 11 + 6 = 17 days; (2,3) = 3 days

            // Act
            var result = calculator.CalculateBestOverlap(entries);

            // Assert
            Assert.Equal(1, result.Emp1);
            Assert.Equal(2, result.Emp2);
            Assert.NotNull(result.Projects);
            Assert.Equal(2, result.Projects.Count);

            // Validate project breakdown for (1,2)
            var proj1001 = result.Projects.Single(p => p.ProjectId == 1001);
            var proj1002 = result.Projects.Single(p => p.ProjectId == 1002);
            Assert.Equal(11, proj1001.DaysWorked);
            Assert.Equal(6, proj1002.DaysWorked);
        }

        [Fact]
        public void CalculateBestOverlap_SamePairMultipleEntriesOneProject_MultipleProjectResults()
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
            // Overlaps: Apr5-Apr10 = 6 days; Apr22-Apr25 = 4 days

            // Act
            var result = calculator.CalculateBestOverlap(entries);

            // Assert
            Assert.Equal(1, result.Emp1);
            Assert.Equal(2, result.Emp2);
            Assert.NotNull(result.Projects);
            Assert.Equal(2, result.Projects.Count);

            // There should be two entries for project 2001 with separate days
            var daysList = result.Projects.Where(p => p.ProjectId == 2001).Select(p => p.DaysWorked).OrderBy(d => d).ToList();
            Assert.Equal(new List<int> { 4, 6 }, daysList);
        }
    }
}
