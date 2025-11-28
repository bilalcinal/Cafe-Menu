namespace CafeMenu.Domain.Entities;

public abstract class BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? CreatorUserId { get; set; }
    public int TenantId { get; set; }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    public void Restore()
    {
        IsDeleted = false;
    }
}

