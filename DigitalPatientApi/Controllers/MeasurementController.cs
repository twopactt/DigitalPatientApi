using DigitalPatientApi.DatabaseContext;
using DigitalPatientApi.Models;
using DigitalPatientApi.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DigitalPatientApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]

    public class MeasurementController : ControllerBase
    {
        private readonly DigitalClinicContext _db;

        public MeasurementController(DigitalClinicContext db)
        {
            _db = db;
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetMeasurementsByPatient(int patientId)
        {
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (currentUserRole == "пациент")
            {
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == currentUserId);

                if (patient == null || patient.Id != patientId)
                {
                    return StatusCode(403, new { success = false, message = "Доступ запрещен" });
                }
            }

            var measurements = await _db.Measurements
                .Include(m => m.MeasurementComplaints)
                    .ThenInclude(mc => mc.Complaint)
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.MeasuredAt)
                .Select(m => new
                {
                    m.Id,
                    m.PatientId,
                    m.SessionId,
                    m.MeasuredAt,
                    m.SystolicPressure,
                    m.DiastolicPressure,
                    m.HeartRate,
                    m.Spo2,
                    m.Temperature,
                    m.IsOffline,
                    Complaints = m.MeasurementComplaints.Select(mc => new
                    {
                        mc.ComplaintId,
                        mc.Complaint.Name
                    })
                })
                .ToListAsync();

            return Ok(new { success = true, data = measurements });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeasurementById(int id)
        {
            var measurement = await _db.Measurements
                .Include(m => m.MeasurementComplaints)
                    .ThenInclude(mc => mc.Complaint)
                .Include(m => m.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (measurement == null)
            {
                return NotFound(new { success = false, message = "Замер не найден" });
            }

            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (currentUserRole == "пациент")
            {
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == currentUserId);
                if (patient == null || patient.Id != measurement.PatientId)
                {
                    return StatusCode(403, new { success = false, message = "Доступ запрещён" });
                }
            }

            var result = new
            {
                measurement.Id,
                measurement.PatientId,
                PatientName = measurement.Patient.User.Surname + " " + measurement.Patient.User.Name,
                measurement.SessionId,
                measurement.MeasuredAt,
                measurement.SystolicPressure,
                measurement.DiastolicPressure,
                measurement.HeartRate,
                measurement.Spo2,
                measurement.Temperature,
                measurement.IsOffline,
                Complaints = measurement.MeasurementComplaints.Select(mc => new
                {
                    mc.ComplaintId,
                    mc.Complaint.Name
                })
            };

            return Ok(new { success = true, data = result });
        }


        [HttpPost]
        public async Task<IActionResult> CreateMeasurement([FromBody] CreateMeasurementRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
            }

            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var patient = await _db.Patients.FindAsync(request.PatientId);
            if (patient == null)
            {
                return BadRequest(new { success = false, message = "Пациент не найден" });
            }

            if (currentUserRole == "пациент")
            {
                var currentPatient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == currentUserId);
                if (currentPatient == null || currentPatient.Id != request.PatientId)
                {
                    return StatusCode(403, new { success = false, message = "Доступ запрещён: можно добавлять замеры только себе" });
                }
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var measurement = new Measurement
                {
                    PatientId = request.PatientId,
                    SessionId = request.SessionId,
                    MeasuredAt = DateTime.Now,
                    SystolicPressure = request.SystolicPressure,
                    DiastolicPressure = request.DiastolicPressure,
                    HeartRate = request.HeartRate,
                    Spo2 = request.Spo2,
                    Temperature = request.Temperature,
                    IsOffline = false
                };

                _db.Measurements.Add(measurement);
                await _db.SaveChangesAsync();

                if (request.ComplaintIds != null && request.ComplaintIds.Any())
                {
                    foreach (var complaintId in request.ComplaintIds.Distinct())
                    {
                        _db.MeasurementComplaints.Add(new MeasurementComplaint
                        {
                            MeasurementId = measurement.Id,
                            ComplaintId = complaintId
                        });
                    }
                    await _db.SaveChangesAsync();
                }

                _db.AuditLogs.Add(new AuditLog
                {
                    UserId = currentUserId,
                    ActionType = "create_measurement",
                    EntityType = "Measurement",
                    EntityId = measurement.Id,
                    OldValue = null,
                    NewValue = $"Создан замер для пациента {request.PatientId}",
                    CreatedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    success = true,
                    message = "Замер успешно создан",
                    data = new { measurementId = measurement.Id }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Ошибка при создании замера", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "врач,администратор")]
        public async Task<IActionResult> UpdateMeasurement(int id, [FromBody] UpdateMeasurementRequest request)
        {
            var measurement = await _db.Measurements
                .Include(m => m.MeasurementComplaints)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (measurement == null)
            {
                return NotFound(new { success = false, message = "Замер не найден" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (request.SystolicPressure.HasValue) measurement.SystolicPressure = request.SystolicPressure.Value;
                if (request.DiastolicPressure.HasValue) measurement.DiastolicPressure = request.DiastolicPressure.Value;
                if (request.HeartRate.HasValue) measurement.HeartRate = request.HeartRate.Value;
                if (request.Spo2.HasValue) measurement.Spo2 = request.Spo2.Value;
                if (request.Temperature.HasValue) measurement.Temperature = request.Temperature.Value;

                await _db.SaveChangesAsync();

                if (request.ComplaintIds != null)
                {
                    var oldComplaints = _db.MeasurementComplaints.Where(mc => mc.MeasurementId == id);
                    _db.MeasurementComplaints.RemoveRange(oldComplaints);
                    await _db.SaveChangesAsync();

                    foreach (var complaintId in request.ComplaintIds.Distinct())
                    {
                        _db.MeasurementComplaints.Add(new MeasurementComplaint
                        {
                            MeasurementId = measurement.Id,
                            ComplaintId = complaintId
                        });
                    }
                    await _db.SaveChangesAsync();
                }

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _db.AuditLogs.Add(new AuditLog
                {
                    UserId = currentUserId,
                    ActionType = "update_measurement",
                    EntityType = "Measurement",
                    EntityId = measurement.Id,
                    OldValue = null,
                    NewValue = $"Обновлён замер {id}",
                    CreatedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Замер обновлён" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении замера", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "врач,администратор")]
        public async Task<IActionResult> DeleteMeasurement(int id)
        {
            var measurement = await _db.Measurements.FindAsync(id);
            if (measurement == null)
            {
                return NotFound(new { success = false, message = "Замер не найден" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var complaints = _db.MeasurementComplaints.Where(mc => mc.MeasurementId == id);
                _db.MeasurementComplaints.RemoveRange(complaints);

                _db.Measurements.Remove(measurement);
                await _db.SaveChangesAsync();

                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                _db.AuditLogs.Add(new AuditLog
                {
                    UserId = currentUserId,
                    ActionType = "delete_measurement",
                    EntityType = "Measurement",
                    EntityId = id,
                    OldValue = null,
                    NewValue = $"Удалён замер {id}",
                    CreatedAt = DateTime.Now
                });
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Замер удалён" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Ошибка при удалении замера", error = ex.Message });
            }
        }
    }
}
