using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class MeasurementComplaint
{
    public int Id { get; set; }

    public int MeasurementId { get; set; }

    public int ComplaintId { get; set; }

    public virtual Complaint Complaint { get; set; } = null!;

    public virtual Measurement Measurement { get; set; } = null!;
}
