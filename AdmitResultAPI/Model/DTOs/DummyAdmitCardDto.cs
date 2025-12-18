namespace AdmitResultAPI.Model.DTOs
{
    public class DummyAdmitCardDto
    {
        public int StudentID { get; set; }

        public string? StudentName { get; set; }

        public int? CollegeId { get; set; }

        public int? FacultyId { get; set; }

        public string? CollegeCode { get; set; }

        public int? ExamTypeId { get; set; }

        public string? FatherName { get; set; }

        public string? MotherName { get; set; }

        public string? DOB { get; set; }   // VARCHAR(10) from SQL

        public string? CategoryName { get; set; }

        public string? Faculty { get; set; }

        public string? College { get; set; }

        public string? OfssReferenceNo { get; set; }

        public bool? FormDownloaded { get; set; }

        public bool? IsRegCardUploaded { get; set; }

        public string? RegCardPath { get; set; }

        public string? PracticalExamCenterName { get; set; }

        public string? TheoryExamCenterName { get; set; }

        public string? RegistrationNo { get; set; }

        public string? RollCode { get; set; }

        public string? RollNumber { get; set; }

        public string? GenderName { get; set; }
    }
}
