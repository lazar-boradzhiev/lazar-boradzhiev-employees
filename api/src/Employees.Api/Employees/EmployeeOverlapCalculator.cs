using Employees.Api.Csv;

namespace Employees.Api.Employees
{
    public class EmployeeOverlapCalculator : IEmployeeOverlapCalculator
    {
        public EmployeeResult CalculateBestOverlap(IEnumerable<Entry> entries)
        {
            var byProject = entries.GroupBy(e => e.ProjectId);
            var overlapTotals = new Dictionary<(int, int), List<EmployeeProjectResult>>();

            foreach (var projectGroup in byProject)
            {
                var projectEntries = projectGroup.ToList();
                for (int i = 0; i < projectEntries.Count; i++)
                {
                    for (int j = i + 1; j < projectEntries.Count; j++)
                    {
                        CheckEmployees(overlapTotals, projectEntries[i], projectEntries[j]);
                    }
                }
            }

            var result = overlapTotals.OrderByDescending(x => x.Value.Sum(a => a.DaysWorked)).First();
            return new EmployeeResult
            {
                Emp1 = result.Key.Item1,
                Emp2 = result.Key.Item2,
                Projects = result.Value
            };
        }

        private static void CheckEmployees(
            Dictionary<(int, int), List<EmployeeProjectResult>> overlapTotals, Entry e1, Entry e2)
        {
            var overlap = GetOverlapDays(e1, e2);
            if (overlap > 0)
            {
                var key = e1.EmpId < e2.EmpId
                    ? (e1.EmpId, e2.EmpId)
                    : (e2.EmpId, e1.EmpId);

                if (!overlapTotals.ContainsKey(key))
                    overlapTotals[key] = [];

                overlapTotals[key].Add(new EmployeeProjectResult
                {
                    DaysWorked = overlap,
                    ProjectId = e1.ProjectId
                });
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
