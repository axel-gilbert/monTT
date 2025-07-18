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
        Task<List<TeleworkRequestDto>> GetCompanyRequestsAsync(int managerId);
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
                RequestDate = request.RequestDate,
                TeleworkDate = request.TeleworkDate,
                Reason = request.Reason,
                Status = request.Status,
                ManagerComment = request.ManagerComment,
                ProcessedAt = request.ProcessedAt,
                Employee = new EmployeeListDto
                {
                    Id = employee.Id,
                    Email = employee.User.Email,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Position = employee.Position,
                    Role = employee.User.Role,
                    CompanyId = employee.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = employee.CompanyId.HasValue
                },
                ProcessedByManager = null
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
                    .ThenInclude(e => e.User)
                .Where(tr => tr.EmployeeId == employee.Id)
                .OrderByDescending(tr => tr.RequestDate)
                .ToListAsync();

            return requests.Select(tr => new TeleworkRequestDto
            {
                Id = tr.Id,
                RequestDate = tr.RequestDate,
                TeleworkDate = tr.TeleworkDate,
                Reason = tr.Reason,
                Status = tr.Status,
                ManagerComment = tr.ManagerComment,
                ProcessedAt = tr.ProcessedAt,
                Employee = new EmployeeListDto
                {
                    Id = employee.Id,
                    Email = employee.User.Email,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Position = employee.Position,
                    Role = employee.User.Role,
                    CompanyId = employee.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = employee.CompanyId.HasValue
                },
                ProcessedByManager = tr.ProcessedByManager != null ? new EmployeeListDto
                {
                    Id = tr.ProcessedByManager.Id,
                    Email = tr.ProcessedByManager.User.Email,
                    FirstName = tr.ProcessedByManager.FirstName,
                    LastName = tr.ProcessedByManager.LastName,
                    Position = tr.ProcessedByManager.Position,
                    Role = tr.ProcessedByManager.User.Role,
                    CompanyId = tr.ProcessedByManager.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = tr.ProcessedByManager.CompanyId.HasValue
                } : null
            }).ToList();
        }

        public async Task<List<TeleworkRequestDto>> GetCompanyRequestsAsync(int managerId)
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
                    .ThenInclude(e => e.User)
                .Where(tr => tr.Employee.CompanyId == manager.CompanyId)
                .OrderByDescending(tr => tr.RequestDate)
                .ToListAsync();

            return requests.Select(tr => new TeleworkRequestDto
            {
                Id = tr.Id,
                RequestDate = tr.RequestDate,
                TeleworkDate = tr.TeleworkDate,
                Reason = tr.Reason,
                Status = tr.Status,
                ManagerComment = tr.ManagerComment,
                ProcessedAt = tr.ProcessedAt,
                Employee = new EmployeeListDto
                {
                    Id = tr.Employee.Id,
                    Email = tr.Employee.User.Email,
                    FirstName = tr.Employee.FirstName,
                    LastName = tr.Employee.LastName,
                    Position = tr.Employee.Position,
                    Role = tr.Employee.User.Role,
                    CompanyId = tr.Employee.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = tr.Employee.CompanyId.HasValue
                },
                ProcessedByManager = tr.ProcessedByManager != null ? new EmployeeListDto
                {
                    Id = tr.ProcessedByManager.Id,
                    Email = tr.ProcessedByManager.User.Email,
                    FirstName = tr.ProcessedByManager.FirstName,
                    LastName = tr.ProcessedByManager.LastName,
                    Position = tr.ProcessedByManager.Position,
                    Role = tr.ProcessedByManager.User.Role,
                    CompanyId = tr.ProcessedByManager.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = tr.ProcessedByManager.CompanyId.HasValue
                } : null
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
                    .ThenInclude(e => e.User)
                .Include(tr => tr.ProcessedByManager)
                    .ThenInclude(e => e.User)
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
                RequestDate = request.RequestDate,
                TeleworkDate = request.TeleworkDate,
                Reason = request.Reason,
                Status = request.Status,
                ManagerComment = request.ManagerComment,
                ProcessedAt = request.ProcessedAt,
                Employee = new EmployeeListDto
                {
                    Id = request.Employee.Id,
                    Email = request.Employee.User.Email,
                    FirstName = request.Employee.FirstName,
                    LastName = request.Employee.LastName,
                    Position = request.Employee.Position,
                    Role = request.Employee.User.Role,
                    CompanyId = request.Employee.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = request.Employee.CompanyId.HasValue
                },
                ProcessedByManager = new EmployeeListDto
                {
                    Id = manager.Id,
                    Email = manager.User.Email,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    Position = manager.Position,
                    Role = manager.User.Role,
                    CompanyId = manager.CompanyId,
                    CompanyName = null,
                    IsAssignedToCompany = manager.CompanyId.HasValue
                }
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
                    .ThenInclude(e => e.User)
                .Include(tr => tr.ProcessedByManager)
                    .ThenInclude(e => e.User)
                .Where(tr => tr.Employee.CompanyId == manager.CompanyId &&
                           tr.TeleworkDate.Date >= weekStart.Date &&
                           tr.TeleworkDate.Date <= weekEnd.Date)
                .OrderBy(tr => tr.TeleworkDate)
                .ThenBy(tr => tr.Employee.FirstName)
                .ToListAsync();

            var dailyRequests = new List<DailyTeleworkDto>();
            var stats = new WeeklyStatsDto
            {
                TotalRequests = requests.Count,
                ApprovedRequests = requests.Count(r => r.Status == "Approved"),
                PendingRequests = requests.Count(r => r.Status == "Pending"),
                RejectedRequests = requests.Count(r => r.Status == "Rejected"),
                UniqueEmployees = requests.Select(r => r.EmployeeId).Distinct().Count()
            };

            if (stats.TotalRequests > 0)
            {
                stats.ApprovalRate = Math.Round((double)stats.ApprovedRequests / stats.TotalRequests * 100, 1);
            }

            // Grouper les demandes par jour
            for (int i = 0; i < 7; i++)
            {
                var currentDate = weekStart.AddDays(i);
                var dayRequests = requests.Where(r => r.TeleworkDate.Date == currentDate.Date).ToList();

                var dailyTelework = new DailyTeleworkDto
                {
                    Date = currentDate,
                    DayName = currentDate.ToString("dddd", new System.Globalization.CultureInfo("fr-FR")),
                    Requests = dayRequests.Select(tr => new TeleworkRequestDto
                    {
                        Id = tr.Id,
                        RequestDate = tr.RequestDate,
                        TeleworkDate = tr.TeleworkDate,
                        Reason = tr.Reason,
                        Status = tr.Status,
                        ManagerComment = tr.ManagerComment,
                        ProcessedAt = tr.ProcessedAt,
                        Employee = new EmployeeListDto
                        {
                            Id = tr.Employee.Id,
                            Email = tr.Employee.User.Email,
                            FirstName = tr.Employee.FirstName,
                            LastName = tr.Employee.LastName,
                            Position = tr.Employee.Position,
                            Role = tr.Employee.User.Role,
                            CompanyId = tr.Employee.CompanyId,
                            CompanyName = null,
                            IsAssignedToCompany = tr.Employee.CompanyId.HasValue
                        },
                        ProcessedByManager = tr.ProcessedByManager != null ? new EmployeeListDto
                        {
                            Id = tr.ProcessedByManager.Id,
                            Email = tr.ProcessedByManager.User.Email,
                            FirstName = tr.ProcessedByManager.FirstName,
                            LastName = tr.ProcessedByManager.LastName,
                            Position = tr.ProcessedByManager.Position,
                            Role = tr.ProcessedByManager.User.Role,
                            CompanyId = tr.ProcessedByManager.CompanyId,
                            CompanyName = null,
                            IsAssignedToCompany = tr.ProcessedByManager.CompanyId.HasValue
                        } : null
                    }).ToList(),
                    TotalRequests = dayRequests.Count,
                    ApprovedRequests = dayRequests.Count(r => r.Status == "Approved"),
                    PendingRequests = dayRequests.Count(r => r.Status == "Pending"),
                    RejectedRequests = dayRequests.Count(r => r.Status == "Rejected")
                };

                dailyRequests.Add(dailyTelework);
            }

            return new WeeklyPlanningDto
            {
                WeekStart = weekStart,
                WeekEnd = weekEnd,
                DailyRequests = dailyRequests,
                Stats = stats
            };
        }
    }
} 