namespace UserManagement.API.Middlewares
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ValidationAttribute : Attribute
    {

        public Type TargetType { get; }

        public ValidationAttribute(Type targetType)
        {
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }
    }
}
