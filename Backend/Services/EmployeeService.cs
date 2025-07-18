using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeleworkManagementAPI.Data;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Models;

namespace TeleworkManagementAPI.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetProfileAsync(int userId);
        Task<EmployeeDto> UpdateProfileAsync(int userId, UpdateEmployeeDto updateDto);
        Task<List<EmployeeListDto>> GetAllEmployeesAsync(int managerId);
        Task<EmployeeDto> AssignToCompanyAsync(AssignEmployeeToCompanyDto assignDto, int managerId);
        Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeDto> GetProfileAsync(int userId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                throw new InvalidOperationException("Employé non trouvé.");

            return new EmployeeDto
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Email = employee.User.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                Role = employee.User.Role,
                CompanyId = employee.CompanyId,
                CompanyName = employee.Company?.Name,
                CreatedAt = employee.User.CreatedAt
            };
        }

        public async Task<EmployeeDto> UpdateProfileAsync(int userId, UpdateEmployeeDto updateDto)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null)
                throw new InvalidOperationException("Employé non trouvé.");

            employee.FirstName = updateDto.FirstName;
            employee.LastName = updateDto.LastName;
            employee.Position = updateDto.Position;

            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Email = employee.User.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                Role = employee.User.Role,
                CompanyId = employee.CompanyId,
                CompanyName = employee.Company?.Name,
                CreatedAt = employee.User.CreatedAt
            };
        }

        public async Task<List<EmployeeListDto>> GetAllEmployeesAsync(int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent voir la liste des employés.");

            var employees = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Company)
                .Where(e => e.CompanyId == manager.CompanyId || e.CompanyId == null)
                .ToListAsync();

            return employees.Select(e => new EmployeeListDto
            {
                Id = e.Id,
                Email = e.User.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Position = e.Position,
                Role = e.User.Role,
                CompanyId = e.CompanyId,
                CompanyName = e.Company?.Name,
                IsAssignedToCompany = e.CompanyId.HasValue
            }).ToList();
        }

        public async Task<EmployeeDto> AssignToCompanyAsync(AssignEmployeeToCompanyDto assignDto, int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent assigner des employés.");

            // Vérifier que l'entreprise appartient au manager
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == assignDto.CompanyId && c.ManagerId == managerId);

            if (company == null)
                throw new InvalidOperationException("Entreprise non trouvée ou vous n'êtes pas autorisé à y assigner des employés.");

            // Vérifier que l'employé existe
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == assignDto.EmployeeId);

            if (employee == null)
                throw new InvalidOperationException("Employé non trouvé.");

            // Assigner l'employé à l'entreprise
            employee.CompanyId = assignDto.CompanyId;
            await _context.SaveChangesAsync();

            return new EmployeeDto
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Email = employee.User.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                Role = employee.User.Role,
                CompanyId = employee.CompanyId,
                CompanyName = employee.Company?.Name,
                CreatedAt = employee.User.CreatedAt
            };
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(e => e.User)
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                throw new InvalidOperationException("Employé non trouvé.");

            return new EmployeeDto
            {
                Id = employee.Id,
                UserId = employee.UserId,
                Email = employee.User.Email,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Position = employee.Position,
                Role = employee.User.Role,
                CompanyId = employee.CompanyId,
                CompanyName = employee.Company?.Name,
                CreatedAt = employee.User.CreatedAt
            };
        }
    }
} 