namespace AdmitResultAPI.Model.DTOs
{
    public class DummyAdmitCardDto
    {
        public int StudentID { get; set; } // Assuming PK is NOT NULL
        public string StudentName { get; set; } = string.Empty;
        public int? CollegeId { get; set; }         // Nullable int
        public int? FacultyId { get; set; }         // Nullable int
        public string CollegeCode { get; set; } = string.Empty;
        public int? ExamTypeId { get; set; }        // Nullable int
        public string? FatherName { get; set; }


        public string? MotherName { get; set; }
        public string? DOB { get; set; }
        public string? CategoryName { get; set; }
        //public string? Faculty { get; set; }
        public string? College { get; set; }
        public string? OfssReferenceNo { get; set; }
     
        //public bool? FormDownloaded { get; set; }   // Nullable bool
        //public bool? IsRegCardUploaded { get; set; } // Nullable bool
        //public string? RegCardPath { get; set; }
        public string? PracticalExamCenterName { get; set; }
        public string? TheoryExamCenterName { get; set; }
        public string? RegistrationNo { get; set; }
        public int? RollCode { get; set; }          // Nullable int
        public string? RollNumber { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? MaritalStatus { get; set; }
        public string? UniqueNo { get; set; }
        public string? FacultyName { get; set; }
        public string? Caste { get; set; }
        public string? StudentPhotoPath { get; set; }
        public string? StudentSignaturePath { get; set; }
        public string? CollegeName { get; set; }
        public string? AadharNo { get; set; }
        public string? ExamTypeName { get; set; }

        public bool? Disability { get; set; } // Nullable bool
    }


}
