using AdmitResultAPI.Model;
using AdmitResultAPI.Model.DTOs;

namespace AdmitResultAPI.Services.IServices
{
    public interface IPracticaladmitcardService
    {
        //Task<List<StudentWithSubjectsDto>> Practicaladmitcard(DummyAdmitCardRequestDto dummyAdmit);
        Task<PracticaladmitcardService.ApiResponse<List<StudentWithSubjectsDto>>> Practicaladmitcard(DummyAdmitCardRequestDto dummyAdmit);

    }
}
