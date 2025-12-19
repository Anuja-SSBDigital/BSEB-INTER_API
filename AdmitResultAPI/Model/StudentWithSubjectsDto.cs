using AdmitResultAPI.Model.DTOs;

namespace AdmitResultAPI.Model
{
    public class StudentWithSubjectsDto
    {
        public DummyAdmitCardDto Student { get; set; }

        public List<SubjectPaperDto> Subjects { get; set; } = new();

        public bool HasVocationalSubjects { get; set; } // NEW
    }

    public class SubjectPaperDto
    {
        public int? Fk_SubjectPaperId { get; set; }
        public int? StudentId { get; set; }
        public string? SubjectName { get; set; }
        public int? SubjectPaperCode { get; set; }
        public int? FacultyId { get; set; }
        public string? SubjectGroup { get; set; }
        public string? PaperType { get; set; }
        public DateTime? ExamDate { get; set; }
        public string? ExamTime { get; set; }
        public string? ExamDay { get; set; }
        public string? ExamShift { get; set; }
 
    }
    public class SpResultDto
    {
        public int Result { get; set; }
    }
}
