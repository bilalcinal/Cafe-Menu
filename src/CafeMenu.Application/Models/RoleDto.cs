namespace CafeMenu.Application.Models;

public class RoleDto
{
    public int RoleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public int UserCount { get; set; }
}

