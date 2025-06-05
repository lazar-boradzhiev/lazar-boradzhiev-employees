using Employees.Api.Csv;
using Employees.Api.Employees;
using Employees.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Api.Endpoints
{
    public static class EmployeeOvarlapEndpoint
    {
        private const int MaxFileSize = 1024 * 1024;
        private const string FileTooBigError = "File must not be larger than 1MB";
        private const string NoValidEntriesError = "Could not find any valid entries in csv";
        private const string InvalidFileExtension = "Invalid file extension";

        public static RouteHandlerBuilder MapEmployeesOverlapEndpoint(this IEndpointRouteBuilder endpoints) 
            => endpoints.MapPost("/employees/overlap",
                async ([FromServices] ICsvLoader csvLoader,
                [FromServices] IEmployeeOverlapCalculator calulator,
                IFormFile formFile) =>
                {
                    if (formFile.Length >= MaxFileSize)
                    {
                        return Results.BadRequest(new ErrorModel(FileTooBigError));
                    }

                    if (!formFile.FileName.EndsWith(".csv"))
                    {
                        return Results.BadRequest(new ErrorModel(InvalidFileExtension));
                    }

                    using var stream = formFile.OpenReadStream();
                    var (entries, errors) = await csvLoader.LoadEntriesAsync(stream);
                    if (errors.Count != 0)
                    {
                        return Results.BadRequest(new ErrorModel([.. errors]));
                    }

                    if (entries.Count == 0)
                    {
                        return Results.BadRequest(new ErrorModel(NoValidEntriesError));
                    }

                    var totals = calulator
                        .CalculateOverlapTotals(entries)
                        .OrderByDescending(kvp => kvp.Value)
                        .Select(FormatResult);

                    return Results.Ok(new
                    {
                        Best = totals.First(),
                        Totals = totals
                    });
                })
                .WithName("EmployeesOverlap")
                .DisableAntiforgery()
                .Accepts<IFormFile>("multipart/form-data")
                .WithOpenApi();

        private static string FormatResult(KeyValuePair<(int Emp1, int Emp2), int> first)
            => $"Employee {first.Key.Emp1} and Employee {first.Key.Emp2} worked {first.Value} days total";
    }
}
