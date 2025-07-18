using Microsoft.EntityFrameworkCore;
using TeleworkManagementAPI.Data;
using TeleworkManagementAPI.Models;

namespace TeleworkManagementAPI.Services
{
    public interface ISeedDataService
    {
        Task SeedDataAsync();
    }

    public class SeedDataService : ISeedDataService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public SeedDataService(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task SeedDataAsync()
        {
            // Vérifier si des données existent déjà
            if (await _context.Users.AnyAsync())
            {
                return; // Les données existent déjà
            }

            // Créer un manager de test
            var managerRegisterDto = new DTOs.RegisterDto
            {
                Email = "manager@test.com",
                Password = "password123",
                FirstName = "Jean",
                LastName = "Dupont",
                Position = "Directeur Général",
                Role = "Manager"
            };

            var managerResult = await _authService.RegisterAsync(managerRegisterDto);
            var managerUser = await _context.Users.FirstAsync(u => u.Email == "manager@test.com");
            var managerEmployee = await _context.Employees.FirstAsync(e => e.UserId == managerUser.Id);

            // Créer une entreprise de test
            var company = new Company
            {
                Name = "TechCorp Solutions",
                ManagerId = managerEmployee.Id
            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Assigner le manager à l'entreprise
            managerEmployee.CompanyId = company.Id;
            await _context.SaveChangesAsync();

            // Créer des employés de test
            var employees = new[]
            {
                new { Email = "marie.martin@test.com", FirstName = "Marie", LastName = "Martin", Position = "Développeur Senior" },
                new { Email = "pierre.durand@test.com", FirstName = "Pierre", LastName = "Durand", Position = "Chef de Projet" },
                new { Email = "sophie.leroy@test.com", FirstName = "Sophie", LastName = "Leroy", Position = "Designer UX/UI" },
                new { Email = "thomas.moreau@test.com", FirstName = "Thomas", LastName = "Moreau", Position = "Développeur Full-Stack" },
                new { Email = "lucie.petit@test.com", FirstName = "Lucie", LastName = "Petit", Position = "Product Owner" }
            };

            foreach (var emp in employees)
            {
                var registerDto = new DTOs.RegisterDto
                {
                    Email = emp.Email,
                    Password = "password123",
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Position = emp.Position,
                    Role = "User"
                };

                await _authService.RegisterAsync(registerDto);
                var user = await _context.Users.FirstAsync(u => u.Email == emp.Email);
                var employee = await _context.Employees.FirstAsync(e => e.UserId == user.Id);
                
                // Assigner à l'entreprise
                employee.CompanyId = company.Id;
            }
            await _context.SaveChangesAsync();

            // Créer des demandes de télétravail de test
            var teleworkRequests = new[]
            {
                new { EmployeeEmail = "marie.martin@test.com", Date = DateTime.Today.AddDays(2), Reason = "Travail sur projet urgent nécessitant de la concentration", Status = "Pending" },
                new { EmployeeEmail = "pierre.durand@test.com", Date = DateTime.Today.AddDays(3), Reason = "Réunion client à distance", Status = "Approved" },
                new { EmployeeEmail = "sophie.leroy@test.com", Date = DateTime.Today.AddDays(1), Reason = "Création de maquettes pour nouveau projet", Status = "Pending" },
                new { EmployeeEmail = "thomas.moreau@test.com", Date = DateTime.Today.AddDays(4), Reason = "Développement backend en cours", Status = "Approved" },
                new { EmployeeEmail = "lucie.petit@test.com", Date = DateTime.Today.AddDays(5), Reason = "Planning sprint et rédaction user stories", Status = "Rejected" },
                new { EmployeeEmail = "marie.martin@test.com", Date = DateTime.Today.AddDays(7), Reason = "Code review et optimisation", Status = "Pending" },
                new { EmployeeEmail = "pierre.durand@test.com", Date = DateTime.Today.AddDays(8), Reason = "Présentation projet à l'équipe", Status = "Approved" }
            };

            foreach (var req in teleworkRequests)
            {
                var user = await _context.Users.FirstAsync(u => u.Email == req.EmployeeEmail);
                var employee = await _context.Employees.FirstAsync(e => e.UserId == user.Id);

                var teleworkRequest = new TeleworkRequest
                {
                    EmployeeId = employee.Id,
                    RequestDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 7)),
                    TeleworkDate = req.Date,
                    Reason = req.Reason,
                    Status = req.Status
                };

                // Si la demande est traitée, ajouter les détails
                if (req.Status != "Pending")
                {
                    teleworkRequest.ProcessedAt = DateTime.UtcNow.AddDays(-1);
                    teleworkRequest.ProcessedByManagerId = managerEmployee.Id;
                    teleworkRequest.ManagerComment = req.Status == "Approved" 
                        ? "Demande approuvée. Bon travail !" 
                        : "Demande rejetée. Présence requise au bureau pour ce jour.";
                }

                _context.TeleworkRequests.Add(teleworkRequest);
            }
            await _context.SaveChangesAsync();
        }
    }
} 