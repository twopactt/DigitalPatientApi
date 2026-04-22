using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Patient
{
    public int Id { get; set; }

    public DateTime Birthday { get; set; }

    public int GenderId { get; set; }

    public int UserId { get; set; }

    public int PatientStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Gender Gender { get; set; } = null!;

    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

    public virtual ICollection<MonitoringSession> MonitoringSessions { get; set; } = new List<MonitoringSession>();

    public virtual ICollection<NotificationThreshold> NotificationThresholds { get; set; } = new List<NotificationThreshold>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PatientChronicDisease> PatientChronicDiseases { get; set; } = new List<PatientChronicDisease>();

    public virtual PatientStatus PatientStatus { get; set; } = null!;

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();

    public virtual ICollection<TwinState> TwinStates { get; set; } = new List<TwinState>();

    public virtual User User { get; set; } = null!;
}
