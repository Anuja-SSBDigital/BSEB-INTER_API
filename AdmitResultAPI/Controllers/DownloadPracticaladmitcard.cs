using AdmitResultAPI.Model;
using AdmitResultAPI.Model.DTOs;
using AdmitResultAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace AdmitResultAPI.Controllers
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadPracticaladmitcard : ControllerBase
    {
        private readonly IPracticaladmitcardService _service;

        public DownloadPracticaladmitcard(IPracticaladmitcardService service)
        {
            _service = service;
        }

        [HttpPost("Practicaladmitcard")]
        public async Task<IActionResult> Practicaladmitcard([FromForm] DummyAdmitCardRequestDto dummyAdmit)
        {
            var response = await _service.Practicaladmitcard(dummyAdmit);

            if (!response.Success || response.Data == null || response.Data.Count == 0)
            {
                return BadRequest(new { success = false, message = response.Message ?? "No data found" });
            }

            var pdf = new InterPracticaladmitPdf(response.Data); // pass the actual list
            var pdfBytes = pdf.GeneratePdf();

            return File(pdfBytes, "application/pdf", "Practicaladmitcard.pdf");
        }

        //[AllowAnonymous]
        //[HttpPost("Practicaladmitcard")]
        //public async Task<IActionResult> Practicaladmitcard([FromForm] DummyAdmitCardRequestDto dummyAdmit)
        //{
        //    var result = await _service.Practicaladmitcard(dummyAdmit);

        //    // return Ok(new { success = true, data = result });
        //    var pdf = new InterPracticaladmitPdf(result);
        //    var pdfBytes = pdf.GeneratePdf();

        //    return File(pdfBytes, "application/pdf", "Practicaladmitcard.pdf");
        //}

    }
}
