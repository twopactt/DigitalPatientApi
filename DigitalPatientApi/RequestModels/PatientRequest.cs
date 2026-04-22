namespace DigitalPatientApi.RequestModels
{
    public class CreatePatientRequest
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? Patronymic { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public int GenderId { get; set; }
        public List<int>? ChronicDiseaseIds { get; set; }
    }

    public class UpdatePatientRequest
    {
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public DateTime? Birthday { get; set; }
        public int? GenderId { get; set; }
        public int? PatientStatusId { get; set; }
    }
}
