using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Complaint
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<MeasurementComplaint> MeasurementComplaints { get; set; } = new List<MeasurementComplaint>();
}
