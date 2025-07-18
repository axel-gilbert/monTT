using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        public int? CompanyId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;
        
        // Navigation properties
        public User User { get; set; } = null!;
        public Company? Company { get; set; }
        public ICollection<TeleworkRequest> TeleworkRequests { get; set; } = new List<TeleworkRequest>();
        public ICollection<TeleworkRequest> ProcessedRequests { get; set; } = new List<TeleworkRequest>();
    }
} 