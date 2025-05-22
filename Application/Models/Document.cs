namespace Models;
[Table("Document")]
public class Document
{
    [Key]
    public int DocumentID { get; set; }

    [MaxLength(50)]
    public string QRCode { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [MaxLength(50)]
    [Required]
    public string IssuedBy { get; set; }

    [MaxLength(20)]
    public string Name { get; set; }

    // Generates a QR code with a logo for the document
    public byte[] GenerateQRCode()
    {
        // Create QR code
        var logoBitmap = GetLogo();
        AnyBitmap logoImage = new AnyBitmap(logoBitmap);
        QRCodeLogo qrCodeLogo = new QRCodeLogo(logoImage);
        var qrCode = QRCodeWriter.CreateQrCodeWithLogo(this.QRCode, qrCodeLogo, 200, 0);
        var qrCodeBitmapPNG = qrCode.ToPngBinaryData();
        return qrCodeBitmapPNG;
    }

    public byte[] GetLogo()
    {
        var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photos", "logo.jpg");
        var bitmap = new System.Drawing.Bitmap(logoPath);

        using (var memoryStream = new MemoryStream())
        {
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return memoryStream.ToArray();
        }
    }
}