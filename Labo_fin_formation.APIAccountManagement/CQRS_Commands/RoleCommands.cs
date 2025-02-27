using MediatR;

namespace Labo_fin_formation.APIAccountManagement.CQRS_Commands
{
    public class RoleCommands
    {
        public record AddRoleCommand(string UserId, string RoleName) : IRequest;

        public record RemoveRoleCommand(string UserId, string RoleName) : IRequest;
    }
}
