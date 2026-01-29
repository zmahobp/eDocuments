using Models;
using DocModel = Models.Document;

namespace Services
{
    public interface IDocumentService<T> where T : DocModel
    {
        Task<object> GetDocumentAsync(int userId);
        Task<string> AddDocumentAsync(int userId, T document);
        Task<string> UpdateDocumentAsync(int userId, T document);
        Task<string> DeleteDocumentAsync(int userId);
        Task<byte[]> GeneratePdfAsync(int userId, T document);
        Task<byte[]> GenerateQrCodeAsync(T document);
    }
}