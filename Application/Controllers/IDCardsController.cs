using Microsoft.AspNetCore.Mvc;
using Services;
using Models;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IDCardsController : ControllerBase
    {
        private readonly IDocumentService<IDCard> _documentService;

        public IDCardsController(IDocumentService<IDCard> documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetIdCard(int userId)
        {
            var document = await _documentService.GetDocumentAsync(userId);
            if (document == null)
                return BadRequest("ID card not found for this user.");
            
            return Ok(document);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> AddIdCard(int userId, [FromBody] IDCard idCard)
        {
            if (idCard == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.AddDocumentAsync(userId, idCard);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateIdCard(int userId, [FromBody] IDCard idCard)
        {
            if (idCard == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.UpdateDocumentAsync(userId, idCard);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteIdCard(int userId)
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
                return BadRequest("ID card not found for this user.");
            
            var idCard = new IDCard
            {
                IdCardNumber = documentData.IdCardNumber,
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
                var pdfBytes = await _documentService.GeneratePdfAsync(userId, idCard);
                return File(pdfBytes, "application/pdf", "ID_card.pdf");
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
                return BadRequest("ID card not found for this user.");
            
            try
            {
                var idCard = new IDCard { QRCode = documentData.QRCode };
                var qrBytes = await _documentService.GenerateQrCodeAsync(idCard);
                return File(qrBytes, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}