using Microsoft.EntityFrameworkCore;
using TeleworkManagementAPI.Data;
using TeleworkManagementAPI.DTOs;
using TeleworkManagementAPI.Models;

namespace TeleworkManagementAPI.Services
{
    public interface ICompanyService
    {
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto, int managerId);
        Task<CompanyDto> GetMyCompanyAsync(int managerId);
        Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyDto updateDto, int managerId);
        Task<CompanyWithEmployeesDto> GetCompanyWithEmployeesAsync(int managerId);
    }

    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createDto, int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent créer des entreprises.");

            // Vérifier que le manager n'a pas déjà une entreprise
            var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.ManagerId == managerId);

            if (existingCompany != null)
                throw new InvalidOperationException("Vous avez déjà une entreprise. Un manager ne peut gérer qu'une seule entreprise.");

            // Créer l'entreprise
            var company = new Company
            {
                Name = createDto.Name,
                ManagerId = managerId
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Assigner le manager à l'entreprise
            manager.CompanyId = company.Id;
            await _context.SaveChangesAsync();

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                ManagerId = company.ManagerId,
                ManagerName = $"{manager.FirstName} {manager.LastName}",
                EmployeeCount = 1 // Le manager lui-même
            };
        }

        public async Task<CompanyDto> GetMyCompanyAsync(int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent accéder aux informations de leur entreprise.");

            var company = await _context.Companies
                .Include(c => c.Manager)
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.ManagerId == managerId);

            if (company == null)
                throw new InvalidOperationException("Aucune entreprise trouvée pour ce manager.");

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                ManagerId = company.ManagerId,
                ManagerName = $"{company.Manager.FirstName} {company.Manager.LastName}",
                EmployeeCount = company.Employees.Count
            };
        }

        public async Task<CompanyDto> UpdateCompanyAsync(UpdateCompanyDto updateDto, int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent modifier leur entreprise.");

            var company = await _context.Companies
                .Include(c => c.Manager)
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(c => c.ManagerId == managerId);

            if (company == null)
                throw new InvalidOperationException("Aucune entreprise trouvée pour ce manager.");

            company.Name = updateDto.Name;
            await _context.SaveChangesAsync();

            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                ManagerId = company.ManagerId,
                ManagerName = $"{company.Manager.FirstName} {company.Manager.LastName}",
                EmployeeCount = company.Employees.Count
            };
        }

        public async Task<CompanyWithEmployeesDto> GetCompanyWithEmployeesAsync(int managerId)
        {
            // Vérifier que l'utilisateur est un manager
            var manager = await _context.Employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == managerId && e.User.Role == "Manager");

            if (manager == null)
                throw new InvalidOperationException("Accès refusé. Seuls les managers peuvent accéder aux informations de leur entreprise.");

            var company = await _context.Companies
                .Include(c => c.Manager)
                .Include(c => c.Employees)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(c => c.ManagerId == managerId);

            if (company == null)
                throw new InvalidOperationException("Aucune entreprise trouvée pour ce manager.");

            return new CompanyWithEmployeesDto
            {
                Id = company.Id,
                Name = company.Name,
                ManagerId = company.ManagerId,
                ManagerName = $"{company.Manager.FirstName} {company.Manager.LastName}",
                Employees = company.Employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    Email = e.User.Email,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Position = e.Position,
                    Role = e.User.Role,
                    CompanyId = e.CompanyId,
                    CompanyName = company.Name
                }).ToList()
            };
        }
    }
} 