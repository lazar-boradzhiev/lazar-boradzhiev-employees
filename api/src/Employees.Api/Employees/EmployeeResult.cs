namespace Employees.Api.Employees
{
    public class EmployeeResult
    {
        public int Emp1 { get; set; }
        public int Emp2 { get; set; }
        public List<EmployeeProjectResult>? Projects { get; set; }
    }
}
