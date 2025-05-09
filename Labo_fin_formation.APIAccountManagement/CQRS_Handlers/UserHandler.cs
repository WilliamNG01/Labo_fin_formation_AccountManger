using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using static Labo_fin_formation.APIAccountManagement.CQRS_Commands.UserCommands;
using System.Threading.Tasks;
using static Labo_fin_formation.APIAccountManagement.CQRS_Queries.GetUserQueries;
using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Labo_fin_formation.APIAccountManagement.CQRS_Handlers
{
    public class UserHandler
    {
        public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, string>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public RegisterUserHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<string?> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
            {
                if ((bool)!request.model.GdprAccepted)
                    return null;
                var user = new ApplicationUser
                {
                    UserName = (String.IsNullOrEmpty(request.model.UserName) ? request.model.FirstName + request.model.LastName : request.model.UserName),
                    Email = request.model.Email,
                    FirstName = request.model.FirstName,
                    LastName = request.model.LastName,
                    PhoneNumber = request.model.PhoneNumber,
                    GDPRConsent = request.model.GdprAccepted,
                    TwoFactorEnabled = request.model.TwoFactorEnabled
                };
                var result = await _userManager.CreateAsync(user, request.model.Password);
                if (result.Succeeded)
                {  // Add the user to the specified role
                    var roleResult = await _userManager.AddToRoleAsync(user, request.model.Role);

                    if (!roleResult.Succeeded)
                    {
                        // Handle role assignment failure
                        // Optionally, you might want to delete the created user
                        // if role assignment fails.
                        var err = IdentityResult.Failed(roleResult.Errors.ToArray());
                        return err.Errors.ToString();
                    }
                    return user.UserName;
                }

                return null;
            }
        }


        public class LoginUserHandler : IRequestHandler<LoginUserCommand, ApplicationUser>
        {
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly UserManager<ApplicationUser> _userManager;

            public LoginUserHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
            {
                _signInManager = signInManager;
                _userManager = userManager;
            }

            public async Task<ApplicationUser> Handle(LoginUserCommand request, CancellationToken cancellationToken)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(request.model.Email);
                if (user == null) { return null; }

                bool pwd = await _userManager.CheckPasswordAsync(user, request.model.Password);
                if (pwd == false) { return null; }

                return user;
            }
        }

        public class LogoutUserHandler : IRequestHandler<LogoutUserCommand>
        {
            private readonly SignInManager<ApplicationUser> _signInManager;

            public LogoutUserHandler(SignInManager<ApplicationUser> signInManager)
            {
                _signInManager = signInManager;
            }

            public async Task Handle(LogoutUserCommand request, CancellationToken cancellationToken)
            {
                await _signInManager.SignOutAsync();
            }
        }

        public class GetAllUserdHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public GetAllUserdHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
            {
                List<ApplicationUser> appUsers = await _userManager.Users.ToListAsync(cancellationToken);
                //if (appUser == null) return [];
                //List<UserDto> listUser = (List<UserDto>)appUser.Select(user => new UserDto(user, _userManager.GetRolesAsync(user).Result));
                //return listUser;
                var userDtoTasks = appUsers.Select(async user =>
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    return new UserDto(user, roles);
                });

                return (await Task.WhenAll(userDtoTasks)).ToList();
            }

        }
        public class GetUserByIdHandler: IRequestHandler<GetUserByIDQuery, UserDto>
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public GetUserByIdHandler(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            public async Task<UserDto?> Handle(GetUserByIDQuery request, CancellationToken cancellationToken)
            {
                ApplicationUser appUser =  await _userManager.FindByIdAsync(request.UserId);
                if (appUser == null) return null;
                
                return new UserDto(appUser, await _userManager.GetRolesAsync(appUser));
            }

        }
    }
}
