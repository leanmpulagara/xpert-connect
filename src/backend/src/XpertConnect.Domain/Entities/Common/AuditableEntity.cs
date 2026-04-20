namespace XpertConnect.Domain.Entities.Common;

/// <summary>
/// Entity with audit trail (who created/modified)
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
