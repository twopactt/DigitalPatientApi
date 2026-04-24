using DigitalPatientApi.DatabaseContext;
using DigitalPatientApi.RequestModels;
using DigitalPatientApi.ResponceModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DigitalPatientApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly DigitalClinicContext _db;

        public AuthController(DigitalClinicContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequestModel request)
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == request.Login && u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized(ApiResponse<object>.Error("Неверный логин или пароль", "INVALID_CREDENTIALS"));
            }

            int? patientId = null;
            if (user.RoleId == 1)
            {
                var patient = await _db.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
                patientId = patient?.Id;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("FullName", $"{user.Surname} {user.Name} {user.Patronymic ?? ""}".Trim())
            };

            if (patientId.HasValue)
            {
                claims.Add(new Claim("PatientId", patientId.Value.ToString()));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties
            );

            return Ok(ApiResponse<object>.Ok(new
            {
                userId = user.Id,
                role = user.Role.Name,
                fullName = $"{user.Surname} {user.Name} {user.Patronymic ?? ""}".Trim(),
                patientId = patientId
            }, "Вход выполнен успешно"));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse<object>.Ok(null, "Выход выполнен"));
        }
    }
}
