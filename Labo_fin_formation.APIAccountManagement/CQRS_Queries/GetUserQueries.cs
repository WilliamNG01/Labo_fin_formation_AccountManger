using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using MediatR;

namespace Labo_fin_formation.APIAccountManagement.CQRS_Queries
{
    public class GetUserQueries
    {
        public record GetUserQuery(string UserId) : IRequest<UserDto>;
    }
}
