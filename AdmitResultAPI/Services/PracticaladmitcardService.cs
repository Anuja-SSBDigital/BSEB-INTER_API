using AdmitResultAPI.Data;
using AdmitResultAPI.Model;
using AdmitResultAPI.Model.DTOs;
using AdmitResultAPI.Services.IServices;
using Azure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace AdmitResultAPI.Services
{
    public class PracticaladmitcardService : IPracticaladmitcardService
    {
        private readonly AppDbContext _context;

        public PracticaladmitcardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<StudentWithSubjectsDto>>> Practicaladmitcard(DummyAdmitCardRequestDto dummyAdmit)
        {
            var response = new ApiResponse<List<StudentWithSubjectsDto>>();
            try
            {
                var result = new List<StudentWithSubjectsDto>();

                var students = await _context.DummyAdmitCardDto
                             .FromSqlRaw(
                                 "EXEC sp_GetDummyAdmitCardByRegRoll @RegistrationNo, @RollCode, @RollNo",
                                 new SqlParameter("@RegistrationNo", string.IsNullOrWhiteSpace(dummyAdmit.RegistrationNo) ? (object)DBNull.Value : dummyAdmit.RegistrationNo),
                                 new SqlParameter("@RollCode", (dummyAdmit.RollCode == null || dummyAdmit.RollCode == 0) ? (object)DBNull.Value : dummyAdmit.RollCode.Value),
                                 new SqlParameter("@RollNo", string.IsNullOrWhiteSpace(dummyAdmit.RollNo) ? (object)DBNull.Value : dummyAdmit.RollNo))
                             .ToListAsync();

                if (students == null || students.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No data found for the given input.";
                    return response;
                }

                foreach (var student in students)
                {
                    var subjects = await _context.SubjectPaperEntities
                        .FromSqlRaw(
                            "EXEC GetStudentExamAddmitCardSubjectDetails " +
                            "@studentID, @facultyId, @CollegeId, @ExamTypeId, @IsPractical",
                            new SqlParameter("@studentID", student.StudentID),
                            new SqlParameter("@facultyId", student.FacultyId),
                            new SqlParameter("@CollegeId", student.CollegeId),
                            new SqlParameter("@ExamTypeId", student.ExamTypeId),
                            new SqlParameter("@IsPractical", true)
                        )
                        .ToListAsync();

                    var isExists = await CheckVocationalCollegeSubjectExistsAsync(Convert.ToInt32(student.CollegeId), Convert.ToInt32(student.FacultyId));

                    result.Add(new StudentWithSubjectsDto
                    {
                        Student = student,
                        Subjects = subjects,
                        HasVocationalSubjects = isExists == 1
                    });
                }

                response.Data = result;
                return response;
            }
            catch (SqlException sqlEx)
            {
                response.Success = false;
                response.Message = sqlEx.Message;
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while fetching the admit card details: " + ex.Message;
                return response;
            }
        }


        public async Task<int> CheckVocationalCollegeSubjectExistsAsync(int fkCollegeId, int fkFacultyId)
        {
            try
            {
                var collegeId = new SqlParameter("@Fk_CollegeId", fkCollegeId);
                var facultyId = new SqlParameter("@Fk_FacultyId", fkFacultyId);

                // Execute SP and materialize the results first
                var resultList = await _context.SpResultDto
                .FromSqlRaw("EXEC sp_CheckVocationalCollegeSubjectExists @Fk_CollegeId, @Fk_FacultyId",
                    collegeId, facultyId)
                .ToListAsync();

                return resultList.FirstOrDefault()?.Result ?? 0;

            }
            catch (Exception e)
            {
                throw;
            }
        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; } = true;
            public string Message { get; set; } = string.Empty;
            public T Data { get; set; }
        }

    }
}
