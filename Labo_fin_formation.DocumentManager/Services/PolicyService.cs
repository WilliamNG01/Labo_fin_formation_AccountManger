using Dapper;
using Labo_fin_formation.DocumentManager.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace Labo_fin_formation.DocumentManager.Services
{
    public class PolicyService
    {
        private readonly SqlConnection _connection; // Ou votre contexte de base de données

        public PolicyService(SqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<string>> GetPoliciesAsync()
        {
            var query = @"SELECT Name as PolicyName
                        FROM [fl_db_medical_documents].[scty].Policies";

            return await _connection.QueryAsync<string>(query);
        }
        public async Task<IEnumerable<PolicyForServicesModel>> GetRolePoliciesAsync()
        {
            var query = @"SELECT rp.[RoleName]
	                          ,p.Name as PolicyName
                        FROM [fl_db_medical_documents].[scty].[RolePolicies] as rp
                          join [fl_db_medical_documents].[scty].Policies as p on rp.PolicyId = p.Id";

            return await _connection.QueryAsync<PolicyForServicesModel>(query);
        }
    }
}
