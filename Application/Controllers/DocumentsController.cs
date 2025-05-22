namespace Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        public DatabaseContext Context { get; set; }

        public DocumentsController(DatabaseContext context)
        {
            Context = context;
        }
    
    ///////////////////////////////////////////ID CARD//////////////////////////////////////////////////////////////////////////////////////
[HttpGet("ShowIdCard")]
public async Task<ActionResult> ShowIdCard(int userId)
{
    var idCard = await Context.IdCards
        .Where(l => l.UserId == userId)
        .Select(l => new
        {
            IdCardNumber = l.IdCardNumber,
            IssueDate = l.IssueDate,
            ExpiryDate = l.ExpiryDate,
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
    if (idCard == null)
        return BadRequest("ID card not found for this user.");
    try
    {
        return Ok(idCard);
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPut("EditIdCard")]
public async Task<ActionResult> EditIdCard(int userId, IdCard idCard)
{
    var existingIdCard = await Context.IdCards.FirstOrDefaultAsync(l => l.UserId == userId);

    if (idCard == null || existingIdCard == null)
        return BadRequest("Invalid data.");
    try
    {
        existingIdCard.IdCardNumber = idCard.IdCardNumber;
        existingIdCard.IssueDate = idCard.IssueDate;
        existingIdCard.ExpiryDate = idCard.ExpiryDate;
        existingIdCard.IssuedBy = idCard.IssuedBy;

        Context.IdCards.Update(existingIdCard);
        await Context.SaveChangesAsync();
        return Ok("ID card successfully updated.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("AddIdCard")]
public async Task<ActionResult> AddIdCard(int userId, [FromBody] IdCard idCard)
{
    if (idCard != null)
    {
        try
        {
            var user = await Context.RegularUsers.FindAsync(userId);

            idCard.Name = "IdCard";
            idCard.QRCode = $"{idCard.Name},{user.JMBG}";
            idCard.User = user;
            user.IdCard = idCard;
            await Context.IdCards.AddAsync(idCard);
            Context.RegularUsers.Update(user);
            await Context.SaveChangesAsync();
            return Ok("ID card successfully added.");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

[HttpDelete("DeleteIdCard")]
public async Task<ActionResult> DeleteIdCard(int userId)
{
    var user = await Context.RegularUsers.Include(l => l.IdCard).FirstOrDefaultAsync(l => l.ID == userId);
    if (userId == 0 || user == null)
        return BadRequest("Invalid data.");
    try
    {
        Context.IdCards.Remove(user.IdCard);
        user.IdCard = null;
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("ID card successfully deleted.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("IdCardPDF")]
public async Task<ActionResult> IdCardPDF(string jmbg)
{
    var user = await Context.RegularUsers
        .Where(l => l.JMBG == jmbg)
        .Include(l => l.IdCard)
        .Select(l => new
        {
            IdCardNumber = l.IdCard.IdCardNumber,
            IssueDate = l.IdCard.IssueDate,
            ExpiryDate = l.IdCard.ExpiryDate,
            IssuedBy = l.IdCard.IssuedBy,

            JMBG = l.JMBG,
            FirstName = l.FirstName,
            ParentName = l.ParentName,
            LastName = l.LastName,
            City = l.City,
            Municipality = l.Municipality,
            Street = l.Street,
            HouseNumber = l.HouseNumber,
            BirthDate = l.BirthDate,
            BirthPlace = l.BirthPlace,
            Gender = l.Gender,
            Photo = l.Photo,
            QRCode = l.IdCard.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var line = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());
                var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13);
                var space = new Paragraph(" ");
                space.Font.Size = 4;

                var image = iTextSharp.text.Image.GetInstance(user.Photo);
                image.Alignment = Element.ALIGN_LEFT;
                image.ScaleToFit(200, 200);

                PdfPTable table = new PdfPTable(2);

                PdfPCell cell1 = new PdfPCell(image);
                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell1);

                using (MemoryStream qrCodeStream = new MemoryStream(user.QRCode))
                {
                    var qrCodeImage = iTextSharp.text.Image.GetInstance(qrCodeStream);
                    qrCodeImage.ScaleToFit(100, 100);

                    PdfPCell cell2 = new PdfPCell(qrCodeImage);
                    cell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell2);
                }
                document.Add(line);
                document.Add(new Paragraph("EDOCUMENTS: ID CARD DATA PRINT", font));
                document.Add(line);
                document.Add(space);

                document.Add(table);

                document.Add(line);
                document.Add(new Paragraph("Citizen data", font));
                document.Add(line);

                void AddData(string label, string value)
                {
                    var table = new PdfPTable(2);
                    table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    var labelCell = new PdfPCell(new Phrase(label, font));
                    labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(labelCell);

                    var valueCell = new PdfPCell(new Phrase(value, font));
                    valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(valueCell);

                    document.Add(table);
                    document.Add(space);
                }

                AddData("Last name:", user.LastName.ToUpper());
                AddData("First name:", user.FirstName.ToUpper());
                AddData("Parent's name:", user.ParentName.ToUpper());
                AddData("Date of birth:", user.BirthDate.ToShortDateString());
                AddData("Place of birth:", $"{user.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                AddData("Residence and address:", $"{user.City.ToUpper()}, {user.Municipality.ToUpper()}, {user.Street.ToUpper()} {user.HouseNumber.ToUpper()}");
                AddData("JMBG:", user.JMBG);
                AddData("Gender:", user.Gender.ToString());
                document.Add(line);
                document.Add(new Paragraph("Document data", font));
                document.Add(line);
                AddData("Issued by:", user.IssuedBy.ToUpper());
                AddData("Document number:", user.IdCardNumber);
                AddData("Date of issue:", user.IssueDate.ToShortDateString());
                AddData("Valid until:", user.ExpiryDate.ToShortDateString());
                document.Add(line);
                document.Add(space);
                document.Add(line);

                document.Close();
                writer.Close();
                var data = ms.ToArray();
                return File(data, "application/pdf", "ID_card.pdf", true);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

[HttpPost("IdCardQR")]
public async Task<ActionResult> IdCardQR(int userId)
{
    var user = await Context.RegularUsers
        .Where(l => l.ID == userId)
        .Include(l => l.IdCard)
        .Select(l => new
        {
            QRCode = l.IdCard.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            return new FileContentResult(user.QRCode, "image/png");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

///////////////////////////////////////////PASSPORT//////////////////////////////////////////////////////////////////////////////////////
[HttpGet("ShowPassport")]
public async Task<ActionResult> ShowPassport(int userId)
{
    var passport = await Context.Passports
        .Where(l => l.UserId == userId)
        .Select(l => new
        {
            PassportNumber = l.PassportNumber,
            IssueDate = l.IssueDate,
            ExpiryDate = l.ExpiryDate,
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
    if (passport == null)
        return BadRequest("Passport not found for this user.");
    try
    {
        return Ok(passport);
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPut("EditPassport")]
public async Task<ActionResult> EditPassport(int userId, Passport passport)
{
    var existingPassport = await Context.Passports.FirstOrDefaultAsync(l => l.UserId == userId);
    if (passport == null || existingPassport == null)
        return BadRequest("Invalid data.");
    try
    {
        existingPassport.PassportNumber = passport.PassportNumber;
        existingPassport.IssueDate = passport.IssueDate;
        existingPassport.ExpiryDate = passport.ExpiryDate;
        existingPassport.IssuedBy = passport.IssuedBy;

        Context.Passports.Update(existingPassport);
        await Context.SaveChangesAsync();
        return Ok("Passport successfully updated.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("AddPassport")]
public async Task<ActionResult> AddPassport(int userId, [FromBody] Passport passport)
{
    if (passport == null)
        return BadRequest("Invalid data.");
    try
    {
        var user = await Context.RegularUsers.FindAsync(userId);
        passport.Name = "Passport";
        passport.QRCode = $"{passport.Name},{user.JMBG}";
        passport.User = user;
        user.Passport = passport;
        await Context.Passports.AddAsync(passport);
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Passport successfully added.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpDelete("DeletePassport")]
public async Task<ActionResult> DeletePassport(int userId)
{
    var user = await Context.RegularUsers.Include(l => l.Passport).FirstOrDefaultAsync(l => l.ID == userId);
    if (userId == 0 || user == null)
        return BadRequest("Invalid data.");
    try
    {
        Context.Passports.Remove(user.Passport);
        user.Passport = null;
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Passport successfully deleted.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("PassportPDF")]
public async Task<ActionResult> PassportPDF(string jmbg)
{
    var user = await Context.RegularUsers
        .Where(l => l.JMBG == jmbg)
        .Include(l => l.Passport)
        .Select(l => new
        {
            PassportNumber = l.Passport.PassportNumber,
            IssueDate = l.Passport.IssueDate,
            ExpiryDate = l.Passport.ExpiryDate,
            IssuedBy = l.Passport.IssuedBy,

            JMBG = l.JMBG,
            FirstName = l.FirstName,
            ParentName = l.ParentName,
            LastName = l.LastName,
            City = l.City,
            Municipality = l.Municipality,
            Street = l.Street,
            HouseNumber = l.HouseNumber,
            BirthDate = l.BirthDate,
            BirthPlace = l.BirthPlace,
            Gender = l.Gender,
            Photo = l.Photo,
            QRCode = l.Passport.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var line = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());
                var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13);
                var space = new Paragraph(" ");
                space.Font.Size = 4;

                var image = iTextSharp.text.Image.GetInstance(user.Photo);
                image.Alignment = Element.ALIGN_LEFT;
                image.ScaleToFit(200, 200);

                PdfPTable table = new PdfPTable(2);

                PdfPCell cell1 = new PdfPCell(image);
                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell1);

                using (MemoryStream qrCodeStream = new MemoryStream(user.QRCode))
                {
                    var qrCodeImage = iTextSharp.text.Image.GetInstance(qrCodeStream);
                    qrCodeImage.ScaleToFit(100, 100);

                    PdfPCell cell2 = new PdfPCell(qrCodeImage);
                    cell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell2);
                }
                document.Add(line);
                document.Add(new Paragraph("EDOCUMENTS: PASSPORT DATA PRINT", font));
                document.Add(line);
                document.Add(space);

                document.Add(table);

                document.Add(line);
                document.Add(new Paragraph("Citizen data", font));
                document.Add(line);

                void AddData(string label, string value)
                {
                    var table = new PdfPTable(2);
                    table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    var labelCell = new PdfPCell(new Phrase(label, font));
                    labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(labelCell);

                    var valueCell = new PdfPCell(new Phrase(value, font));
                    valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(valueCell);

                    document.Add(table);
                    document.Add(space);
                }

                AddData("Last name:", user.LastName.ToUpper());
                AddData("First name:", user.FirstName.ToUpper());
                AddData("Parent's name:", user.ParentName.ToUpper());
                AddData("Date of birth:", user.BirthDate.ToShortDateString());
                AddData("Place of birth:", $"{user.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                AddData("Residence and address:", $"{user.City.ToUpper()}, {user.Municipality.ToUpper()}, {user.Street.ToUpper()} {user.HouseNumber.ToUpper()}");
                AddData("JMBG:", user.JMBG);
                AddData("Gender:", user.Gender.ToString());
                document.Add(line);
                document.Add(new Paragraph("Document data", font));
                document.Add(line);
                AddData("Issued by:", user.IssuedBy.ToUpper());
                AddData("Document number:", user.PassportNumber);
                AddData("Date of issue:", user.IssueDate.ToShortDateString());
                AddData("Valid until:", user.ExpiryDate.ToShortDateString());
                document.Add(line);
                document.Add(space);
                document.Add(line);

                document.Close();
                writer.Close();
                var data = ms.ToArray();
                return File(data, "application/pdf", "Passport.pdf", true);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

[HttpPost("PassportQR")]
public async Task<ActionResult> PassportQR(int userId)
{
    var user = await Context.RegularUsers
        .Where(l => l.ID == userId)
        .Include(l => l.Passport)
        .Select(l => new
        {
            QRCode = l.Passport.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            return new FileContentResult(user.QRCode, "image/png");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}
    ///////////////////////////////////////////DRIVER'S LICENSE//////////////////////////////////////////////////////////////////////////////////////
[HttpGet("ShowDriversLicense")]
public async Task<ActionResult> ShowDriversLicense(int userId)
{
    var driversLicense = await Context.DriversLicenses
        .Where(l => l.UserId == userId)
        .Select(l => new
        {
            DriversLicenseNumber = l.DriversLicenseNumber,
            VehicleCategories = l.VehicleCategories,
            IssueDate = l.IssueDate,
            ExpiryDate = l.ExpiryDate,
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
    if (driversLicense == null)
        return BadRequest("Driver's license not found for this user.");
    try
    {
        return Ok(driversLicense);
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPut("EditDriversLicense")]
public async Task<ActionResult> EditDriversLicense(int userId, DriversLicense license)
{
    var driversLicense = await Context.DriversLicenses.FirstOrDefaultAsync(l => l.UserId == userId);
    if (license == null || driversLicense == null)
        return BadRequest("Invalid data.");
    try
    {
        driversLicense.DriversLicenseNumber = license.DriversLicenseNumber;
        driversLicense.VehicleCategories = license.VehicleCategories;
        driversLicense.IssueDate = license.IssueDate;
        driversLicense.ExpiryDate = license.ExpiryDate;
        driversLicense.IssuedBy = license.IssuedBy;

        Context.DriversLicenses.Update(driversLicense);
        await Context.SaveChangesAsync();
        return Ok("Driver's license successfully updated.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("AddDriversLicense")]
public async Task<ActionResult> AddDriversLicense(int userId, [FromBody] DriversLicense license)
{
    if (license == null)
        return BadRequest("Invalid data.");
    try
    {
        var user = await Context.RegularUsers.FindAsync(userId);

        license.Name = "DriversLicense";
        license.QRCode = $"{license.Name},{user.JMBG}";
        license.User = user;
        user.DriversLicense = license;
        await Context.DriversLicenses.AddAsync(license);
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Driver's license successfully added.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpDelete("DeleteDriversLicense")]
public async Task<ActionResult> DeleteDriversLicense(int userId)
{
    var user = await Context.RegularUsers.Include(l => l.DriversLicense).FirstOrDefaultAsync(l => l.ID == userId);
    if (userId == 0 || user == null)
        return BadRequest("Invalid data.");
    try
    {
        Context.DriversLicenses.Remove(user.DriversLicense);
        user.DriversLicense = null;
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Driver's license successfully deleted.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("DriversLicensePDF")]
public async Task<ActionResult> DriversLicensePDF(string jmbg)
{
    var user = await Context.RegularUsers
        .Where(l => l.JMBG == jmbg)
        .Include(l => l.DriversLicense)
        .Select(l => new
        {
            DriversLicenseNumber = l.DriversLicense.DriversLicenseNumber,
            VehicleCategories = l.DriversLicense.VehicleCategories,
            IssueDate = l.DriversLicense.IssueDate,
            ExpiryDate = l.DriversLicense.ExpiryDate,
            IssuedBy = l.DriversLicense.IssuedBy,

            JMBG = l.JMBG,
            FirstName = l.FirstName,
            ParentName = l.ParentName,
            LastName = l.LastName,
            City = l.City,
            Municipality = l.Municipality,
            Street = l.Street,
            HouseNumber = l.HouseNumber,
            BirthDate = l.BirthDate,
            BirthPlace = l.BirthPlace,
            Gender = l.Gender,
            Photo = l.Photo,
            QRCode = l.DriversLicense.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var line = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());
                var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13);
                var space = new Paragraph(" ");
                space.Font.Size = 4;

                var image = iTextSharp.text.Image.GetInstance(user.Photo);
                image.Alignment = Element.ALIGN_LEFT;
                image.ScaleToFit(200, 200);

                PdfPTable table = new PdfPTable(2);

                PdfPCell cell1 = new PdfPCell(image);
                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell1);

                using (MemoryStream qrCodeStream = new MemoryStream(user.QRCode))
                {
                    var qrCodeImage = iTextSharp.text.Image.GetInstance(qrCodeStream);
                    qrCodeImage.ScaleToFit(100, 100);

                    PdfPCell cell2 = new PdfPCell(qrCodeImage);
                    cell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell2);
                }
                document.Add(line);
                document.Add(new Paragraph("EDOCUMENTS: DRIVER'S LICENSE DATA PRINT", font));
                document.Add(line);
                document.Add(space);

                document.Add(table);

                document.Add(line);
                document.Add(new Paragraph("Citizen data", font));
                document.Add(line);

                void AddData(string label, string value)
                {
                    var table = new PdfPTable(2);
                    table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    var labelCell = new PdfPCell(new Phrase(label, font));
                    labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(labelCell);

                    var valueCell = new PdfPCell(new Phrase(value, font));
                    valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(valueCell);

                    document.Add(table);
                    document.Add(space);
                }

                AddData("Last name:", user.LastName.ToUpper());
                AddData("First name:", user.FirstName.ToUpper());
                AddData("Parent's name:", user.ParentName.ToUpper());
                AddData("Date of birth:", user.BirthDate.ToShortDateString());
                AddData("Place of birth:", $"{user.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                AddData("Residence and address:", $"{user.City.ToUpper()}, {user.Municipality.ToUpper()}, {user.Street.ToUpper()} {user.HouseNumber.ToUpper()}");
                AddData("JMBG:", user.JMBG);
                AddData("Gender:", user.Gender.ToString());
                document.Add(line);
                document.Add(new Paragraph("Document data", font));
                document.Add(line);
                AddData("Issued by:", user.IssuedBy.ToUpper());
                AddData("Document number:", user.DriversLicenseNumber);
                AddData("Vehicle categories:", user.VehicleCategories.ToUpper());
                AddData("Date of issue:", user.IssueDate.ToShortDateString());
                AddData("Valid until:", user.ExpiryDate.ToShortDateString());
                document.Add(line);
                document.Add(space);
                document.Add(line);

                document.Close();
                writer.Close();
                var data = ms.ToArray();
                return File(data, "application/pdf", "Drivers_license.pdf");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

[HttpPost("DriversLicenseQR")]
public async Task<ActionResult> DriversLicenseQR(int userId)
{
    var user = await Context.RegularUsers
        .Where(l => l.ID == userId)
        .Include(l => l.DriversLicense)
        .Select(l => new
        {
            QRCode = l.DriversLicense.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            return new FileContentResult(user.QRCode, "image/png");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

///////////////////////////////////////////WEAPON PERMIT//////////////////////////////////////////////////////////////////////////////////////
[HttpGet("ShowWeaponPermit")]
public async Task<ActionResult> ShowWeaponPermit(int userId)
{
    var permit = await Context.WeaponPermits
        .Where(l => l.UserId == userId)
        .Select(l => new
        {
            WeaponPermitNumber = l.WeaponPermitNumber,
            WeaponTypes = l.WeaponTypes,
            WeaponQuantity = l.WeaponQuantity,
            WeaponCountByType = l.WeaponCountByType,
            WeaponCaliber = l.WeaponCaliber,
            PlaceOfUse = l.PlaceOfUse,
            PurposeOfUse = l.PurposeOfUse,
            IssueDate = l.IssueDate,
            ExpiryDate = l.ExpiryDate,
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
    if (permit == null)
        return BadRequest("Weapon permit not found for this user.");
    try
    {
        return Ok(permit);
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPut("EditWeaponPermit")]
public async Task<ActionResult> EditWeaponPermit(int userId, WeaponPermit permit)
{
    var existingPermit = await Context.WeaponPermits.FirstOrDefaultAsync(l => l.UserId == userId);
    if (permit == null || existingPermit == null)
        return BadRequest("Invalid data.");
    try
    {
        existingPermit.WeaponPermitNumber = permit.WeaponPermitNumber;
        existingPermit.WeaponTypes = permit.WeaponTypes;
        existingPermit.WeaponQuantity = permit.WeaponQuantity;
        existingPermit.WeaponCountByType = permit.WeaponCountByType;
        existingPermit.WeaponCaliber = permit.WeaponCaliber;
        existingPermit.PlaceOfUse = permit.PlaceOfUse;
        existingPermit.PurposeOfUse = permit.PurposeOfUse;
        existingPermit.IssueDate = permit.IssueDate;
        existingPermit.ExpiryDate = permit.ExpiryDate;
        existingPermit.IssuedBy = permit.IssuedBy;

        Context.WeaponPermits.Update(existingPermit);
        await Context.SaveChangesAsync();
        return Ok("Weapon permit successfully updated.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("AddWeaponPermit")]
public async Task<ActionResult> AddWeaponPermit(int userId, [FromBody] WeaponPermit permit)
{
    if (permit == null)
        return BadRequest("Invalid data.");
    try
    {
        var user = await Context.RegularUsers.FindAsync(userId);
        permit.Name = "WeaponPermit";
        permit.QRCode = $"{permit.Name},{user.JMBG}";
        permit.User = user;
        user.WeaponPermit = permit;
        await Context.WeaponPermits.AddAsync(permit);
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Weapon permit successfully added.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpDelete("DeleteWeaponPermit")]
public async Task<ActionResult> DeleteWeaponPermit(int userId)
{
    var user = await Context.RegularUsers.Include(l => l.WeaponPermit).FirstOrDefaultAsync(l => l.ID == userId);
    if (userId == 0 || user == null)
        return BadRequest("Invalid data.");
    try
    {
        Context.WeaponPermits.Remove(user.WeaponPermit);
        user.WeaponPermit = null;
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Weapon permit successfully deleted.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("WeaponPermitPDF")]
public async Task<ActionResult> WeaponPermitPDF(string jmbg)
{
    var user = await Context.RegularUsers
        .Where(l => l.JMBG == jmbg)
        .Include(l => l.WeaponPermit)
        .Select(l => new
        {
            WeaponPermitNumber = l.WeaponPermit.WeaponPermitNumber,
            WeaponTypes = l.WeaponPermit.WeaponTypes,
            WeaponQuantity = l.WeaponPermit.WeaponQuantity,
            WeaponCountByType = l.WeaponPermit.WeaponCountByType,
            WeaponCaliber = l.WeaponPermit.WeaponCaliber,
            PlaceOfUse = l.WeaponPermit.PlaceOfUse,
            PurposeOfUse = l.WeaponPermit.PurposeOfUse,
            IssueDate = l.WeaponPermit.IssueDate,
            ExpiryDate = l.WeaponPermit.ExpiryDate,
            IssuedBy = l.WeaponPermit.IssuedBy,

            JMBG = l.JMBG,
            FirstName = l.FirstName,
            ParentName = l.ParentName,
            LastName = l.LastName,
            City = l.City,
            Municipality = l.Municipality,
            Street = l.Street,
            HouseNumber = l.HouseNumber,
            BirthDate = l.BirthDate,
            BirthPlace = l.BirthPlace,
            Gender = l.Gender,
            Photo = l.Photo,
            QRCode = l.WeaponPermit.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var line = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());
                var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13);
                var space = new Paragraph(" ");
                space.Font.Size = 4;

                var image = iTextSharp.text.Image.GetInstance(user.Photo);
                image.Alignment = Element.ALIGN_LEFT;
                image.ScaleToFit(200, 200);

                PdfPTable table = new PdfPTable(2);

                PdfPCell cell1 = new PdfPCell(image);
                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell1);

                using (MemoryStream qrCodeStream = new MemoryStream(user.QRCode))
                {
                    var qrCodeImage = iTextSharp.text.Image.GetInstance(qrCodeStream);
                    qrCodeImage.ScaleToFit(100, 100);

                    PdfPCell cell2 = new PdfPCell(qrCodeImage);
                    cell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell2);
                }
                document.Add(line);
                document.Add(new Paragraph("EDOCUMENTS: WEAPON PERMIT DATA PRINT", font));
                document.Add(line);
                document.Add(space);

                document.Add(table);

                document.Add(line);
                document.Add(new Paragraph("Citizen data", font));
                document.Add(line);

                void AddData(string label, string value)
                {
                    var table = new PdfPTable(2);
                    table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    var labelCell = new PdfPCell(new Phrase(label, font));
                    labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(labelCell);

                    var valueCell = new PdfPCell(new Phrase(value, font));
                    valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(valueCell);

                    document.Add(table);
                    document.Add(space);
                }

                AddData("Last name:", user.LastName.ToUpper());
                AddData("First name:", user.FirstName.ToUpper());
                AddData("Parent's name:", user.ParentName.ToUpper());
                AddData("Date of birth:", user.BirthDate.ToShortDateString());
                AddData("Place of birth:", $"{user.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                AddData("Residence and address:", $"{user.City.ToUpper()}, {user.Municipality.ToUpper()}, {user.Street.ToUpper()} {user.HouseNumber.ToUpper()}");
                AddData("JMBG:", user.JMBG);
                AddData("Gender:", user.Gender.ToString());
                document.Add(line);
                document.Add(new Paragraph("Document data", font));
                document.Add(line);
                AddData("Issued by:", user.IssuedBy.ToUpper());
                AddData("Document number:", user.WeaponPermitNumber);
                AddData("Weapon types:", user.WeaponTypes.ToUpper());
                AddData("Weapon quantity:", user.WeaponQuantity.ToString());
                AddData("Weapon count by type:", user.WeaponCountByType.ToUpper());
                AddData("Weapon caliber:", user.WeaponCaliber.ToUpper());
                AddData("Place of use:", user.PlaceOfUse.ToUpper());
                AddData("Purpose of use:", user.PurposeOfUse.ToUpper());
                AddData("Date of issue:", user.IssueDate.ToShortDateString());
                AddData("Valid until:", user.ExpiryDate.ToShortDateString());
                document.Add(line);
                document.Add(space);
                document.Add(line);

                document.Close();
                writer.Close();
                var data = ms.ToArray();
                return File(data, "application/pdf", "Weapon_permit.pdf");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

[HttpPost("WeaponPermitQR")]
public async Task<ActionResult> WeaponPermitQR(int userId)
{
    var user = await Context.RegularUsers
        .Where(l => l.ID == userId)
        .Include(l => l.WeaponPermit)
        .Select(l => new
        {
            QRCode = l.WeaponPermit.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            return new FileContentResult(user.QRCode, "image/png");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}
    ///////////////////////////////////////////VEHICLE REGISTRATION//////////////////////////////////////////////////////////////////////////////////////
[HttpGet("ShowVehicleRegistration")]
public async Task<ActionResult> ShowVehicleRegistration(int userId)
{
    var registration = await Context.VehicleRegistrations
        .Where(l => l.UserId == userId)
        .Select(l => new
        {
            RegistrationNumber = l.RegistrationNumber,
            PlateNumber = l.PlateNumber,
            FirstRegistrationDate = l.FirstRegistrationDate,
            LoadCapacity = l.LoadCapacity,
            Weight = l.Weight,
            SeatCount = l.SeatCount,
            YearOfProduction = l.YearOfProduction,
            EngineNumber = l.EngineNumber,
            ChassisNumber = l.ChassisNumber,
            Brand = l.Brand,
            Type = l.Type,
            IssueDate = l.IssueDate,
            ExpiryDate = l.ExpiryDate,
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
    if (registration == null)
        return BadRequest("Vehicle registration not found for this user.");
    try
    {
        return Ok(registration);
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPut("EditVehicleRegistration")]
public async Task<ActionResult> EditVehicleRegistration(int userId, VehicleRegistration registration)
{
    var existingRegistration = await Context.VehicleRegistrations.FirstOrDefaultAsync(l => l.UserId == userId);
    if (registration == null || existingRegistration == null)
        return BadRequest("Invalid data.");
    try
    {
        existingRegistration.RegistrationNumber = registration.RegistrationNumber;
        existingRegistration.PlateNumber = registration.PlateNumber;
        existingRegistration.FirstRegistrationDate = registration.FirstRegistrationDate;
        existingRegistration.LoadCapacity = registration.LoadCapacity;
        existingRegistration.Weight = registration.Weight;
        existingRegistration.SeatCount = registration.SeatCount;
        existingRegistration.YearOfProduction = registration.YearOfProduction;
        existingRegistration.EngineNumber = registration.EngineNumber;
        existingRegistration.ChassisNumber = registration.ChassisNumber;
        existingRegistration.Brand = registration.Brand;
        existingRegistration.Type = registration.Type;
        existingRegistration.IssueDate = registration.IssueDate;
        existingRegistration.ExpiryDate = registration.ExpiryDate;
        existingRegistration.IssuedBy = registration.IssuedBy;

        Context.VehicleRegistrations.Update(existingRegistration);
        await Context.SaveChangesAsync();
        return Ok("Vehicle registration successfully updated.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("AddVehicleRegistration")]
public async Task<ActionResult> AddVehicleRegistration(int userId, [FromBody] VehicleRegistration registration)
{
    if (registration == null)
        return BadRequest("Invalid data.");
    try
    {
        var user = await Context.RegularUsers.FindAsync(userId);
        registration.Name = "VehicleRegistration";
        registration.QRCode = $"{registration.Name},{user.JMBG}";
        registration.User = user;
        user.VehicleRegistration = registration;
        await Context.VehicleRegistrations.AddAsync(registration);
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Vehicle registration successfully added.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpDelete("DeleteVehicleRegistration")]
public async Task<ActionResult> DeleteVehicleRegistration(int userId)
{
    var user = await Context.RegularUsers.Include(l => l.VehicleRegistration).FirstOrDefaultAsync(l => l.ID == userId);
    if (userId == 0 || user == null)
        return BadRequest("Invalid data.");
    try
    {
        Context.VehicleRegistrations.Remove(user.VehicleRegistration);
        user.VehicleRegistration = null;
        Context.RegularUsers.Update(user);
        await Context.SaveChangesAsync();
        return Ok("Vehicle registration successfully deleted.");
    }
    catch (Exception e)
    {
        return BadRequest(e.Message);
    }
}

[HttpPost("VehicleRegistrationPDF")]
public async Task<ActionResult> VehicleRegistrationPDF(string jmbg)
{
    var user = await Context.RegularUsers
        .Where(l => l.JMBG == jmbg)
        .Include(l => l.VehicleRegistration)
        .Select(l => new
        {
            RegistrationNumber = l.VehicleRegistration.RegistrationNumber,
            PlateNumber = l.VehicleRegistration.PlateNumber,
            FirstRegistrationDate = l.VehicleRegistration.FirstRegistrationDate,
            LoadCapacity = l.VehicleRegistration.LoadCapacity,
            Weight = l.VehicleRegistration.Weight,
            SeatCount = l.VehicleRegistration.SeatCount,
            YearOfProduction = l.VehicleRegistration.YearOfProduction,
            EngineNumber = l.VehicleRegistration.EngineNumber,
            ChassisNumber = l.VehicleRegistration.ChassisNumber,
            Brand = l.VehicleRegistration.Brand,
            Type = l.VehicleRegistration.Type,
            IssueDate = l.VehicleRegistration.IssueDate,
            ExpiryDate = l.VehicleRegistration.ExpiryDate,
            IssuedBy = l.VehicleRegistration.IssuedBy,

            JMBG = l.JMBG,
            FirstName = l.FirstName,
            ParentName = l.ParentName,
            LastName = l.LastName,
            City = l.City,
            Municipality = l.Municipality,
            Street = l.Street,
            HouseNumber = l.HouseNumber,
            BirthDate = l.BirthDate,
            BirthPlace = l.BirthPlace,
            Gender = l.Gender,
            Photo = l.Photo,
            QRCode = l.VehicleRegistration.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var line = new Chunk(new iTextSharp.text.pdf.draw.LineSeparator());
                var font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13);
                var space = new Paragraph(" ");
                space.Font.Size = 4;

                var image = iTextSharp.text.Image.GetInstance(user.Photo);
                image.Alignment = Element.ALIGN_LEFT;
                image.ScaleToFit(200, 200);

                PdfPTable table = new PdfPTable(2);

                PdfPCell cell1 = new PdfPCell(image);
                cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell(cell1);

                using (MemoryStream qrCodeStream = new MemoryStream(user.QRCode))
                {
                    var qrCodeImage = iTextSharp.text.Image.GetInstance(qrCodeStream);
                    qrCodeImage.ScaleToFit(100, 100);

                    PdfPCell cell2 = new PdfPCell(qrCodeImage);
                    cell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell2);
                }
                document.Add(line);
                document.Add(new Paragraph("EDOCUMENTS: VEHICLE REGISTRATION DATA PRINT", font));
                document.Add(line);
                document.Add(space);

                document.Add(table);

                document.Add(line);
                document.Add(new Paragraph("Citizen data", font));
                document.Add(line);

                void AddData(string label, string value)
                {
                    var table = new PdfPTable(2);
                    table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    var labelCell = new PdfPCell(new Phrase(label, font));
                    labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(labelCell);

                    var valueCell = new PdfPCell(new Phrase(value, font));
                    valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    table.AddCell(valueCell);

                    document.Add(table);
                    document.Add(space);
                }

                AddData("Last name:", user.LastName.ToUpper());
                AddData("First name:", user.FirstName.ToUpper());
                AddData("Parent's name:", user.ParentName.ToUpper());
                AddData("Date of birth:", user.BirthDate.ToShortDateString());
                AddData("Place of birth:", $"{user.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                AddData("Residence and address:", $"{user.City.ToUpper()}, {user.Municipality.ToUpper()}, {user.Street.ToUpper()} {user.HouseNumber.ToUpper()}");
                AddData("JMBG:", user.JMBG);
                AddData("Gender:", user.Gender.ToString());
                document.Add(line);
                document.Add(new Paragraph("Document data", font));
                document.Add(line);
                AddData("Issued by:", user.IssuedBy.ToUpper());
                AddData("Document number:", user.RegistrationNumber);
                AddData("Plate number:", user.PlateNumber.ToUpper());
                AddData("First registration date:", user.FirstRegistrationDate.ToShortDateString());
                AddData("Load capacity:", user.LoadCapacity.ToUpper());
                AddData("Weight:", user.Weight.ToUpper());
                AddData("Seat count:", user.SeatCount.ToString());
                AddData("Year of production:", user.YearOfProduction.ToString());
                AddData("Engine number:", user.EngineNumber.ToUpper());
                AddData("Chassis number:", user.ChassisNumber.ToUpper());
                AddData("Brand:", user.Brand.ToUpper());
                AddData("Type:", user.Type.ToUpper());
                AddData("Date of issue:", user.IssueDate.ToShortDateString());
                AddData("Valid until:", user.ExpiryDate.ToShortDateString());
                document.Add(line);
                document.Add(space);
                document.Add(line);

                document.Close();
                writer.Close();
                var data = ms.ToArray();
                return File(data, "application/pdf", "Vehicle_registration.pdf");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}

[HttpPost("VehicleRegistrationQR")]
public async Task<ActionResult> VehicleRegistrationQR(int userId)
{
    var user = await Context.RegularUsers
        .Where(l => l.ID == userId)
        .Include(l => l.VehicleRegistration)
        .Select(l => new
        {
            QRCode = l.VehicleRegistration.GenerateQRCode()
        }).FirstOrDefaultAsync();
    if (user != null)
    {
        try
        {
            return new FileContentResult(user.QRCode, "image/png");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    return BadRequest("Invalid data.");
}
}