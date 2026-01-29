using iTextSharp.text;
using iTextSharp.text.pdf;
using Models;
using DocModel = Models.Document;

namespace Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        public async Task<byte[]> GenerateIdCardPdfAsync(dynamic documentData)
        {
            return await Task.Run(() =>
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

                    var image = Image.GetInstance(documentData.Photo);
                    image.Alignment = Element.ALIGN_LEFT;
                    image.ScaleToFit(200, 200);

                    PdfPTable table = new PdfPTable(2);

                    PdfPCell cell1 = new PdfPCell(image);
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell1);

                    using (MemoryStream qrCodeStream = new MemoryStream(documentData.QRCode))
                    {
                        var qrCodeImage = Image.GetInstance(qrCodeStream);
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
                        var dataTable = new PdfPTable(2);
                        dataTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        var labelCell = new PdfPCell(new Phrase(label, font));
                        labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(labelCell);

                        var valueCell = new PdfPCell(new Phrase(value, font));
                        valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(valueCell);

                        document.Add(dataTable);
                        document.Add(space);
                    }

                    AddData("Last name:", documentData.LastName.ToUpper());
                    AddData("First name:", documentData.FirstName.ToUpper());
                    AddData("Parent's name:", documentData.ParentName.ToUpper());
                    AddData("Date of birth:", documentData.BirthDate.ToShortDateString());
                    AddData("Place of birth:", $"{documentData.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                    AddData("Residence and address:", $"{documentData.City.ToUpper()}, {documentData.Municipality.ToUpper()}, {documentData.Street.ToUpper()} {documentData.HouseNumber.ToUpper()}");
                    AddData("JMBG:", documentData.JMBG);
                    AddData("Gender:", documentData.Gender.ToString());

                    document.Add(line);
                    document.Add(new Paragraph("Document data", font));
                    document.Add(line);
                    AddData("Issued by:", documentData.IssuedBy.ToUpper());
                    AddData("Document number:", documentData.IdCardNumber);
                    AddData("Date of issue:", documentData.IssueDate.ToShortDateString());
                    AddData("Valid until:", documentData.ExpiryDate.ToShortDateString());

                    document.Add(line);
                    document.Add(space);
                    document.Add(line);

                    document.Close();
                    writer.Close();
                    return ms.ToArray();
                }
            });
        }

        public async Task<byte[]> GeneratePassportPdfAsync(dynamic documentData)
        {
            return await Task.Run(() =>
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

                    var image = Image.GetInstance(documentData.Photo);
                    image.Alignment = Element.ALIGN_LEFT;
                    image.ScaleToFit(200, 200);

                    PdfPTable table = new PdfPTable(2);

                    PdfPCell cell1 = new PdfPCell(image);
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell1);

                    using (MemoryStream qrCodeStream = new MemoryStream(documentData.QRCode))
                    {
                        var qrCodeImage = Image.GetInstance(qrCodeStream);
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
                        var dataTable = new PdfPTable(2);
                        dataTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        var labelCell = new PdfPCell(new Phrase(label, font));
                        labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(labelCell);

                        var valueCell = new PdfPCell(new Phrase(value, font));
                        valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(valueCell);

                        document.Add(dataTable);
                        document.Add(space);
                    }

                    AddData("Last name:", documentData.LastName.ToUpper());
                    AddData("First name:", documentData.FirstName.ToUpper());
                    AddData("Parent's name:", documentData.ParentName.ToUpper());
                    AddData("Date of birth:", documentData.BirthDate.ToShortDateString());
                    AddData("Place of birth:", $"{documentData.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                    AddData("Residence and address:", $"{documentData.City.ToUpper()}, {documentData.Municipality.ToUpper()}, {documentData.Street.ToUpper()} {documentData.HouseNumber.ToUpper()}");
                    AddData("JMBG:", documentData.JMBG);
                    AddData("Gender:", documentData.Gender.ToString());

                    document.Add(line);
                    document.Add(new Paragraph("Document data", font));
                    document.Add(line);
                    AddData("Issued by:", documentData.IssuedBy.ToUpper());
                    AddData("Document number:", documentData.PassportNumber);
                    AddData("Date of issue:", documentData.IssueDate.ToShortDateString());
                    AddData("Valid until:", documentData.ExpiryDate.ToShortDateString());

                    document.Add(line);
                    document.Add(space);
                    document.Add(line);

                    document.Close();
                    writer.Close();
                    return ms.ToArray();
                }
            });
        }

        public async Task<byte[]> GenerateDrivingLicensePdfAsync(dynamic documentData)
        {
            return await Task.Run(() =>
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

                    var image = Image.GetInstance(documentData.Photo);
                    image.Alignment = Element.ALIGN_LEFT;
                    image.ScaleToFit(200, 200);

                    PdfPTable table = new PdfPTable(2);

                    PdfPCell cell1 = new PdfPCell(image);
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell1);

                    using (MemoryStream qrCodeStream = new MemoryStream(documentData.QRCode))
                    {
                        var qrCodeImage = Image.GetInstance(qrCodeStream);
                        qrCodeImage.ScaleToFit(100, 100);

                        PdfPCell cell2 = new PdfPCell(qrCodeImage);
                        cell2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell2.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell2);
                    }

                    document.Add(line);
                    document.Add(new Paragraph("EDOCUMENTS: DRIVING LICENSE DATA PRINT", font));
                    document.Add(line);
                    document.Add(space);
                    document.Add(table);
                    document.Add(line);
                    document.Add(new Paragraph("Citizen data", font));
                    document.Add(line);

                    void AddData(string label, string value)
                    {
                        var dataTable = new PdfPTable(2);
                        dataTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        var labelCell = new PdfPCell(new Phrase(label, font));
                        labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(labelCell);

                        var valueCell = new PdfPCell(new Phrase(value, font));
                        valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(valueCell);

                        document.Add(dataTable);
                        document.Add(space);
                    }

                    AddData("Last name:", documentData.LastName.ToUpper());
                    AddData("First name:", documentData.FirstName.ToUpper());
                    AddData("Date of birth:", documentData.BirthDate.ToShortDateString());
                    AddData("Place of birth:", $"{documentData.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                    AddData("Residence and address:", $"{documentData.City.ToUpper()}, {documentData.Municipality.ToUpper()}, {documentData.Street.ToUpper()} {documentData.HouseNumber.ToUpper()}");
                    AddData("JMBG:", documentData.JMBG);
                    AddData("Date of first issue:", documentData.IssueDate.ToShortDateString());
                    AddData("Valid until:", documentData.ExpiryDate.ToShortDateString());
                    AddData("Issuing authority:", documentData.IssuedBy.ToUpper());
                    AddData("Driving license number:", documentData.DrivingLicenseNumber);
                    AddData("Categories:", documentData.VehicleCategories.ToUpper());

                    document.Add(line);
                    document.Add(new Paragraph("Document data", font));
                    document.Add(line);
                    AddData("Issued by:", documentData.IssuedBy.ToUpper());

                    document.Add(line);
                    document.Add(space);
                    document.Add(line);

                    document.Close();
                    writer.Close();
                    return ms.ToArray();
                }
            });
        }

        public async Task<byte[]> GenerateWeaponPermitPdfAsync(dynamic documentData)
        {
            return await Task.Run(() =>
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

                    var image = Image.GetInstance(documentData.Photo);
                    image.Alignment = Element.ALIGN_LEFT;
                    image.ScaleToFit(200, 200);

                    PdfPTable table = new PdfPTable(2);

                    PdfPCell cell1 = new PdfPCell(image);
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell1);

                    using (MemoryStream qrCodeStream = new MemoryStream(documentData.QRCode))
                    {
                        var qrCodeImage = Image.GetInstance(qrCodeStream);
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
                        var dataTable = new PdfPTable(2);
                        dataTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        var labelCell = new PdfPCell(new Phrase(label, font));
                        labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(labelCell);

                        var valueCell = new PdfPCell(new Phrase(value, font));
                        valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(valueCell);

                        document.Add(dataTable);
                        document.Add(space);
                    }

                    AddData("Last name:", documentData.LastName.ToUpper());
                    AddData("First name:", documentData.FirstName.ToUpper());
                    AddData("Parent's name:", documentData.ParentName.ToUpper());
                    AddData("Date of birth:", documentData.BirthDate.ToShortDateString());
                    AddData("Place of birth:", $"{documentData.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                    AddData("Residence and address:", $"{documentData.City.ToUpper()}, {documentData.Municipality.ToUpper()}, {documentData.Street.ToUpper()} {documentData.HouseNumber.ToUpper()}");
                    AddData("JMBG:", documentData.JMBG);
                    AddData("Gender:", documentData.Gender.ToString());

                    document.Add(line);
                    document.Add(new Paragraph("Document data", font));
                    document.Add(line);
                    AddData("Issued by:", documentData.IssuedBy.ToUpper());
                    AddData("Permit number:", documentData.WeaponPermitNumber);
                    AddData("Date of issue:", documentData.IssueDate.ToShortDateString());
                    AddData("Valid until:", documentData.ExpiryDate.ToShortDateString());
                    AddData("Weapon type:", documentData.WeaponTypes.ToUpper());
                    AddData("Weapon caliber:", documentData.WeaponCaliber.ToUpper());
                    AddData("Reason for issuing:", documentData.UsagePurpose.ToUpper());
                    AddData("Issuing authority:", documentData.IssuingAuthority.ToUpper());

                    document.Add(line);
                    document.Add(space);
                    document.Add(line);

                    document.Close();
                    writer.Close();
                    return ms.ToArray();
                }
            });
        }

        public async Task<byte[]> GenerateVehicleRegistrationPdfAsync(dynamic documentData)
        {
            return await Task.Run(() =>
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

                    var image = Image.GetInstance(documentData.Photo);
                    image.Alignment = Element.ALIGN_LEFT;
                    image.ScaleToFit(200, 200);

                    PdfPTable table = new PdfPTable(2);

                    PdfPCell cell1 = new PdfPCell(image);
                    cell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table.AddCell(cell1);

                    using (MemoryStream qrCodeStream = new MemoryStream(documentData.QRCode))
                    {
                        var qrCodeImage = Image.GetInstance(qrCodeStream);
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
                        var dataTable = new PdfPTable(2);
                        dataTable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        var labelCell = new PdfPCell(new Phrase(label, font));
                        labelCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        labelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(labelCell);

                        var valueCell = new PdfPCell(new Phrase(value, font));
                        valueCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        valueCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        dataTable.AddCell(valueCell);

                        document.Add(dataTable);
                        document.Add(space);
                    }

                    AddData("Last name:", documentData.LastName.ToUpper());
                    AddData("First name:", documentData.FirstName.ToUpper());
                    AddData("Parent's name:", documentData.ParentName.ToUpper());
                    AddData("Date of birth:", documentData.BirthDate.ToShortDateString());
                    AddData("Place of birth:", $"{documentData.BirthPlace.ToUpper()}, REPUBLIC OF SERBIA");
                    AddData("Residence and address:", $"{documentData.City.ToUpper()}, {documentData.Municipality.ToUpper()}, {documentData.Street.ToUpper()} {documentData.HouseNumber.ToUpper()}");
                    AddData("JMBG:", documentData.JMBG);
                    AddData("Date of first issue:", documentData.IssueDate.ToShortDateString());
                    AddData("Valid until:", documentData.ExpiryDate.ToShortDateString());
                    AddData("Registration number:", documentData.RegistrationNumber.ToUpper());
                    AddData("Vehicle brand:", documentData.Brand.ToUpper());
                    AddData("Vehicle model:", documentData.Brand.ToUpper());
                    AddData("Vehicle type:", documentData.Type.ToUpper());
                    AddData("Engine serial number:", documentData.EngineNumber.ToUpper());
                    AddData("Chassis number:", documentData.ChassisNumber.ToUpper());
                    AddData("Engine capacity:", documentData.LoadCapacity.ToUpper());
                    AddData("Engine power:", documentData.Weight.ToUpper());
                    AddData("Number of seats:", documentData.SeatNumber.ToString());
                    AddData("Vehicle mass:", documentData.LoadCapacity.ToString());
                    AddData("Maximum permissible mass:", documentData.Weight.ToString());

                    document.Add(line);
                    document.Add(new Paragraph("Document data", font));
                    document.Add(line);
                    AddData("Issued by:", documentData.IssuedBy.ToUpper());

                    document.Add(line);
                    document.Add(space);
                    document.Add(line);

                    document.Close();
                    writer.Close();
                    return ms.ToArray();
                }
            });
        }
    }
}