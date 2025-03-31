namespace ProductManagement.Application.Common.Exeptions
{
    public class ValidationUserManagementException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; set; }
        public ValidationUserManagementException(IReadOnlyDictionary<string, string[]> errors)
            : base("One or more validation errors occured")
            => Errors = errors;
    }
}
