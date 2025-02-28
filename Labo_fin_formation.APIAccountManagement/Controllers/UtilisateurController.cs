using Labo_fin_formation.APIAccountManagement.Domain.Entities;
using Labo_fin_formation.APIAccountManagement.Models.DTOs;
using Labo_fin_formation.APIAccountManagement.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Labo_fin_formation.APIAccountManagement.CQRS_Queries.GetUserQueries;

namespace Labo_fin_formation.APIAccountManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateurController : Controller
    {
        private readonly IMediator _mediator;
        public UtilisateurController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("search")]
        [Authorize(Roles= "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> search([FromQuery] string userid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { error = "Vous devez être authentifié pour accéder à cette ressource." });
            }

            var query = new GetUserByIDQuery(userid);

            UserDto user = await _mediator.Send(query);
           if (user == null) {return NotFound();}
            return Ok(user);
        }
        [HttpGet("AllUsers")]
        [Authorize]
        public async Task<IActionResult> AllUsers()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { error = "Vous devez être authentifié pour accéder à cette ressource." });
            }
            return Ok();
        }
    }
}
