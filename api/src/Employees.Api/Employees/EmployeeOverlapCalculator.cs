using Employees.Api.Csv;

namespace Employees.Api.Employees
{
    public class EmployeeOverlapCalculator : IEmployeeOverlapCalculator
    {
        public IDictionary<(int Emp1, int Emp2), int> CalculateOverlapTotals(IEnumerable<Entry> entries)
        {
            var byProject = entries.GroupBy(e => e.ProjectId);

            var overlapTotals = new Dictionary<(int, int), int>();

            foreach (var projectGroup in byProject)
            {
                var projectEntries = projectGroup.ToList();
                for (int i = 0; i < projectEntries.Count; i++)
                {
                    for (int j = i + 1; j < projectEntries.Count; j++)
                    {
                        CheckEmployees(overlapTotals, projectEntries, i, j);
                    }
                }
            }

            return overlapTotals;
        }

        private static void CheckEmployees(Dictionary<(int, int), int> overlapTotals, List<Entry> projectEntries, int i, int j)
        {
            var e1 = projectEntries[i];
            var e2 = projectEntries[j];
            var overlap = GetOverlapDays(e1, e2);
            if (overlap > 0)
            {
                var key = e1.EmpId < e2.EmpId
                    ? (e1.EmpId, e2.EmpId)
                    : (e2.EmpId, e1.EmpId);

                if (!overlapTotals.ContainsKey(key))
                    overlapTotals[key] = 0;

                overlapTotals[key] += overlap;
            }
        }


        private static int GetOverlapDays(Entry e1, Entry e2)
        {
            var overlapStart = e1.DateFrom > e2.DateFrom ? e1.DateFrom : e2.DateFrom;
            var overlapEnd = e1.DateTo < e2.DateTo ? e1.DateTo : e2.DateTo;

            if (overlapEnd < overlapStart)
                return 0;

            return (overlapEnd - overlapStart).Days + 1;
        }
    }
}
