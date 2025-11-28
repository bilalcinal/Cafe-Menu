namespace CafeMenu.Domain.Entities;

public class Property : BaseEntity
{
    public int PropertyId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public ICollection<ProductProperty> ProductProperties { get; set; } = new List<ProductProperty>();
}

