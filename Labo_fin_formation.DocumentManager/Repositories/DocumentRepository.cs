using Dapper;
using Labo_fin_formation.DocumentManager.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Data;

namespace Labo_fin_formation.DocumentManager.Repositories;

public class DocumentRepository: IDocumentRepository
{
    private readonly SqlConnection _dbConnection;

    public DocumentRepository(SqlConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<DocumentDTO>> GetFilteredDocumentsAsync(DocumentFilter filter)
    {
        var query = @"SELECT 
                        d.Id, d.Name, d.Path, d.ConfidentialityLevel, 
                        d.CreatedBy, u1.Name AS CreatedByName, d.CreatedAt, 
                        d.DeletedBy, d.DeletedAt, d.UpdatedBy, d.UpdatedAt
                      FROM documents.Documents d
                      LEFT JOIN users u1 ON d.CreatedBy = u1.Id
                      WHERE (ISNULL(@SearchTerm, '') = '' OR d.Name LIKE '%' + @SearchTerm + '%' OR d.Path LIKE '%' + @SearchTerm + '%')
                      AND (ISNULL(@ConfidentialityLevel, '') = '' OR d.ConfidentialityLevel = @ConfidentialityLevel)
                      ORDER BY 
                        CASE WHEN @OrderBy = 'Name' THEN d.Name END ASC,
                        CASE WHEN @OrderBy = 'CreatedAt' THEN d.CreatedAt END DESC,
                        CASE WHEN @Descending = 0 THEN d.CreatedAt END ASC
                      OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";

        var parameters = new
        {
            SearchTerm = filter.SearchTerm,
            ConfidentialityLevel = filter.ConfidentialityLevel,
            OrderBy = filter.OrderBy,
            Descending = filter.Descending ? 1 : 0,
            Offset = (filter.PageNumber - 1) * filter.PageSize,
            PageSize = filter.PageSize
        };

        return await _dbConnection.QueryAsync<DocumentDTO>(query, parameters);
    }
    public async Task<IEnumerable<DocumentDTO>> GetAllDocumentsAsync()
    {
        //List<DocumentDTO> utilisateurs = new List<DocumentDTO>();

        //using (SqlCommand command = _dbConnection.CreateCommand())
        //{
        //    command.CommandText = @"SELECT 
        //                              d.Id, d.Name, d.Path, d.PriorityLevel, 
        //                              d.CreatedBy, d.Name AS CreatedByName, d.CreatedAt,
        //                              d.DeletedBy, d.DeletedAt, d.UpdatedBy, d.UpdatedAt
        //                            FROM [fl_db_medical_documents].[dcmt].[Documents] d";
        //    command.CommandType = CommandType.Text;

        //    _dbConnection.Open();
        //    using (SqlDataReader reader = command.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            utilisateurs.Add(
        //                new DocumentDTO
        //                {
        //                    Name = reader.GetString("Name"),
        //                    Path = reader.GetString("Path"),
        //                    PriorityLevel = reader.GetString("PriorityLevel"),
        //                    CreatedByName = reader.GetString("CreatedByName"),
        //                }
        //            );
        //        }
        //        _dbConnection.Close();
        //        return utilisateurs;
        //    }

        //}

        var query = @"SELECT 
                        d.Id, d.Name, d.Path, d.PriorityLevel, 
                        d.CreatedBy, d.Name AS CreatedByName, d.CreatedAt, 
                        d.DeletedBy, d.DeletedAt, d.UpdatedBy, d.UpdatedAt
                      FROM [fl_db_medical_documents].[dcmt].[Documents] d";

        //var c =  await _dbConnection.QueryAsync(query);
        return await _dbConnection.QueryAsync<DocumentDTO>(query);
    }
}

