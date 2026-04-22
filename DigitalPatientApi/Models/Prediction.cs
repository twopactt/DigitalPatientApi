using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Prediction
{
    public int Id { get; set; }

    public int TwinStateId { get; set; }

    public int ForecastHours { get; set; }

    public int RiskScore { get; set; }

    public string? Recommendation { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual TwinState TwinState { get; set; } = null!;
}
