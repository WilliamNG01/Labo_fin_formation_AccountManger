using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using MediatR;

namespace Labo_fin_formation.APIAccountManagement.CQRS_Commands
{
    public class UserCommands
    {
        public record RegisterUserCommand(RegisterDto model) : IRequest<string>;
        public record LoginUserCommand(LoginDto model) : IRequest<ApplicationUser>;
        public record LogoutUserCommand(string UserName) : IRequest;
        public record ActivateUserCommand(string UserId) : IRequest;
        public record DeactivateUserCommand(string UserId) : IRequest;
        public record Activate2FAUserCommand(string UserId) : IRequest;
        public record Deactivate2FAUserCommand(string UserId) : IRequest;
    }
}
