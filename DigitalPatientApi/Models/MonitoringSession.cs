using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class MonitoringSession
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Purpose { get; set; }

    public bool IsActive { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
