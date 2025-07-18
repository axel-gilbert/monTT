using Microsoft.AspNetCore.Mvc;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Services;

namespace TeleworkManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Inscription d'un nouvel utilisateur
        /// </summary>
        /// <param name="registerDto">Données d'inscription</param>
        /// <returns>Token d'authentification et informations utilisateur</returns>
        /// <response code="201">Utilisateur créé avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="409">Email déjà utilisé</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return CreatedAtAction(nameof(Login), result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Connexion d'un utilisateur
        /// </summary>
        /// <param name="loginDto">Données de connexion</param>
        /// <returns>Token d'authentification et informations utilisateur</returns>
        /// <response code="200">Connexion réussie</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Email ou mot de passe incorrect</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Renouvellement du token d'authentification
        /// </summary>
        /// <param name="refreshTokenDto">Token de renouvellement</param>
        /// <returns>Nouveau token d'authentification</returns>
        /// <response code="200">Token renouvelé avec succès</response>
        /// <response code="400">Token invalide</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
                return Ok(result);
            }
            catch (NotImplementedException)
            {
                return BadRequest(new { message = "Refresh token non implémenté dans cette version." });
            }
        }
    }
} 