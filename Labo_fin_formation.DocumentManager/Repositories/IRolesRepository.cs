using Labo_fin_formation.DocumentManager.Models;

namespace Labo_fin_formation.DocumentManager.Repositories
{
    public interface IRolesRepository
    {
        Task<IEnumerable<PolicyDTO>> GetAllPoliciesAsync();
        Task<PolicyDTO> GetPolicyAsync(string policyId);
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync(DocumentFilter filter);
        Task<RoleDTO> GetRoleAsync(string rolename);
        Task<RoleDTO> GetPolicyRoleAsync(string rolename);
    }
}