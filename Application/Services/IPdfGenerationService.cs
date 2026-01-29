namespace Services
{
    public interface IPdfGenerationService
    {
        Task<byte[]> GenerateIdCardPdfAsync(dynamic documentData);
        Task<byte[]> GeneratePassportPdfAsync(dynamic documentData);
        Task<byte[]> GenerateDrivingLicensePdfAsync(dynamic documentData);
        Task<byte[]> GenerateWeaponPermitPdfAsync(dynamic documentData);
        Task<byte[]> GenerateVehicleRegistrationPdfAsync(dynamic documentData);
    }
}