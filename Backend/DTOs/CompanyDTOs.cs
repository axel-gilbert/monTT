using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public int EmployeeCount { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateCompanyDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }

    public class CompanyWithEmployeesDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public List<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();
    }
} 