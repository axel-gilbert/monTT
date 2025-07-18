using Microsoft.EntityFrameworkCore;
using TeleworkManagementAPI.Data;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Models;

namespace TeleworkManagementAPI.Services
{
    public interface ITeleworkRequestService
    {
        Task<TeleworkRequestDto> CreateRequestAsync(CreateTeleworkRequestDto createDto, int userId);
        Task<List<TeleworkRequestDto>> GetMyRequestsAsync(int userId);
        Task<List<TeleworkRequestListDto>> GetCompanyRequestsAsync(int managerId);
        Task<TeleworkRequestDto> ProcessRequestAsync(int requestId, ProcessTeleworkRequestDto processDto, int managerId);
        Task<WeeklyPlanningDto> GetWeeklyPlanningAsync(int managerId, DateTime weekStart);
    }

    public class TeleworkRequestService : ITeleworkRequestService
    {
        private readonly ApplicationDbContext _context;

        public TeleworkRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TeleworkRequestDto> CreateRequestAsync(CreateTeleworkRequestDto createDto, int userId)
        {
            // Vérifier que l'utilisateur existe et est un employé
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                throw new InvalidOperationException("Employé non trouvé.");

            // Vérifier que la date de télétravail est dans le futur
            if (createDto.TeleworkDate.Date <= DateTime.UtcNow.Date)
                throw new InvalidOperationException("La date de télétravail doit être dans le futur.");

            // Vérifier qu'il n'y a pas déjà une demande pour cette date
            var existingRequest = await _context.TeleworkRequests
                .FirstOrDefaultAsync(tr => tr.EmployeeId == employee.Id && 
                                         tr.TeleworkDate.Date == createDto.TeleworkDate.Date &&
                                         tr.Status != "Rejected");

            if (existingRequest != null)
                throw new InvalidOperationException("Vous avez déjà une demande de télétravail pour cette date.");

            // Créer la demande
            var request = new TeleworkRequest
            {
                EmployeeId = employee.Id,
                RequestDate = DateTime.UtcNow,
                TeleworkDate = createDto.TeleworkDate,
                Reason = createDto.Reason,
                Status = "Pending"
            };

            _context.TeleworkRequests.Add(request);
            await _context.SaveChangesAsync();

            return new TeleworkRequestDto
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                RequestDate = request.RequestDate,
                TeleworkDate = request.TeleworkDate,
                Reason = request.Reason,
                Status = request.Status,
                ManagerComment = request.ManagerComment,
                ProcessedAt = request.ProcessedAt,
                ProcessedByManagerId = request.ProcessedByManagerId,
                ProcessedByManagerName = null
            };
        }

        public async Task<List<TeleworkRequestDto>> GetMyRequestsAsync(int userId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                throw new InvalidOperationException("Employé non trouvé.");

            var requests = await _context.TeleworkRequests
                .Include(tr => tr.ProcessedByManager)
                .Where(tr => tr.EmployeeId == employee.Id)
                .OrderByDescending(tr => tr.RequestDate)
                .ToListAsync();

            return requests.Select(tr => new TeleworkRequestDto
            {
                Id = tr.Id,
                EmployeeId = tr.EmployeeId,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                RequestDate = tr.RequestDate,
                TeleworkDate = tr.TeleworkDate,
                Reason = tr.Reason,
                Status = tr.Status,
                ManagerComment = tr.ManagerComment,
                ProcessedAt = tr.ProcessedAt,
                ProcessedByManagerId = tr.ProcessedByManagerId,
                ProcessedByManagerName = tr.ProcessedByManager != null 
                    ? $"{tr.ProcessedByManager.FirstName} {tr.ProcessedByManager.LastName}" 
                    : null
            }).ToList();
        }

        public async Task<List<TeleworkRequestListDto>> GetCompanyRequestsAsync(int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent voir les demandes de l'entreprise.");

            if (!manager.CompanyId.HasValue)
                throw new InvalidOperationException("Vous devez d'abord créer une entreprise.");

            var requests = await _context.TeleworkRequests
                .Include(tr => tr.Employee)
                    .ThenInclude(e => e.User)
                .Include(tr => tr.ProcessedByManager)
                .Where(tr => tr.Employee.CompanyId == manager.CompanyId)
                .OrderByDescending(tr => tr.RequestDate)
                .ToListAsync();

            return requests.Select(tr => new TeleworkRequestListDto
            {
                Id = tr.Id,
                EmployeeName = $"{tr.Employee.FirstName} {tr.Employee.LastName}",
                EmployeeEmail = tr.Employee.User.Email,
                RequestDate = tr.RequestDate,
                TeleworkDate = tr.TeleworkDate,
                Reason = tr.Reason,
                Status = tr.Status,
                ManagerComment = tr.ManagerComment,
                ProcessedAt = tr.ProcessedAt,
                ProcessedByManagerName = tr.ProcessedByManager != null 
                    ? $"{tr.ProcessedByManager.FirstName} {tr.ProcessedByManager.LastName}" 
                    : null
            }).ToList();
        }

        public async Task<TeleworkRequestDto> ProcessRequestAsync(int requestId, ProcessTeleworkRequestDto processDto, int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent traiter les demandes.");

            if (!manager.CompanyId.HasValue)
                throw new InvalidOperationException("Vous devez d'abord créer une entreprise.");

            // Vérifier que la demande existe et appartient à l'entreprise du manager
            var request = await _context.TeleworkRequests
                .Include(tr => tr.Employee)
                .Include(tr => tr.ProcessedByManager)
                .FirstOrDefaultAsync(tr => tr.Id == requestId && 
                                         tr.Employee.CompanyId == manager.CompanyId);

            if (request == null)
                throw new InvalidOperationException("Demande non trouvée ou vous n'êtes pas autorisé à la traiter.");

            if (request.Status != "Pending")
                throw new InvalidOperationException("Cette demande a déjà été traitée.");

            // Traiter la demande
            request.Status = processDto.Status;
            request.ManagerComment = processDto.ManagerComment;
            request.ProcessedAt = DateTime.UtcNow;
            request.ProcessedByManagerId = managerId;

            await _context.SaveChangesAsync();

            return new TeleworkRequestDto
            {
                Id = request.Id,
                EmployeeId = request.EmployeeId,
                EmployeeName = $"{request.Employee.FirstName} {request.Employee.LastName}",
                RequestDate = request.RequestDate,
                TeleworkDate = request.TeleworkDate,
                Reason = request.Reason,
                Status = request.Status,
                ManagerComment = request.ManagerComment,
                ProcessedAt = request.ProcessedAt,
                ProcessedByManagerId = request.ProcessedByManagerId,
                ProcessedByManagerName = $"{manager.FirstName} {manager.LastName}"
            };
        }

        public async Task<WeeklyPlanningDto> GetWeeklyPlanningAsync(int managerId, DateTime weekStart)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent voir le planning hebdomadaire.");

            if (!manager.CompanyId.HasValue)
                throw new InvalidOperationException("Vous devez d'abord créer une entreprise.");

            var weekEnd = weekStart.AddDays(6);

            var requests = await _context.TeleworkRequests
                .Include(tr => tr.Employee)
                .Include(tr => tr.ProcessedByManager)
                .Where(tr => tr.Employee.CompanyId == manager.CompanyId &&
                           tr.TeleworkDate.Date >= weekStart.Date &&
                           tr.TeleworkDate.Date <= weekEnd.Date)
                .OrderBy(tr => tr.TeleworkDate)
                .ThenBy(tr => tr.Employee.FirstName)
                .ToListAsync();

            return new WeeklyPlanningDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                Requests = requests.Select(tr => new TeleworkRequestDto
                {
                    Id = tr.Id,
                    EmployeeId = tr.EmployeeId,
                    EmployeeName = $"{tr.Employee.FirstName} {tr.Employee.LastName}",
                    RequestDate = tr.RequestDate,
                    TeleworkDate = tr.TeleworkDate,
                    Reason = tr.Reason,
                    Status = tr.Status,
                    ManagerComment = tr.ManagerComment,
                    ProcessedAt = tr.ProcessedAt,
                    ProcessedByManagerId = tr.ProcessedByManagerId,
                    ProcessedByManagerName = tr.ProcessedByManager != null 
                        ? $"{tr.ProcessedByManager.FirstName} {tr.ProcessedByManager.LastName}" 
                        : null
                }).ToList()
            };
        }
    }
} 