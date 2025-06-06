using CsvHelper.Configuration;

namespace Employees.Api.Csv;

public class EntryMap : ClassMap<Entry>
{
    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd",
        "yyyy/MM/dd",
        "MM/dd/yyyy",
        "dd-MM-yyyy",
        "dd/MM/yyyy"
    ];

    public EntryMap()
    {
        Map(m => m.EmpId).Name("EmpID", "EmpId", "EmployeeID");

        Map(m => m.ProjectId).Name("ProjectID", "ProjectId");

        Map(m => m.DateFrom).Name("DateFrom")
            .TypeConverterOption
            .Format(DateFormats);

        Map(m => m.DateTo).Name("DateTo")
            .TypeConverterOption
            .NullValues("NULL", "")
            .Default(DateTime.Today, true)
            .TypeConverterOption
            .Format(DateFormats);
    }
}