using Microsoft.AspNetCore.Mvc;
using Services;
using Models;

namespace Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleRegistrationsController : ControllerBase
    {
        private readonly IDocumentService<VehicleRegistration> _documentService;

        public VehicleRegistrationsController(IDocumentService<VehicleRegistration> documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetVehicleRegistration(int userId)
        {
            var document = await _documentService.GetDocumentAsync(userId);
            if (document == null)
                return BadRequest("Vehicle registration not found for this user.");
            
            return Ok(document);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> AddVehicleRegistration(int userId, [FromBody] VehicleRegistration vehicleRegistration)
        {
            if (vehicleRegistration == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.AddDocumentAsync(userId, vehicleRegistration);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateVehicleRegistration(int userId, [FromBody] VehicleRegistration vehicleRegistration)
        {
            if (vehicleRegistration == null)
                return BadRequest("Invalid data.");
            
            var result = await _documentService.UpdateDocumentAsync(userId, vehicleRegistration);
            
            if (result.Contains("successfully"))
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteVehicleRegistration(int userId)
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
                return BadRequest("Vehicle registration not found for this user.");
            
            var vehicleRegistration = new VehicleRegistration
            {
                VehicleRegistrationNumber = documentData.VehicleRegistrationNumber,
                RegistrationNumber = documentData.RegistrationNumber,
                FirstRegistrationDate = documentData.FirstRegistrationDate,
                LoadCapacity = documentData.LoadCapacity,
                Weight = documentData.Weight,
                SeatNumber = documentData.SeatNumber,
                ProductionYear = documentData.ProductionYear,
                EngineNumber = documentData.EngineNumber,
                ChassisNumber = documentData.ChassisNumber,
                Brand = documentData.Brand,
                Type = documentData.Type,
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
                var pdfBytes = await _documentService.GeneratePdfAsync(userId, vehicleRegistration);
                return File(pdfBytes, "application/pdf", "VehicleRegistration.pdf");
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
                return BadRequest("Vehicle registration not found for this user.");
            
            try
            {
                var vehicleRegistration = new VehicleRegistration { QRCode = documentData.QRCode };
                var qrBytes = await _documentService.GenerateQrCodeAsync(vehicleRegistration);
                return File(qrBytes, "image/png");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}