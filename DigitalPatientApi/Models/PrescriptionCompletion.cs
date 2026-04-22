using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class PrescriptionCompletion
{
    public int Id { get; set; }

    public int PrescriptionId { get; set; }

    public DateTime CompletedAt { get; set; }

    public int PrescriptionCompletionStatusId { get; set; }

    public string? Reason { get; set; }

    public virtual Prescription Prescription { get; set; } = null!;

    public virtual PrescriptionCompletionStatus PrescriptionCompletionStatus { get; set; } = null!;
}
