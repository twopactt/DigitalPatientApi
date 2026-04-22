using System;
using System.Collections.Generic;
using DigitalPatientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalPatientApi.DatabaseContext;

public partial class DigitalClinicContext : DbContext
{
    public DigitalClinicContext(DbContextOptions<DigitalClinicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<ChronicDisease> ChronicDiseases { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Deviation> Deviations { get; set; }

    public virtual DbSet<DoseUnit> DoseUnits { get; set; }

    public virtual DbSet<Frequency> Frequencies { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Instruction> Instructions { get; set; }

    public virtual DbSet<Measurement> Measurements { get; set; }

    public virtual DbSet<MeasurementComplaint> MeasurementComplaints { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<MonitoringSession> MonitoringSessions { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationThreshold> NotificationThresholds { get; set; }

    public virtual DbSet<NotificationType> NotificationTypes { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientChronicDisease> PatientChronicDiseases { get; set; }

    public virtual DbSet<PatientStatus> PatientStatuses { get; set; }

    public virtual DbSet<Prediction> Predictions { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionCompletion> PrescriptionCompletions { get; set; }

    public virtual DbSet<PrescriptionCompletionStatus> PrescriptionCompletionStatuses { get; set; }

    public virtual DbSet<PrescriptionStatus> PrescriptionStatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Trend> Trends { get; set; }

    public virtual DbSet<TwinState> TwinStates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-KSRNQI5;Initial Catalog=DigitalClinic;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLog");

            entity.Property(e => e.ActionType).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EntityType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AuditLog_User");
        });

        modelBuilder.Entity<ChronicDisease>(entity =>
        {
            entity.ToTable("ChronicDisease");

            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.ToTable("Complaint");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Deviation>(entity =>
        {
            entity.ToTable("Deviation");

            entity.Property(e => e.DetectedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DeviationType).HasMaxLength(100);
            entity.Property(e => e.ResolvedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ResolvedByNavigation).WithMany(p => p.Deviations)
                .HasForeignKey(d => d.ResolvedBy)
                .HasConstraintName("FK_Deviation_User");
        });

        modelBuilder.Entity<DoseUnit>(entity =>
        {
            entity.ToTable("DoseUnit");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Frequency>(entity =>
        {
            entity.ToTable("Frequency");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.ToTable("Gender");

            entity.Property(e => e.Name).HasMaxLength(7);
        });

        modelBuilder.Entity<Instruction>(entity =>
        {
            entity.ToTable("Instruction");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.ToTable("Measurement", tb => tb.HasTrigger("trg_NoMeasurements"));

            entity.Property(e => e.IsOffline).HasDefaultValue(true);
            entity.Property(e => e.MeasuredAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Temperature).HasColumnType("decimal(3, 1)");

            entity.HasOne(d => d.Patient).WithMany(p => p.Measurements)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Measurement_Patient");
        });

        modelBuilder.Entity<MeasurementComplaint>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_MeasurementComplaint_1");

            entity.ToTable("MeasurementComplaint");

            entity.HasOne(d => d.Complaint).WithMany(p => p.MeasurementComplaints)
                .HasForeignKey(d => d.ComplaintId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeasurementComplaint_Complaint");

            entity.HasOne(d => d.Measurement).WithMany(p => p.MeasurementComplaints)
                .HasForeignKey(d => d.MeasurementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeasurementComplaint_Measurement");
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.ToTable("Medication");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<MonitoringSession>(entity =>
        {
            entity.ToTable("MonitoringSession");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.StartDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Patient).WithMany(p => p.MonitoringSessions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonitoringSession_Patient");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notification");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SentAt).HasColumnType("datetime");

            entity.HasOne(d => d.NotificationType).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.NotificationTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_NotificationType");

            entity.HasOne(d => d.Patient).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_Notification_Patient");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<NotificationThreshold>(entity =>
        {
            entity.ToTable("NotificationThreshold", tb => tb.HasTrigger("trg_ThresholdAudit"));

            entity.Property(e => e.CriticalRiskThreshold).HasDefaultValue(70);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Patient).WithMany(p => p.NotificationThresholds)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotificationThreshold_Patient");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.NotificationThresholds)
                .HasForeignKey(d => d.UpdatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotificationThreshold_User");
        });

        modelBuilder.Entity<NotificationType>(entity =>
        {
            entity.ToTable("NotificationType");

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.ToTable("Patient");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Gender).WithMany(p => p.Patients)
                .HasForeignKey(d => d.GenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_Gender");

            entity.HasOne(d => d.PatientStatus).WithMany(p => p.Patients)
                .HasForeignKey(d => d.PatientStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_Status");

            entity.HasOne(d => d.User).WithMany(p => p.Patients)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Patient_User");
        });

        modelBuilder.Entity<PatientChronicDisease>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PatientChronicDisease_1");

            entity.ToTable("PatientChronicDisease");

            entity.HasOne(d => d.ChronicDisease).WithMany(p => p.PatientChronicDiseases)
                .HasForeignKey(d => d.ChronicDiseaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PatientChronicDisease_ChronicDisease");

            entity.HasOne(d => d.Patient).WithMany(p => p.PatientChronicDiseases)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PatientChronicDisease_Patient");
        });

        modelBuilder.Entity<PatientStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Status");

            entity.ToTable("PatientStatus");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("Prediction");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.TwinState).WithMany(p => p.Predictions)
                .HasForeignKey(d => d.TwinStateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prediction_TwinState");
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescription");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_User");

            entity.HasOne(d => d.DoseUnit).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.DoseUnitId)
                .HasConstraintName("FK_Prescription_DoseUnit");

            entity.HasOne(d => d.Frequency).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.FrequencyId)
                .HasConstraintName("FK_Prescription_Frequency");

            entity.HasOne(d => d.Instruction).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.InstructionId)
                .HasConstraintName("FK_Prescription_Instruction");

            entity.HasOne(d => d.Medication).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.MedicationId)
                .HasConstraintName("FK_Prescription_Medication");

            entity.HasOne(d => d.Patient).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_Patient");

            entity.HasOne(d => d.PrescriptionStatus).WithMany(p => p.Prescriptions)
                .HasForeignKey(d => d.PrescriptionStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prescription_PrescriptionStatus");
        });

        modelBuilder.Entity<PrescriptionCompletion>(entity =>
        {
            entity.ToTable("PrescriptionCompletion");

            entity.Property(e => e.CompletedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.PrescriptionCompletionStatus).WithMany(p => p.PrescriptionCompletions)
                .HasForeignKey(d => d.PrescriptionCompletionStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionCompletion_PrescriptionCompletionStatus");

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionCompletions)
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PrescriptionCompletion_Prescription");
        });

        modelBuilder.Entity<PrescriptionCompletionStatus>(entity =>
        {
            entity.ToTable("PrescriptionCompletionStatus");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<PrescriptionStatus>(entity =>
        {
            entity.ToTable("PrescriptionStatus");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Trend>(entity =>
        {
            entity.ToTable("Trend");

            entity.Property(e => e.Name).HasMaxLength(20);
        });

        modelBuilder.Entity<TwinState>(entity =>
        {
            entity.ToTable("TwinState", tb => tb.HasTrigger("trg_CriticalRisk"));

            entity.Property(e => e.CalculatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ModelVersion)
                .HasMaxLength(10)
                .HasDefaultValueSql("((1.0))");

            entity.HasOne(d => d.Patient).WithMany(p => p.TwinStates)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TwinState_Patient");

            entity.HasOne(d => d.Trend).WithMany(p => p.TwinStates)
                .HasForeignKey(d => d.TrendId)
                .HasConstraintName("FK_TwinState_Trend");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Login).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Patronymic).HasMaxLength(30);
            entity.Property(e => e.Surname).HasMaxLength(30);

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_User_Department");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
