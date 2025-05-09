using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using MediatR;

namespace Labo_fin_formation.APIAccountManagement.CQRS_Queries
{
    public class GetUserQueries
    {
        public record GetUserByIDQuery(string UserId) : IRequest<UserDto>;
        public record GetAllUserQuery() : IRequest<List<UserDto>>;
    }
}
