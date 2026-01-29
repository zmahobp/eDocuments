using Microsoft.AspNetCore.Mvc;
using Services;
using Models;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassportsController : ControllerBase
    {
        private readonly IDocumentService<Passport> _documentService;

        public PassportsController(IDocumentService<Passport> documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetPassport(int userId)
        {
            var document = await _documentService.GetDocumentAsync(userId);
            if (document == null)
                return BadRequest("Passport not found for this user.");
            
            return Ok(document);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> AddPassport(int userId, [FromBody] Passport passport)
        {
            if (passport == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.AddDocumentAsync(userId, passport);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdatePassport(int userId, [FromBody] Passport passport)
        {
            if (passport == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.UpdateDocumentAsync(userId, passport);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeletePassport(int userId)
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
                return BadRequest("Passport not found for this user.");
            
            var passport = new Passport
            {
                PassportNumber = documentData.PassportNumber,
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
                var pdfBytes = await _documentService.GeneratePdfAsync(userId, passport);
                return File(pdfBytes, "application/pdf", "Passport.pdf");
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
                return BadRequest("Passport not found for this user.");
            
            try
            {
                var passport = new Passport { QRCode = documentData.QRCode };
                var qrBytes = await _documentService.GenerateQrCodeAsync(passport);
                return File(qrBytes, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}