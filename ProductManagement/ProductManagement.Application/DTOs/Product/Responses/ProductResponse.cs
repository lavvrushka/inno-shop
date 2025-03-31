namespace ProductManagement.Application.DTOs.Product.Responses
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ImageData { get; set; }
        public string ImageType { get; set; }
        public Guid ImageId { get; set; }

        public ProductResponse() { }
        public ProductResponse(
            Guid id,
            string name,
            string description,
            decimal price,
            bool isAvailable,
            int quantity,
            Guid userId,
            DateTime createdAt,
            string imageData,
            string imageType,
             Guid imageId)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            IsAvailable = isAvailable;
            Quantity = quantity;
            UserId = userId;
            CreatedAt = createdAt;
            ImageData = imageData;
            ImageType = imageType;
            ImageId = imageId;
        }
    }
}
