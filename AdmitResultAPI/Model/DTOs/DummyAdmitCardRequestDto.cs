namespace AdmitResultAPI.Model.DTOs
{
    public class DummyAdmitCardRequestDto
    {
        public string? RegistrationNo { get; set; }
        public string? RollCode { get; set; }
        public string? RollNo { get; set; }

        public int? IsPractical { get; set; }
        public int? IsTheory { get; set; }
    }
}
