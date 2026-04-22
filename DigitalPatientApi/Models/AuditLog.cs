using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ActionType { get; set; } = null!;

    public string? EntityType { get; set; }

    public int? EntityId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
