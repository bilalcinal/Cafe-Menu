namespace CafeMenu.Application.Models;

public class PermissionDto
{
    public int PermissionId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

