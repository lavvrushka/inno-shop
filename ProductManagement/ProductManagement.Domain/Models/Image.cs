namespace ProductManagement.Domain.Models
{
    public class Image
    {
        public Guid Id { get; set; }

        public string ImageData { get; set; } = string.Empty;

        public string ImageType { get; set; } = string.Empty;
    }
}
