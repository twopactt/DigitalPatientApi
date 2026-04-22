using DigitalPatientApi.Models;

namespace DigitalPatientApi.ResponceModels
{
    public class PatientAuthResponseModel
    {
        public User Patient { get; set; }
        public string Role { get; set; }
    }

    public class DoctorAuthResponseModel
    {
        public User Doctor { get; set; }
        public string Role { get; set; }
    }
}
