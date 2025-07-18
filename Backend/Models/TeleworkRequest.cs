using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.Models
{
    public class TeleworkRequest
    {
        public int Id { get; set; }
        
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        public DateTime TeleworkDate { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
        
        [StringLength(500)]
        public string? ManagerComment { get; set; }
        
        public DateTime? ProcessedAt { get; set; }
        
        public int? ProcessedByManagerId { get; set; }
        
        // Navigation properties
        public Employee Employee { get; set; } = null!;
        public Employee? ProcessedByManager { get; set; }
    }
} 