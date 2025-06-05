namespace Employees.Api.Models
{
    public class ErrorModel
    {
        public string[] Errors { get; }

        public ErrorModel(params string[] errors) => Errors = errors;
    }
}
