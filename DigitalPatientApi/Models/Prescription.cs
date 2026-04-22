using System;
using System.Collections.Generic;

namespace DigitalPatientApi.Models;

public partial class Prescription
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public string? ActionDescription { get; set; }

    public int? MedicationId { get; set; }

    public int? DoseUnitId { get; set; }

    public int? FrequencyId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? InstructionId { get; set; }

    public int PrescriptionStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User Doctor { get; set; } = null!;

    public virtual DoseUnit? DoseUnit { get; set; }

    public virtual Frequency? Frequency { get; set; }

    public virtual Instruction? Instruction { get; set; }

    public virtual Medication? Medication { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<PrescriptionCompletion> PrescriptionCompletions { get; set; } = new List<PrescriptionCompletion>();

    public virtual PrescriptionStatus PrescriptionStatus { get; set; } = null!;
}
