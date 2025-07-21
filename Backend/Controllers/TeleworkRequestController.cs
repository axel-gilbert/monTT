using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Services;

namespace TeleworkManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeleworkRequestController : ControllerBase
    {
        private readonly ITeleworkRequestService _teleworkRequestService;

        public TeleworkRequestController(ITeleworkRequestService teleworkRequestService)
        {
            _teleworkRequestService = teleworkRequestService;
        }

        /// <summary>
        /// Créer une nouvelle demande de télétravail
        /// </summary>
        /// <remarks>
        /// Permet à un employé de soumettre une demande de télétravail pour une date donnée.
        /// 
        /// Exemple de requête :
        /// {
        ///   "TeleworkDate": "2024-06-20",
        ///   "Reason": "Travail sur un dossier nécessitant de la concentration."
        /// }
        /// 
        /// Exemple de réponse (201) :
        /// {
        ///   "id": 1,
        ///   "requestDate": "2024-06-10T09:00:00Z",
        ///   "teleworkDate": "2024-06-20",
        ///   "reason": "Travail sur un dossier nécessitant de la concentration.",
        ///   "status": "Pending",
        ///   "employee": { ... }
        /// }
        /// </remarks>
        /// <param name="createDto">Données de la demande</param>
        /// <returns>Demande créée</returns>
        /// <response code="201">Demande créée avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="409">Demande déjà existante pour cette date</response>
        [HttpPost]
        [ProducesResponseType(typeof(TeleworkRequestDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<TeleworkRequestDto>> CreateRequest(CreateTeleworkRequestDto createDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var request = await _teleworkRequestService.CreateRequestAsync(createDto, userId);
                return CreatedAtAction(nameof(GetMyRequests), request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir mes demandes de télétravail
        /// </summary>
        /// <remarks>
        /// Retourne la liste de toutes les demandes de télétravail soumises par l'utilisateur connecté.
        /// </remarks>
        /// <returns>Liste de mes demandes</returns>
        /// <response code="200">Demandes récupérées avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="404">Employé non trouvé</response>
        [HttpGet("my-requests")]
        [ProducesResponseType(typeof(List<TeleworkRequestDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<TeleworkRequestDto>>> GetMyRequests()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var requests = await _teleworkRequestService.GetMyRequestsAsync(userId);
                return Ok(requests);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir les demandes de télétravail de l'entreprise (Manager uniquement)
        /// </summary>
        /// <remarks>
        /// Permet à un manager de consulter toutes les demandes de télétravail des employés de son entreprise.
        /// </remarks>
        /// <returns>Liste des demandes de l'entreprise</returns>
        /// <response code="200">Demandes récupérées avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Aucune entreprise trouvée</response>
        [HttpGet("company")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(List<TeleworkRequestDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<TeleworkRequestDto>>> GetCompanyRequests()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var requests = await _teleworkRequestService.GetCompanyRequestsAsync(userId);
                return Ok(requests);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Traiter une demande de télétravail (Manager uniquement)
        /// </summary>
        /// <remarks>
        /// Permet à un manager d'approuver ou de rejeter une demande de télétravail d'un employé.
        /// 
        /// Exemple de requête :
        /// {
        ///   "Status": "Approved",
        ///   "ManagerComment": "OK pour ce jour."
        /// }
        /// </remarks>
        /// <param name="id">ID de la demande</param>
        /// <param name="processDto">Données de traitement</param>
        /// <returns>Demande traitée</returns>
        /// <response code="200">Demande traitée avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Demande non trouvée</response>
        [HttpPut("{id}/process")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(TeleworkRequestDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TeleworkRequestDto>> ProcessRequest(int id, ProcessTeleworkRequestDto processDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var request = await _teleworkRequestService.ProcessRequestAsync(id, processDto, userId);
                return Ok(request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir le planning hebdomadaire de l'entreprise (Manager uniquement)
        /// </summary>
        /// <remarks>
        /// Permet à un manager de visualiser le planning des demandes de télétravail de son entreprise pour une semaine donnée.
        /// </remarks>
        /// <param name="weekStart">Date de début de semaine (format: yyyy-MM-dd)</param>
        /// <returns>Planning hebdomadaire</returns>
        /// <response code="200">Planning récupéré avec succès</response>
        /// <response code="400">Date invalide</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Aucune entreprise trouvée</response>
        [HttpGet("company/weekly-planning")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(WeeklyPlanningDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<WeeklyPlanningDto>> GetWeeklyPlanning([FromQuery] DateTime weekStart)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var planning = await _teleworkRequestService.GetWeeklyPlanningAsync(userId, weekStart);
                return Ok(planning);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
} 