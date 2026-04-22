using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? PatientId { get; set; }

    public int NotificationTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public virtual NotificationType NotificationType { get; set; } = null!;

    public virtual Patient? Patient { get; set; }

    public virtual User User { get; set; } = null!;
}
