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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Obtenir le profil de l'utilisateur connecté
        /// </summary>
        /// <returns>Profil de l'employé connecté</returns>
        /// <response code="200">Profil récupéré avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="404">Employé non trouvé</response>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeDto>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var profile = await _employeeService.GetProfileAsync(userId);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mettre à jour le profil de l'utilisateur connecté
        /// </summary>
        /// <param name="updateDto">Données de mise à jour</param>
        /// <returns>Profil mis à jour</returns>
        /// <response code="200">Profil mis à jour avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="404">Employé non trouvé</response>
        [HttpPut("profile")]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeDto>> UpdateProfile(UpdateEmployeeDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var profile = await _employeeService.UpdateProfileAsync(userId, updateDto);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir la liste de tous les employés (Manager uniquement)
        /// </summary>
        /// <returns>Liste des employés</returns>
        /// <response code="200">Liste récupérée avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        [HttpGet]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(List<EmployeeListDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<List<EmployeeListDto>>> GetAllEmployees()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var employees = await _employeeService.GetAllEmployeesAsync(userId);
                return Ok(employees);
            }
            catch (InvalidOperationException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Assigner un employé à une entreprise (Manager uniquement)
        /// </summary>
        /// <param name="assignDto">Données d'assignation</param>
        /// <returns>Employé assigné</returns>
        /// <response code="200">Employé assigné avec succès</response>
        /// <response code="400">Données invalides</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Employé ou entreprise non trouvé</response>
        [HttpPost("assign-to-company")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeDto>> AssignToCompany(AssignEmployeeToCompanyDto assignDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var employee = await _employeeService.AssignToCompanyAsync(assignDto, userId);
                return Ok(employee);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtenir un employé par son ID (Manager uniquement)
        /// </summary>
        /// <param name="id">ID de l'employé</param>
        /// <returns>Détails de l'employé</returns>
        /// <response code="200">Employé récupéré avec succès</response>
        /// <response code="401">Non authentifié</response>
        /// <response code="403">Accès interdit - Manager uniquement</response>
        /// <response code="404">Employé non trouvé</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(EmployeeDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _employeeService.GetEmployeeByIdAsync(id);
                return Ok(employee);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
} 