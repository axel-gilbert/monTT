using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.DTOs
{
    /// <summary>
    /// DTO pour afficher les détails d'un employé
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Identifiant unique de l'employé
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur associé
        /// </summary>
        /// <example>1</example>
        public int UserId { get; set; }

        /// <summary>
        /// Adresse email de l'employé
        /// </summary>
        /// <example>john.doe@entreprise.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'employé
        /// </summary>
        /// <example>John</example>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'employé
        /// </summary>
        /// <example>Doe</example>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Poste ou fonction de l'employé
        /// </summary>
        /// <example>Développeur Senior</example>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'employé
        /// </summary>
        /// <example>User</example>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant de l'entreprise (si assigné)
        /// </summary>
        /// <example>1</example>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Nom de l'entreprise (si assigné)
        /// </summary>
        /// <example>TechCorp Solutions</example>
        public string? CompanyName { get; set; }
    }

    /// <summary>
    /// DTO pour le profil d'un employé (utilisé dans les réponses d'authentification)
    /// </summary>
    public class EmployeeProfileDto
    {
        /// <summary>
        /// Identifiant unique de l'employé
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Prénom de l'employé
        /// </summary>
        /// <example>John</example>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'employé
        /// </summary>
        /// <example>Doe</example>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Poste ou fonction de l'employé
        /// </summary>
        /// <example>Développeur Senior</example>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant de l'entreprise (si assigné)
        /// </summary>
        /// <example>1</example>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Nom de l'entreprise (si assigné)
        /// </summary>
        /// <example>TechCorp Solutions</example>
        public string? CompanyName { get; set; }
    }

    /// <summary>
    /// DTO pour mettre à jour le profil d'un employé
    /// </summary>
    public class UpdateEmployeeDto
    {
        /// <summary>
        /// Prénom de l'employé
        /// </summary>
        /// <example>John</example>
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'employé
        /// </summary>
        /// <example>Doe</example>
        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(50, ErrorMessage = "Le nom ne peut pas dépasser 50 caractères")]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Poste ou fonction de l'employé
        /// </summary>
        /// <example>Développeur Senior</example>
        [Required(ErrorMessage = "Le poste est requis")]
        [StringLength(100, ErrorMessage = "Le poste ne peut pas dépasser 100 caractères")]
        public string Position { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour assigner un employé à une entreprise
    /// </summary>
    public class AssignEmployeeToCompanyDto
    {
        /// <summary>
        /// Identifiant de l'employé à assigner
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant de l'employé est requis")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// Identifiant de l'entreprise
        /// </summary>
        /// <example>1</example>
        [Required(ErrorMessage = "L'identifiant de l'entreprise est requis")]
        public int CompanyId { get; set; }
    }

    /// <summary>
    /// DTO pour afficher un employé dans une liste
    /// </summary>
    public class EmployeeListDto
    {
        /// <summary>
        /// Identifiant unique de l'employé
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Adresse email de l'employé
        /// </summary>
        /// <example>john.doe@entreprise.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'employé
        /// </summary>
        /// <example>John</example>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'employé
        /// </summary>
        /// <example>Doe</example>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Poste ou fonction de l'employé
        /// </summary>
        /// <example>Développeur Senior</example>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'employé
        /// </summary>
        /// <example>User</example>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Identifiant de l'entreprise (si assigné)
        /// </summary>
        /// <example>1</example>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Nom de l'entreprise (si assigné)
        /// </summary>
        /// <example>TechCorp Solutions</example>
        public string? CompanyName { get; set; }

        /// <summary>
        /// Indique si l'employé est assigné à une entreprise
        /// </summary>
        /// <example>true</example>
        public bool IsAssignedToCompany { get; set; }
    }
} 