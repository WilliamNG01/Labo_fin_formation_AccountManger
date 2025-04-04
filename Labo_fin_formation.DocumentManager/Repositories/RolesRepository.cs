using Dapper;
using Labo_fin_formation.DocumentManager.Models;
using Microsoft.Data.SqlClient;

namespace Labo_fin_formation.DocumentManager.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly SqlConnection _dbConnection;

    public RolesRepository(SqlConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<PolicyDTO>> GetAllPoliciesAsync()
    {

        var query = @"SELECT * FROM [fl_db_medical_documents].[scty].[Policies]";

        return await _dbConnection.QueryAsync<PolicyDTO>(query);
    }

    public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync(DocumentFilter filter)
    {

        var query = @"SELECT 
                        d.Id, d.Name, d.Path, d.PriorityLevel, 
                        d.CreatedBy, u1.Name AS CreatedByName, d.CreatedAt, 
                        d.DeletedBy, d.DeletedAt, d.UpdatedBy, d.UpdatedAt
                      FROM [fl_db_medical_documents].[dcmt].[Documents] d";

        return await _dbConnection.QueryAsync<RoleDTO>(query);
    }

    public async Task<PolicyDTO> GetPolicyAsync(string policyId)
    {

        var query = @"SELECT * FROM [fl_db_medical_documents].[scty].[Policies] Where id= @PolicyId";
        var parameters = new
        {
            PolicyId = policyId
        };
        return (await _dbConnection.QueryAsync<PolicyDTO>(query, parameters)).FirstOrDefault();
    }

    public async Task<RoleDTO> GetRoleAsync(string roleName)
    {
        var query = @"SELECT * FROM [fl_db_medical_documents].[scty].[RolePolicies] Where RoleName = @RoleName";
        var parameters = new
        {
            RoleName = roleName
        };

        return (await _dbConnection.QueryAsync<RoleDTO>(query)).FirstOrDefault();
    }
    public async Task<RoleDTO> GetPolicyRoleAsync(string roleName)
    {
        var query = @"SELECT rp.[RoleName]
                              ,rp.[PolicyId]
	                          ,p.Name
                    FROM [fl_db_medical_documents].[scty].[RolePolicies] as rp
                          join [fl_db_medical_documents].[scty].Policies as p on rp.PolicyId = p.Id
                    Where RoleName = @RoleName";

        var parameters = new
        {
            RoleName = roleName
        };

        return (await _dbConnection.QueryAsync<RoleDTO>(query)).FirstOrDefault();
    }
}

