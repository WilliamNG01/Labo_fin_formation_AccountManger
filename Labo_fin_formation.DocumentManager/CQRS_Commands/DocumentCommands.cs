using MediatR;

namespace Labo_fin_formation.DocumentManager.CQRS_Commands
{
    public class DocumentCommands
    {
        public record CreateDocumentCommand(string Name, string Path, string ConfidentialityLevel, Guid CreatedBy) : IRequest<Guid>;
        public record UpdateDocumentCommand(Guid Id, string Name, string Path, string ConfidentialityLevel) : IRequest;
        public record DeleteDocumentCommand(Guid Id) : IRequest;
    }
}
