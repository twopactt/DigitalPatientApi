using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class NotificationThreshold
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int CriticalRiskThreshold { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
