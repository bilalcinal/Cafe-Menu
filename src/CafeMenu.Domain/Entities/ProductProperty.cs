namespace CafeMenu.Domain.Entities;

public class ProductProperty
{
    public int ProductPropertyId { get; set; }
    public int ProductId { get; set; }
    public int PropertyId { get; set; }

    public Product Product { get; set; } = null!;
    public Property Property { get; set; } = null!;
}

