using Labo_fin_formation.DocumentManager.Models;

namespace Labo_fin_formation.DocumentManager.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<DocumentDTO>> GetFilteredDocumentsAsync(DocumentFilter filter);
        Task<IEnumerable<DocumentDTO>> GetAllDocumentsAsync();
    }
}