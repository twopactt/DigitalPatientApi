using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Deviation
{
    public int Id { get; set; }

    public int? PatientId { get; set; }

    public int? MeasurementId { get; set; }

    public string DeviationType { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DetectedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public int? ResolvedBy { get; set; }

    public virtual User? ResolvedByNavigation { get; set; }
}
