using Microsoft.AspNetCore.Mvc;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Services;

namespace TeleworkManagementAPI.Controllers
{
    /// <summary>
    /// Contrôleur pour l'authentification des utilisateurs
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
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
        /// <remarks>
        /// Crée un nouveau compte utilisateur avec les informations fournies.
        /// L'utilisateur peut choisir son rôle (User ou Manager).
        /// 
        /// Exemple de requête :
        /// {
        ///   "email": "john.doe@entreprise.com",
        ///   "password": "MotDePasse123!",
        ///   "firstName": "John",
        ///   "lastName": "Doe",
        ///   "position": "Développeur Senior",
        ///   "role": "User"
        /// }
        /// </remarks>
        /// <param name="registerDto">Données d'inscription de l'utilisateur</param>
        /// <returns>Token d'authentification et informations utilisateur</returns>
        /// <response code="201">Utilisateur créé avec succès. Retourne le token JWT et les informations du profil.</response>
        /// <response code="400">Données invalides. Vérifiez le format des données envoyées.</response>
        /// <response code="409">Email déjà utilisé. Choisissez une autre adresse email.</response>
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
        /// <remarks>
        /// Authentifie un utilisateur avec son email et mot de passe.
        /// Retourne un token JWT valide pour les requêtes authentifiées.
        /// 
        /// Exemple de requête :
        /// {
        ///   "email": "john.doe@entreprise.com",
        ///   "password": "MotDePasse123!"
        /// }
        /// </remarks>
        /// <param name="loginDto">Données de connexion</param>
        /// <returns>Token d'authentification et informations utilisateur</returns>
        /// <response code="200">Connexion réussie. Retourne le token JWT et les informations du profil.</response>
        /// <response code="400">Données invalides. Vérifiez le format des données envoyées.</response>
        /// <response code="401">Email ou mot de passe incorrect.</response>
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
        /// <remarks>
        /// Renouvelle le token JWT à partir d'un refresh token.
        /// Cette fonctionnalité sera implémentée dans une version future.
        /// 
        /// Exemple de requête :
        /// {
        ///   "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
        /// }
        /// </remarks>
        /// <param name="refreshTokenDto">Token de renouvellement</param>
        /// <returns>Nouveau token d'authentification</returns>
        /// <response code="200">Token renouvelé avec succès.</response>
        /// <response code="400">Token invalide ou fonctionnalité non implémentée.</response>
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