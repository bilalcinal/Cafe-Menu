namespace CafeMenu.Domain.Entities;

public class Role : BaseEntity
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public Tenant? Tenant { get; set; }
    public User? CreatedByUser { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<User> Users { get; set; } = new List<User>();
}

