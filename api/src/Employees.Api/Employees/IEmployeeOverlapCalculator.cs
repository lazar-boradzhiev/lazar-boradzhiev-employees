using Employees.Api.Csv;

namespace Employees.Api.Employees
{
    public interface IEmployeeOverlapCalculator
    {
        EmployeeResult CalculateBestOverlap(IEnumerable<Entry> entries);
    }
}