namespace Employees.Api.Csv;

using System;

public class Entry
{
    public int EmpId { get; set; }
    public int ProjectId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}