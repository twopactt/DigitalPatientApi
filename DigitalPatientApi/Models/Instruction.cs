using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Instruction
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
