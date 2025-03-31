namespace ProductManagement.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public int Quantity { get; set; } 
        public Guid UserId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? ImageId { get; set; }

        public Image? Image { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
