using Microsoft.AspNetCore.Mvc;
using Services;
using Models;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrivingLicensesController : ControllerBase
    {
        private readonly IDocumentService<DrivingLicense> _documentService;

        public DrivingLicensesController(IDocumentService<DrivingLicense> documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetDrivingLicense(int userId)
        {
            var document = await _documentService.GetDocumentAsync(userId);
            if (document == null)
                return BadRequest("Driving license not found for this user.");
            
            return Ok(document);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> AddDrivingLicense(int userId, [FromBody] DrivingLicense drivingLicense)
        {
            if (drivingLicense == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.AddDocumentAsync(userId, drivingLicense);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateDrivingLicense(int userId, [FromBody] DrivingLicense drivingLicense)
        {
            if (drivingLicense == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.UpdateDocumentAsync(userId, drivingLicense);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteDrivingLicense(int userId)
        {
            var result = await _documentService.DeleteDocumentAsync(userId);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("pdf/{userId}")]
        public async Task<ActionResult> GeneratePdf(int userId)
        {
            var documentData = await _documentService.GetDocumentAsync(userId);
            if (documentData == null)
                return BadRequest("Driving license not found for this user.");
            
            var drivingLicense = new DrivingLicense
            {
                DrivingLicenseNumber = documentData.DrivingLicenseNumber,
                VehicleCategories = documentData.VehicleCategories,
                IssueDate = documentData.IssueDate,
                ExpirationDate = documentData.ExpiryDate,
                IssuedBy = documentData.IssuedBy,
                QRCode = documentData.QRCode,
                User = new RegularUser
                {
                    JMBG = documentData.JMBG,
                    FirstName = documentData.FirstName,
                    ParentName = documentData.ParentName,
                    LastName = documentData.LastName,
                    City = documentData.City,
                    Municipality = documentData.Municipality,
                    Street = documentData.Street,
                    HouseNumber = documentData.HouseNumber,
                    BirthDate = documentData.BirthDate,
                    BirthPlace = documentData.BirthPlace,
                    Gender = documentData.Gender,
                    Photo = documentData.Photo
                }
            };

            try
            {
                var pdfBytes = await _documentService.GeneratePdfAsync(userId, drivingLicense);
                return File(pdfBytes, "application/pdf", "DrivingLicense.pdf");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("qr/{userId}")]
        public async Task<ActionResult> GenerateQrCode(int userId)
        {
            var documentData = await _documentService.GetDocumentAsync(userId);
            if (documentData == null)
                return BadRequest("Driving license not found for this user.");
            
            try
            {
                var drivingLicense = new DrivingLicense { QRCode = documentData.QRCode };
                var qrBytes = await _documentService.GenerateQrCodeAsync(drivingLicense);
                return File(qrBytes, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}