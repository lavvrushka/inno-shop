
namespace UserManagement.Application.Common.Exeptions
{
    public class NotFoundException : Exception
    {
        public string EntityName { get; }
        public object Key { get; }

        public NotFoundException(string entityName, object key)
            : base($"Entity '{entityName}' with key {key} was not found.")
        {
            EntityName = entityName;
            Key = key;
        }

        public NotFoundException(string entityName, object key, Exception innerException)
            : base($"Entity '{entityName}' with key {key} was not found.", innerException)
        {
            EntityName = entityName;
            Key = key;
        }
    }
}
