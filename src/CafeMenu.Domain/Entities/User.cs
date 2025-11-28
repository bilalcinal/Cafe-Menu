namespace CafeMenu.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public byte[] HashPassword { get; set; } = Array.Empty<byte>();
    public byte[] SaltPassword { get; set; } = Array.Empty<byte>();
}

