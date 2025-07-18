using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.DTOs
{
    /// <summary>
    /// DTO pour créer une nouvelle demande de télétravail
    /// </summary>
    public class CreateTeleworkRequestDto
    {
        /// <summary>
        /// Date de la demande de télétravail (doit être dans le futur)
        /// </summary>
        /// <example>2024-02-15</example>
        [Required(ErrorMessage = "La date de télétravail est requise")]
        public DateTime TeleworkDate { get; set; }

        /// <summary>
        /// Raison de la demande de télétravail
        /// </summary>
        /// <example>Travail sur projet urgent nécessitant de la concentration</example>
        [Required(ErrorMessage = "La raison est requise")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "La raison doit contenir entre 10 et 500 caractères")]
        public string Reason { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour traiter une demande de télétravail (Manager uniquement)
    /// </summary>
    public class ProcessTeleworkRequestDto
    {
        /// <summary>
        /// Nouveau statut de la demande (Approved ou Rejected)
        /// </summary>
        /// <example>Approved</example>
        [Required(ErrorMessage = "Le statut est requis")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Le statut doit être 'Approved' ou 'Rejected'")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Commentaire du manager (optionnel pour approbation, recommandé pour rejet)
        /// </summary>
        /// <example>Demande approuvée. Bon travail sur ce projet !</example>
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères")]
        public string? ManagerComment { get; set; }
    }

    /// <summary>
    /// DTO pour afficher une demande de télétravail
    /// </summary>
    public class TeleworkRequestDto
    {
        /// <summary>
        /// Identifiant unique de la demande
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Date de création de la demande
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// Date de la demande de télétravail
        /// </summary>
        /// <example>2024-02-15</example>
        public DateTime TeleworkDate { get; set; }

        /// <summary>
        /// Raison de la demande
        /// </summary>
        /// <example>Travail sur projet urgent nécessitant de la concentration</example>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Statut actuel de la demande
        /// </summary>
        /// <example>Pending</example>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Commentaire du manager (si traité)
        /// </summary>
        /// <example>Demande approuvée. Bon travail sur ce projet !</example>
        public string? ManagerComment { get; set; }

        /// <summary>
        /// Date de traitement par le manager
        /// </summary>
        /// <example>2024-01-16T14:30:00Z</example>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Informations de l'employé qui a fait la demande
        /// </summary>
        public EmployeeListDto Employee { get; set; } = new EmployeeListDto();

        /// <summary>
        /// Informations du manager qui a traité la demande (si traité)
        /// </summary>
        public EmployeeListDto? ProcessedByManager { get; set; }
    }

    /// <summary>
    /// DTO pour le planning hebdomadaire des demandes de télétravail
    /// </summary>
    public class WeeklyPlanningDto
    {
        /// <summary>
        /// Date de début de la semaine
        /// </summary>
        /// <example>2024-02-12</example>
        public DateTime WeekStart { get; set; }

        /// <summary>
        /// Date de fin de la semaine
        /// </summary>
        /// <example>2024-02-18</example>
        public DateTime WeekEnd { get; set; }

        /// <summary>
        /// Demandes de télétravail groupées par jour
        /// </summary>
        public List<DailyTeleworkDto> DailyRequests { get; set; } = new List<DailyTeleworkDto>();

        /// <summary>
        /// Statistiques de la semaine
        /// </summary>
        public WeeklyStatsDto Stats { get; set; } = new WeeklyStatsDto();
    }

    /// <summary>
    /// DTO pour les demandes de télétravail d'un jour spécifique
    /// </summary>
    public class DailyTeleworkDto
    {
        /// <summary>
        /// Date du jour
        /// </summary>
        /// <example>2024-02-15</example>
        public DateTime Date { get; set; }

        /// <summary>
        /// Nom du jour de la semaine
        /// </summary>
        /// <example>Jeudi</example>
        public string DayName { get; set; } = string.Empty;

        /// <summary>
        /// Demandes de télétravail pour ce jour
        /// </summary>
        public List<TeleworkRequestDto> Requests { get; set; } = new List<TeleworkRequestDto>();

        /// <summary>
        /// Nombre total de demandes pour ce jour
        /// </summary>
        /// <example>3</example>
        public int TotalRequests { get; set; }

        /// <summary>
        /// Nombre de demandes approuvées
        /// </summary>
        /// <example>2</example>
        public int ApprovedRequests { get; set; }

        /// <summary>
        /// Nombre de demandes en attente
        /// </summary>
        /// <example>1</example>
        public int PendingRequests { get; set; }

        /// <summary>
        /// Nombre de demandes rejetées
        /// </summary>
        /// <example>0</example>
        public int RejectedRequests { get; set; }
    }

    /// <summary>
    /// DTO pour les statistiques hebdomadaires
    /// </summary>
    public class WeeklyStatsDto
    {
        /// <summary>
        /// Nombre total de demandes de la semaine
        /// </summary>
        /// <example>15</example>
        public int TotalRequests { get; set; }

        /// <summary>
        /// Nombre de demandes approuvées
        /// </summary>
        /// <example>12</example>
        public int ApprovedRequests { get; set; }

        /// <summary>
        /// Nombre de demandes en attente
        /// </summary>
        /// <example>2</example>
        public int PendingRequests { get; set; }

        /// <summary>
        /// Nombre de demandes rejetées
        /// </summary>
        /// <example>1</example>
        public int RejectedRequests { get; set; }

        /// <summary>
        /// Pourcentage d'approbation
        /// </summary>
        /// <example>80.0</example>
        public double ApprovalRate { get; set; }

        /// <summary>
        /// Nombre d'employés ayant fait des demandes
        /// </summary>
        /// <example>8</example>
        public int UniqueEmployees { get; set; }
    }
} 