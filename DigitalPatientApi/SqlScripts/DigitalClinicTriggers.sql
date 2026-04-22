-- Триггер 1: риск > 80 => отклонение
CREATE OR ALTER TRIGGER trg_CriticalRisk
ON [dbo].[TwinState]
AFTER INSERT
AS
BEGIN
    INSERT INTO [dbo].[Deviation] ([PatientId], [MeasurementId], [DeviationType], [Description], [DetectedAt])
    SELECT [PatientId], [MeasurementId], 'critical_risk', 'Риск > 80', GETDATE()
    FROM inserted WHERE [RiskIndex] > 80
END
GO

-- Проверка: добавляем риск 85
INSERT INTO [TwinState] ([PatientId], [MeasurementId], [RiskIndex]) VALUES (1, 1, 85)
SELECT * FROM [Deviation] WHERE [DeviationType] = 'critical_risk'



-- Триггер 2: нет замеров > 24ч => отклонение
CREATE OR ALTER TRIGGER trg_NoMeasurements
ON [dbo].[Measurement]
AFTER INSERT
AS
BEGIN
    INSERT INTO [dbo].[Deviation] ([PatientId], [DeviationType], [Description], [DetectedAt])
    SELECT p.[Id], 'no_measurements', 'Нет замеров > 24ч', GETDATE()
    FROM [dbo].[Patient] p
    WHERE p.[PatientStatusId] = 1
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[Measurement] m 
          WHERE m.[PatientId] = p.[Id] 
            AND m.[MeasuredAt] > DATEADD(HOUR, -24, GETDATE())
      )
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[Deviation] d 
          WHERE d.[PatientId] = p.[Id] 
            AND d.[DeviationType] = 'no_measurements' 
            AND d.[ResolvedAt] IS NULL
      )
END
GO

-- Проверка: добавляем полноценный замер
INSERT INTO [Measurement] ([PatientId], [MeasuredAt], [SystolicPressure], [DiastolicPressure], [HeartRate], [IsOffline])
VALUES (1, '2026-04-19', 120, 80, 70, 0)
SELECT * FROM [Deviation] WHERE [DeviationType] = 'no_measurements'



-- Триггер 3: изменение порога => аудит
CREATE OR ALTER TRIGGER trg_ThresholdAudit
ON [dbo].[NotificationThreshold]
AFTER UPDATE
AS
BEGIN
    INSERT INTO [dbo].[AuditLog] ([UserId], [ActionType], [EntityType], [EntityId], [OldValue], [NewValue], [CreatedAt])
    SELECT i.[UpdatedBy], 'update_threshold', 'Patient', i.[PatientId], CAST(d.[CriticalRiskThreshold] AS NVARCHAR), CAST(i.[CriticalRiskThreshold] AS NVARCHAR), GETDATE()
    FROM inserted i INNER JOIN deleted d ON i.[Id] = d.[Id]
    WHERE i.[CriticalRiskThreshold] != d.[CriticalRiskThreshold]
END
GO

-- Проверка: меняем порог с 70 на 50
UPDATE [NotificationThreshold] SET [CriticalRiskThreshold] = 50, [UpdatedBy] = 1 WHERE [PatientId] = 1
SELECT * FROM [AuditLog] WHERE [ActionType] = 'update_threshold'