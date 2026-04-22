using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Measurement
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int? SessionId { get; set; }

    public DateTime MeasuredAt { get; set; }

    public int SystolicPressure { get; set; }

    public int DiastolicPressure { get; set; }

    public int HeartRate { get; set; }

    public int? Spo2 { get; set; }

    public decimal? Temperature { get; set; }

    public bool IsOffline { get; set; }

    public virtual ICollection<MeasurementComplaint> MeasurementComplaints { get; set; } = new List<MeasurementComplaint>();

    public virtual Patient Patient { get; set; } = null!;
}
