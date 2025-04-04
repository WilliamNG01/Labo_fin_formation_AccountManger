﻿using Labo_fin_formation.DocumentManager.Models;
using MediatR;

namespace Labo_fin_formation.DocumentManager.CQRS_Queries
{
    public class DocumentQueries
    {
        public record GetDocumentsQuery(DocumentFilter Filter) : IRequest<List<DocumentDTO>>;
        public record GetALLDocumentsQuery() : IRequest<List<DocumentDTO>>;// just pour tester: à retirer
        public record GetDocumentByIdQuery(Guid Id) : IRequest<DocumentDTO>;
    }
}
