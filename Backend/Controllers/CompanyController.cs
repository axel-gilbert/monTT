using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Services;

namespace TeleworkManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Créer une nouvelle entreprise
        /// </summary>
        /// <remarks>
        /// Permet à un manager de créer une nouvelle entreprise.
        /// 
        /// Exemple de requête :
        /// {
        ///   "name": "TechCorp Solutions"
        /// }
        /// </remarks>
        /// <param name="createDto">Données de l'entreprise</param>
        /// <returns>Entreprise créée</returns>
        /// <response code="201">Entreprise créée avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="409">Manager a déjà une entreprise</response>
        [HttpPost]
        [ProducesResponseType(typeof(CompanyDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto createDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var company = await _companyService.CreateCompanyAsync(createDto, userId);
                return CreatedAtAction(nameof(GetMyCompany), company);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir les informations de mon entreprise
        /// </summary>
        /// <remarks>
        /// Retourne les informations de l'entreprise gérée par le manager connecté.
        /// </remarks>
        /// <returns>Informations de l'entreprise</returns>
        /// <response code="200">Entreprise récupérée avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Aucune entreprise trouvée</response>
        [HttpGet("my-company")]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CompanyDto>> GetMyCompany()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var company = await _companyService.GetMyCompanyAsync(userId);
                return Ok(company);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mettre à jour les informations de mon entreprise
        /// </summary>
        /// <remarks>
        /// Permet au manager de modifier le nom de son entreprise.
        /// 
        /// Exemple de requête :
        /// {
        ///   "name": "Nouvelle société"
        /// }
        /// </remarks>
        /// <param name="updateDto">Données de mise à jour</param>
        /// <returns>Entreprise mise à jour</returns>
        /// <response code="200">Entreprise mise à jour avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Aucune entreprise trouvée</response>
        [HttpPut]
        [ProducesResponseType(typeof(CompanyDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CompanyDto>> UpdateCompany(UpdateCompanyDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var company = await _companyService.UpdateCompanyAsync(updateDto, userId);
                return Ok(company);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir les informations de mon entreprise avec la liste des employés
        /// </summary>
        /// <remarks>
        /// Retourne les informations de l'entreprise et la liste de tous les employés assignés à cette entreprise.
        /// </remarks>
        /// <returns>Entreprise avec liste des employés</returns>
        /// <response code="200">Données récupérées avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Aucune entreprise trouvée</response>
        [HttpGet("my-company/employees")]
        [ProducesResponseType(typeof(CompanyWithEmployeesDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<CompanyWithEmployeesDto>> GetMyCompanyWithEmployees()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var company = await _companyService.GetCompanyWithEmployeesAsync(userId);
                return Ok(company);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
} 