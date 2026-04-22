namespace DigitalPatientApi.RequestModels
{
    public class CreateMeasurementRequestModel
    {
        public int PatientId { get; set; }
        public int? SessionId { get; set; }
        public int SystolicPressure { get; set; }
        public int DiastolicPressure { get; set; }
        public int HeartRate { get; set; }
        public int? Spo2 { get; set; }
        public decimal? Temperature { get; set; }
        public List<int>? ComplaintIds { get; set; }
    }

    public class UpdateMeasurementRequest
    {
        public int? SystolicPressure { get; set; }
        public int? DiastolicPressure { get; set; }
        public int? HeartRate { get; set; }
        public int? Spo2 { get; set; }
        public decimal? Temperature { get; set; }
        public List<int>? ComplaintIds { get; set; }
    }
}
