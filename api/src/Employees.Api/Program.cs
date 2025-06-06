using Employees.Api.Csv;
using Employees.Api.Employees;
using Employees.Api.Endpoints;
using Employees.Api.ErrorsHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSingleton<IEmployeeOverlapCalculator, EmployeeOverlapCalculator>();
builder.Services.AddSingleton<ICsvLoader, CsvLoader>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
}

app.UseStaticFiles();
app.MapEmployeesOverlapEndpoint();

app.Run();
