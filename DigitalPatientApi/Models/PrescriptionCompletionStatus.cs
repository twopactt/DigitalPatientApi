using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class PrescriptionCompletionStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PrescriptionCompletion> PrescriptionCompletions { get; set; } = new List<PrescriptionCompletion>();
}
