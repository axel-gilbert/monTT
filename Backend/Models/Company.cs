using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.Models
{
    public class Company
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public int ManagerId { get; set; }
        
        // Navigation properties
        public Employee Manager { get; set; } = null!;
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
} 