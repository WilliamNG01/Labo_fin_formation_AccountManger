using Labo_fin_formation.DocumentManager.Models;
using Labo_fin_formation.DocumentManager.Repositories;
using MediatR;
using static Labo_fin_formation.DocumentManager.CQRS_Commands.DocumentCommands;
using static Labo_fin_formation.DocumentManager.CQRS_Queries.DocumentQueries;

namespace Labo_fin_formation.DocumentManager.CQRS_Handlers;

public class DocumentsHandlers
{
    public class GetAllDocumentsHandler(IDocumentRepository documentRepository) : IRequestHandler<GetALLDocumentsQuery, List<DocumentDTO>>
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;

        public async Task<List<DocumentDTO>> Handle(GetALLDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = await _documentRepository.GetAllDocumentsAsync();
            return (List<DocumentDTO>)documents;
        }
    }
    public class GetDocumentsHandler(IDocumentRepository documentRepository) : IRequestHandler<GetDocumentsQuery, List<DocumentDTO>>
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;

        public async Task<List<DocumentDTO>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        {
            var documents = await _documentRepository.GetFilteredDocumentsAsync(request.Filter);
            return (List<DocumentDTO>)documents;
        }
    }

    public class GetDocumentByIdHandler(IDocumentRepository documentRepository) : IRequestHandler<GetDocumentByIdQuery, DocumentDTO>
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;
        public async Task<DocumentDTO> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            // Implémentation de la récupération d'un document spécifique
            return null; // Remplacer par une requête à la base de données
        }
    }

    public class CreateDocumentHandler(IDocumentRepository documentRepository) : IRequestHandler<CreateDocumentCommand, Guid>
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;
        public async Task<Guid> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
        {
            // Implémentation de la création d'un document
            return Guid.NewGuid(); // Remplacer par l'insertion en base
        }
    }

    public class UpdateDocumentHandler(IDocumentRepository documentRepository) : IRequestHandler<UpdateDocumentCommand>
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;
        public async Task Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
        {
            // Implémentation de la mise à jour d'un document
        }
    }

    public class DeleteDocumentHandler(IDocumentRepository documentRepository) : IRequestHandler<DeleteDocumentCommand>
    {
        private readonly IDocumentRepository _documentRepository = documentRepository;
        public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
        {
            // Implémentation de la suppression d'un document
        }
    }
}
