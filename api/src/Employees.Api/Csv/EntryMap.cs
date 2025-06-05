using CsvHelper.Configuration;

namespace Employees.Api.Csv;

public class EntryMap : ClassMap<Entry>
{
    private const string DateFormat = "yyyy-MM-dd";

    public EntryMap()
    {
        Map(m => m.EmpId).Name("EmpID", "EmpId", "EmployeeID");
        Map(m => m.ProjectId).Name("ProjectID", "ProjectId");
        Map(m => m.DateFrom).Name("DateFrom").TypeConverterOption.Format(DateFormat);
        Map(m => m.DateTo).Name("DateTo").TypeConverterOption.NullValues("NULL", "").Default(DateTime.Today, true);
    }
}