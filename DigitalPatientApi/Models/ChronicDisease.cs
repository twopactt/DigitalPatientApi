using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class ChronicDisease
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; } = new List<PatientChronicDisease>();
}
