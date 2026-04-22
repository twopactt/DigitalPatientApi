using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class TwinState
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int MeasurementId { get; set; }

    public DateTime CalculatedAt { get; set; }

    public int RiskIndex { get; set; }

    public string ModelVersion { get; set; } = null!;

    public int? TrendId { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

    public virtual Trend? Trend { get; set; }
}
