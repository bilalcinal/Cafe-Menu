namespace CafeMenu.Domain.Entities;

public class Product : BaseEntity
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public decimal Price { get; set; }
    public string ImagePath { get; set; } = string.Empty;

    public Category Category { get; set; } = null!;
    public ICollection<ProductProperty> ProductProperties { get; set; } = new List<ProductProperty>();
}

