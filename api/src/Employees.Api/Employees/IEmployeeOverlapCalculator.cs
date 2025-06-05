using Employees.Api.Csv;

namespace Employees.Api.Employees
{
    public interface IEmployeeOverlapCalculator
    {
        IDictionary<(int Emp1, int Emp2), int> CalculateOverlapTotals(IEnumerable<Entry> entries);
    }
}