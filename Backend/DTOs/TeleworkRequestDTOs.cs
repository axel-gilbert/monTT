using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.DTOs
{
    public class TeleworkRequestDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime TeleworkDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ManagerComment { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public int? ProcessedByManagerId { get; set; }
        public string? ProcessedByManagerName { get; set; }
    }

    public class CreateTeleworkRequestDto
    {
        [Required]
        public DateTime TeleworkDate { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class ProcessTeleworkRequestDto
    {
        [Required]
        public string Status { get; set; } = string.Empty; // "Approved" ou "Rejected"

        [StringLength(500)]
        public string? ManagerComment { get; set; }
    }

    public class TeleworkRequestListDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime TeleworkDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ManagerComment { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ProcessedByManagerName { get; set; }
    }

    public class WeeklyPlanningDto
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public List<TeleworkRequestDto> Requests { get; set; } = new List<TeleworkRequestDto>();
    }
} 