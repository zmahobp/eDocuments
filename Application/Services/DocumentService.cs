using Microsoft.EntityFrameworkCore;
using Models;
using DocModel = Models.Document;

namespace Services
{
    public class DocumentService<T> : IDocumentService<T> where T : DocModel
    {
        private readonly DatabaseContext _context;
        private readonly IPdfGenerationService _pdfService;

        public DocumentService(DatabaseContext context, IPdfGenerationService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        public async Task<object> GetDocumentAsync(int userId)
        {
            var documentType = typeof(T).Name;
            
            try
            {
                switch (documentType)
                {
                    case "IDCard":
                        var idCard = await _context.IDCards
                            .Where(l => l.UserId == userId)
                            .Select(l => new
                            {
                                IdCardNumber = l.IdCardNumber,
                                IssueDate = l.IssueDate,
                                ExpiryDate = l.ExpirationDate,
                                IssuedBy = l.IssuedBy,

                                JMBG = l.User.JMBG,
                                FirstName = l.User.FirstName,
                                ParentName = l.User.ParentName,
                                LastName = l.User.LastName,
                                City = l.User.City,
                                Municipality = l.User.Municipality,
                                Street = l.User.Street,
                                HouseNumber = l.User.HouseNumber,
                                BirthDate = l.User.BirthDate,
                                BirthPlace = l.User.BirthPlace,
                                Gender = l.User.Gender,
                                Photo = l.User.Photo,
                                QRCode = l.GenerateQRCode()
                            }).FirstOrDefaultAsync();
                        return idCard ?? null;

                    case "Passport":
                        var passport = await _context.Passports
                            .Where(l => l.UserId == userId)
                            .Select(l => new
                            {
                                PassportNumber = l.PassportNumber,
                                IssueDate = l.IssueDate,
                                ExpiryDate = l.ExpirationDate,
                                IssuedBy = l.IssuedBy,

                                JMBG = l.User.JMBG,
                                FirstName = l.User.FirstName,
                                ParentName = l.User.ParentName,
                                LastName = l.User.LastName,
                                City = l.User.City,
                                Municipality = l.User.Municipality,
                                Street = l.User.Street,
                                HouseNumber = l.User.HouseNumber,
                                BirthDate = l.User.BirthDate,
                                BirthPlace = l.User.BirthPlace,
                                Gender = l.User.Gender,
                                Photo = l.User.Photo,
                                QRCode = l.GenerateQRCode()
                            }).FirstOrDefaultAsync();
                        return passport ?? null;

                    case "DrivingLicense":
                        var drivingLicense = await _context.DrivingLicenses
                            .Where(l => l.UserId == userId)
                            .Select(l => new
                            {
                                DrivingLicenseNumber = l.DrivingLicenseNumber,
                                VehicleCategories = l.VehicleCategories,
                                Categories = l.VehicleCategories,
                                IssueDate = l.IssueDate,
                                ExpiryDate = l.ExpirationDate,
                                IssuedBy = l.IssuedBy,
                                IssuingAuthority = l.IssuedBy,

                                JMBG = l.User.JMBG,
                                FirstName = l.User.FirstName,
                                ParentName = l.User.ParentName,
                                LastName = l.User.LastName,
                                City = l.User.City,
                                Municipality = l.User.Municipality,
                                Street = l.User.Street,
                                HouseNumber = l.User.HouseNumber,
                                BirthDate = l.User.BirthDate,
                                BirthPlace = l.User.BirthPlace,
                                Gender = l.User.Gender,
                                Photo = l.User.Photo,
                                QRCode = l.GenerateQRCode()
                            }).FirstOrDefaultAsync();
                        return drivingLicense ?? null;

                    case "WeaponPermit":
                        var weaponPermit = await _context.WeaponPermits
                            .Where(l => l.UserId == userId)
                            .Select(l => new
                            {
                                WeaponPermitNumber = l.WeaponPermitNumber,
                                WeaponTypes = l.WeaponTypes,
                                WeaponType = l.WeaponTypes,
                                WeaponQuantity = l.WeaponQuantity,
                                WeaponTypeCount = l.WeaponTypeCount,
                                WeaponCaliber = l.WeaponCaliber,
                                UsageLocation = l.UsageLocation,
                                UsagePurpose = l.UsagePurpose,
                                ReasonForIssuing = l.UsagePurpose,
                                IssueDate = l.IssueDate,
                                ExpiryDate = l.ExpirationDate,
                                IssuedBy = l.IssuedBy,

                                JMBG = l.User.JMBG,
                                FirstName = l.User.FirstName,
                                ParentName = l.User.ParentName,
                                LastName = l.User.LastName,
                                City = l.User.City,
                                Municipality = l.User.Municipality,
                                Street = l.User.Street,
                                HouseNumber = l.User.HouseNumber,
                                BirthDate = l.User.BirthDate,
                                BirthPlace = l.User.BirthPlace,
                                Gender = l.User.Gender,
                                Photo = l.User.Photo,
                                QRCode = l.GenerateQRCode()
                            }).FirstOrDefaultAsync();
                        return weaponPermit ?? null;

                    case "VehicleRegistration":
                        var vehicleRegistration = await _context.VehicleRegistrations
                            .Where(l => l.UserId == userId)
                            .Select(l => new
                            {
                                VehicleRegistrationNumber = l.VehicleRegistrationNumber,
                                RegistrationNumber = l.RegistrationNumber,
                                FirstRegistrationDate = l.FirstRegistrationDate,
                                LoadCapacity = l.LoadCapacity,
                                Weight = l.Weight,
                                SeatNumber = l.SeatNumber,
                                ProductionYear = l.ProductionYear,
                                EngineNumber = l.EngineNumber,
                                EngineSerialNumber = l.EngineNumber,
                                ChassisNumber = l.ChassisNumber,
                                Brand = l.Brand,
                                VehicleBrand = l.Brand,
                                Type = l.Type,
                                VehicleType = l.Type,
                                ProductionYearForPdf = l.ProductionYear,
                                IssueDate = l.IssueDate,
                                ExpiryDate = l.ExpirationDate,
                                IssuedBy = l.IssuedBy,
                                EngineCapacity = l.LoadCapacity,
                                EnginePower = l.Weight,
                                NumberOfSeats = l.SeatNumber,
                                VehicleMass = l.LoadCapacity,
                                MaximumPermissibleMass = l.Weight,

                                JMBG = l.User.JMBG,
                                FirstName = l.User.FirstName,
                                ParentName = l.User.ParentName,
                                LastName = l.User.LastName,
                                City = l.User.City,
                                Municipality = l.User.Municipality,
                                Street = l.User.Street,
                                HouseNumber = l.User.HouseNumber,
                                BirthDate = l.User.BirthDate,
                                BirthPlace = l.User.BirthPlace,
                                Gender = l.User.Gender,
                                Photo = l.User.Photo,
                                QRCode = l.GenerateQRCode()
                            }).FirstOrDefaultAsync();
                        return vehicleRegistration ?? null;

                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> AddDocumentAsync(int userId, T document)
        {
            if (document == null)
                return "Invalid data.";

            try
            {
                var user = await _context.RegularUsers.FindAsync(userId);
                if (user == null)
                    return "User not found.";

                var documentType = typeof(T).Name;
                document.Name = documentType;
                document.QRCode = $"{documentType},{user.JMBG}";

                switch (documentType)
                {
                    case "IDCard":
                        var idCard = document as IDCard;
                        idCard.UserId = userId;
                        user.IdCard = idCard;
                        await _context.IDCards.AddAsync(idCard);
                        break;
                    case "Passport":
                        var passport = document as Passport;
                        passport.UserId = userId;
                        user.Passport = passport;
                        await _context.Passports.AddAsync(passport);
                        break;
                    case "DrivingLicense":
                        var drivingLicense = document as DrivingLicense;
                        drivingLicense.UserId = userId;
                        user.DrivingLicense = drivingLicense;
                        await _context.DrivingLicenses.AddAsync(drivingLicense);
                        break;
                    case "WeaponPermit":
                        var weaponPermit = document as WeaponPermit;
                        weaponPermit.UserId = userId;
                        user.WeaponPermit = weaponPermit;
                        await _context.WeaponPermits.AddAsync(weaponPermit);
                        break;
                    case "VehicleRegistration":
                        var vehicleRegistration = document as VehicleRegistration;
                        vehicleRegistration.UserId = userId;
                        user.VehicleRegistration = vehicleRegistration;
                        await _context.VehicleRegistrations.AddAsync(vehicleRegistration);
                        break;
                }

                _context.RegularUsers.Update(user);
                await _context.SaveChangesAsync();
                return $"{documentType} successfully added.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> UpdateDocumentAsync(int userId, T document)
        {
            if (document == null)
                return "Invalid data.";

            try
            {
                var documentType = typeof(T).Name;
                object existingDocument = null;

                switch (documentType)
                {
                    case "IDCard":
                        existingDocument = await _context.IDCards.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (existingDocument != null)
                        {
                            var existingIdCard = existingDocument as IDCard;
                            var newIdCard = document as IDCard;
                            existingIdCard.IdCardNumber = newIdCard.IdCardNumber;
                            existingIdCard.IssueDate = newIdCard.IssueDate;
                            existingIdCard.ExpirationDate = newIdCard.ExpirationDate;
                            existingIdCard.IssuedBy = newIdCard.IssuedBy;
                            _context.IDCards.Update(existingIdCard);
                        }
                        break;
                    case "Passport":
                        existingDocument = await _context.Passports.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (existingDocument != null)
                        {
                            var existingPassport = existingDocument as Passport;
                            var newPassport = document as Passport;
                            existingPassport.PassportNumber = newPassport.PassportNumber;
                            existingPassport.IssueDate = newPassport.IssueDate;
                            existingPassport.ExpirationDate = newPassport.ExpirationDate;
                            existingPassport.IssuedBy = newPassport.IssuedBy;
                            _context.Passports.Update(existingPassport);
                        }
                        break;
                    case "DrivingLicense":
                        existingDocument = await _context.DrivingLicenses.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (existingDocument != null)
                        {
                            var existingDrivingLicense = existingDocument as DrivingLicense;
                            var newDrivingLicense = document as DrivingLicense;
                            existingDrivingLicense.DrivingLicenseNumber = newDrivingLicense.DrivingLicenseNumber;
                            existingDrivingLicense.VehicleCategories = newDrivingLicense.VehicleCategories;
                            existingDrivingLicense.IssueDate = newDrivingLicense.IssueDate;
                            existingDrivingLicense.ExpirationDate = newDrivingLicense.ExpirationDate;
                            existingDrivingLicense.IssuedBy = newDrivingLicense.IssuedBy;
                            _context.DrivingLicenses.Update(existingDrivingLicense);
                        }
                        break;
                    case "WeaponPermit":
                        existingDocument = await _context.WeaponPermits.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (existingDocument != null)
                        {
                            var existingWeaponPermit = existingDocument as WeaponPermit;
                            var newWeaponPermit = document as WeaponPermit;
                            existingWeaponPermit.WeaponPermitNumber = newWeaponPermit.WeaponPermitNumber;
                            existingWeaponPermit.WeaponTypes = newWeaponPermit.WeaponTypes;
                            existingWeaponPermit.WeaponQuantity = newWeaponPermit.WeaponQuantity;
                            existingWeaponPermit.WeaponTypeCount = newWeaponPermit.WeaponTypeCount;
                            existingWeaponPermit.WeaponCaliber = newWeaponPermit.WeaponCaliber;
                            existingWeaponPermit.UsageLocation = newWeaponPermit.UsageLocation;
                            existingWeaponPermit.UsagePurpose = newWeaponPermit.UsagePurpose;
                            existingWeaponPermit.IssueDate = newWeaponPermit.IssueDate;
                            existingWeaponPermit.ExpirationDate = newWeaponPermit.ExpirationDate;
                            existingWeaponPermit.IssuedBy = newWeaponPermit.IssuedBy;
                            _context.WeaponPermits.Update(existingWeaponPermit);
                        }
                        break;
                    case "VehicleRegistration":
                        existingDocument = await _context.VehicleRegistrations.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (existingDocument != null)
                        {
                            var existingVehicleRegistration = existingDocument as VehicleRegistration;
                            var newVehicleRegistration = document as VehicleRegistration;
                            existingVehicleRegistration.VehicleRegistrationNumber = newVehicleRegistration.VehicleRegistrationNumber;
                            existingVehicleRegistration.RegistrationNumber = newVehicleRegistration.RegistrationNumber;
                            existingVehicleRegistration.FirstRegistrationDate = newVehicleRegistration.FirstRegistrationDate;
                            existingVehicleRegistration.LoadCapacity = newVehicleRegistration.LoadCapacity;
                            existingVehicleRegistration.Weight = newVehicleRegistration.Weight;
                            existingVehicleRegistration.SeatNumber = newVehicleRegistration.SeatNumber;
                            existingVehicleRegistration.ProductionYear = newVehicleRegistration.ProductionYear;
                            existingVehicleRegistration.EngineNumber = newVehicleRegistration.EngineNumber;
                            existingVehicleRegistration.ChassisNumber = newVehicleRegistration.ChassisNumber;
                            existingVehicleRegistration.Brand = newVehicleRegistration.Brand;
                            existingVehicleRegistration.Type = newVehicleRegistration.Type;
                            existingVehicleRegistration.IssueDate = newVehicleRegistration.IssueDate;
                            existingVehicleRegistration.ExpirationDate = newVehicleRegistration.ExpirationDate;
                            existingVehicleRegistration.IssuedBy = newVehicleRegistration.IssuedBy;
                            _context.VehicleRegistrations.Update(existingVehicleRegistration);
                        }
                        break;
                }

                if (existingDocument == null)
                    return $"{documentType} not found for this user.";

                await _context.SaveChangesAsync();
                return $"{documentType} successfully updated.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> DeleteDocumentAsync(int userId)
        {
            try
            {
                var documentType = typeof(T).Name;
                object document = null;

                switch (documentType)
                {
                    case "IDCard":
                        document = await _context.IDCards.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (document != null)
                        {
                            _context.IDCards.Remove(document as IDCard);
                        }
                        break;
                    case "Passport":
                        document = await _context.Passports.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (document != null)
                        {
                            _context.Passports.Remove(document as Passport);
                        }
                        break;
                    case "DrivingLicense":
                        document = await _context.DrivingLicenses.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (document != null)
                        {
                            _context.DrivingLicenses.Remove(document as DrivingLicense);
                        }
                        break;
                    case "WeaponPermit":
                        document = await _context.WeaponPermits.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (document != null)
                        {
                            _context.WeaponPermits.Remove(document as WeaponPermit);
                        }
                        break;
                    case "VehicleRegistration":
                        document = await _context.VehicleRegistrations.FirstOrDefaultAsync(l => l.UserId == userId);
                        if (document != null)
                        {
                            _context.VehicleRegistrations.Remove(document as VehicleRegistration);
                        }
                        break;
                }

                if (document == null)
                    return $"{documentType} not found for this user.";

                await _context.SaveChangesAsync();
                return $"{documentType} successfully deleted.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<byte[]> GeneratePdfAsync(int userId, T document)
        {
            var documentData = await GetDocumentAsync(userId);
            var documentType = typeof(T).Name;

            switch (documentType)
            {
                case "IDCard":
                    return await _pdfService.GenerateIdCardPdfAsync(documentData);
                case "Passport":
                    return await _pdfService.GeneratePassportPdfAsync(documentData);
                case "DrivingLicense":
                    return await _pdfService.GenerateDrivingLicensePdfAsync(documentData);
                case "WeaponPermit":
                    return await _pdfService.GenerateWeaponPermitPdfAsync(documentData);
                case "VehicleRegistration":
                    return await _pdfService.GenerateVehicleRegistrationPdfAsync(documentData);
                default:
                    return null;
            }
        }

        public async Task<byte[]> GenerateQrCodeAsync(T document)
        {
            return await Task.Run(() => document.GenerateQRCode());
        }
    }
}