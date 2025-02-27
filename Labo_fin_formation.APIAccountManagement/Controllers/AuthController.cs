using Azure.Core;
using JwtConfiguration;
using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using Labo_fin_formation.APIAccountManagement.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Labo_fin_formation.APIAccountManagement.CQRS_Commands.UserCommands;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

[ApiController]
[Route("[controller]")]
public class AuthController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IMediator _mediator;
    private readonly IJwtTokenServices _jwtTokenServices; // Ajoutez votre service JWT
    public AuthController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IMediator mediator, IJwtTokenServices jwtTokenServices)
    {
        _mediator = mediator;
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenServices = jwtTokenServices;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        ApplicationUser user = await _mediator.Send(model);

        if (user == null)
        {
            return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
        }

        // Générer le token JWT
        IList<string> userRoles = await _userManager.GetRolesAsync(user);
        var roles = _roleManager.Roles.Where(r => userRoles.Contains(r.Name)).OrderBy(x => x.RoleLevel).ToList();
        user.UserName = user.UserName == null ? user.FirstName + user.LastName : user.UserName;
        var token = _jwtTokenServices.GenerateJwtToken(roles, user);

        return Ok(new
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            user.UserName,
            Roles = roles
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutUserCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Déconnexion réussie." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        if (!model.model.GdprAccepted)
            return BadRequest(ModelState);
        
        if(String.IsNullOrEmpty(model.model.Role))
            model.model.Role = _roleManager.Roles.Where(x => x.RoleLevel == 0).Select(x => x.Name).FirstOrDefault();
        else if(!(_roleManager.Roles.Any(x => x.Name == model.model.Role)))
            return BadRequest("Erreur lors de l'enregistrement de l'utilisateur. Le role n'existe pas");

        string username = await _mediator.Send(model);
        if (username == null)
        {
            return BadRequest("Erreur lors de l'enregistrement de l'utilisateur.");
        }
        return Ok(new { Message = "Utilisateur créé avec succès.", UserName = username });
    }
}
