namespace CafeMenu.Application.Models;

public class UserDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
}

