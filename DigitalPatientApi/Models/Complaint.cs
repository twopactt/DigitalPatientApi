using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Complaint
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}
