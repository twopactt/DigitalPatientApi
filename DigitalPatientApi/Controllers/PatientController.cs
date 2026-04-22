using DigitalPatientApi.DatabaseContext;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAll()
        {
            var patients = _db.Patients.ToList();
            return Ok(patients);
        }
    }
}
