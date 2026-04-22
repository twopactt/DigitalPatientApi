using DigitalPatientApi.DatabaseContext;
using DigitalPatientApi.Models;
using DigitalPatientApi.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalPatientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PatientController : ControllerBase
    {
        private readonly DigitalClinicContext _db;
        
        public PatientController(DigitalClinicContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await _db.Patients
                .Include(p => p.User)
                .Include(p => p.Gender)
                .Include(p => p.PatientStatus)
                .Select(p => new
                {
                    p.Id,
                    p.UserId,
                    FullName = p.User.Surname + " " + p.User.Name + " " + (p.User.Patronymic ?? ""),
                    p.Birthday,
                    Age = DateTime.Now.Year - p.Birthday.Year,
                    Gender = p.Gender.Name,
                    p.PatientStatusId,
                    Status = p.PatientStatus.Name,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .ToListAsync();

            return Ok(new { success = true, data = patients });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _db.Patients
                .Include(p => p.User)
                .Include(p => p.Gender)
                .Include(p => p.PatientStatus)
                .Include(p => p.PatientChronicDiseases)
                    .ThenInclude(pcd => pcd.ChronicDisease)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound(new { success = false, message = "Пациент не найден" });
            }

            var result = new
            {
                patient.Id,
                patient.UserId,
                Surname = patient.User.Surname,
                Name = patient.User.Name,
                Patronymic = patient.User.Patronymic,
                FullName = patient.User.Surname + " " + patient.User.Name + " " + (patient.User.Patronymic ?? ""),
                patient.Birthday,
                Age = DateTime.Now.Year - patient.Birthday.Year,
                GenderName = patient.Gender.Name,
                patient.PatientStatusId,
                Status = patient.PatientStatus.Name,
                ChronicDiseases = patient.PatientChronicDiseases.Select(pcd => new
                {
                    pcd.ChronicDiseaseId,
                    pcd.ChronicDisease.Name
                }),
                patient.CreatedAt,
                patient.UpdatedAt
            };

            return Ok(new { success = true, data = result });
        }


        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Неверные данные", errors = ModelState });
            }

            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
            if (existingUser != null)
            {
                return BadRequest(new { success = false, message = "Пользователь с таким логином уже существует" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var user = new User
                {
                    Surname = request.Surname,
                    Name = request.Name,
                    Patronymic = request.Patronymic,
                    Login = request.Login,
                    Password = request.Password,
                    RoleId = 1,
                    DepartmentId = null
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                var patient = new Patient
                {
                    UserId = user.Id,
                    Birthday = request.Birthday,
                    GenderId = request.GenderId,
                    PatientStatusId = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _db.Patients.Add(patient);
                await _db.SaveChangesAsync();

                if (request.ChronicDiseaseIds != null && request.ChronicDiseaseIds.Any())
                {
                    foreach (var diseaseId in request.ChronicDiseaseIds)
                    {
                        _db.PatientChronicDiseases.Add(new PatientChronicDisease
                        {
                            PatientId = patient.Id,
                            ChronicDiseaseId = diseaseId
                        });
                    }

                    await _db.SaveChangesAsync();
                }

                _db.AuditLogs.Add(new AuditLog
                {
                    UserId = GetCurrentUserId(),
                    ActionType = "create_patient",
                    EntityType = "Patient",
                    EntityId = patient.Id,
                    OldValue = null,
                    NewValue = $"Создан пациент {request.Surname} {request.Name}",
                    CreatedAt = DateTime.Now
                });

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Пациент успешно создан", data = new { patientId = patient.Id, userId = user.Id } });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Ошибка при создании пациента", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientRequest request)
        {
            var patient = await _db.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return NotFound(new { success = false, message = "Пациент не найден" });
            }

            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                if (!string.IsNullOrEmpty(request.Surname)) patient.User.Surname = request.Surname;
                if (!string.IsNullOrEmpty(request.Name)) patient.User.Name = request.Name;
                if (request.Patronymic != null) patient.User.Patronymic = request.Patronymic;
                if (request.Birthday.HasValue) patient.Birthday = request.Birthday.Value;
                if (request.GenderId.HasValue) patient.GenderId = request.GenderId.Value;
                if (request.PatientStatusId.HasValue) patient.PatientStatusId = request.PatientStatusId.Value;

                patient.UpdatedAt = DateTime.Now;

                await _db.SaveChangesAsync();

                _db.AuditLogs.Add(new AuditLog
                {
                    UserId = GetCurrentUserId(),
                    ActionType = "update_patient",
                    EntityType = "Patient",
                    EntityId = patient.Id,
                    OldValue = null,
                    NewValue = $"Обновлены данные пациента {patient.User.Surname} {patient.User.Name}",
                    CreatedAt = DateTime.Now
                });

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { success = true, message = "Данные пациента обновлены" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { success = false, message = "Ошибка при обновлении", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _db.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound(new { success = false, message = "Пациент не найден" });
            }

            patient.PatientStatusId = 2;
            patient.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            _db.AuditLogs.Add(new AuditLog
            {
                UserId = GetCurrentUserId(),
                ActionType = "archive_patient",
                EntityType = "Patient",
                EntityId = patient.Id,
                OldValue = null,
                NewValue = $"Пациент архивирован",
                CreatedAt = DateTime.Now
            });

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Пациент архивирован" });
        }

        private int GetCurrentUserId()
        {
            return 1;
        }
    }
}
