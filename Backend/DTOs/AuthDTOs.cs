using System.ComponentModel.DataAnnotations;

namespace TeleworkManagementAPI.DTOs
{
    /// <summary>
    /// DTO pour l'inscription d'un nouvel utilisateur
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Adresse email de l'utilisateur (doit être unique)
        /// </summary>
        /// <example>john.doe@entreprise.com</example>
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe (minimum 6 caractères)
        /// </summary>
        /// <example>MotDePasse123!</example>
        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Prénom de l'employé
        /// </summary>
        /// <example>Jean</example>
        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(50, ErrorMessage = "Le prénom ne peut pas dépasser 50 caractères")]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Nom de famille de l'employé
        /// </summary>
        /// <example>Dupont</example>
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

        /// <summary>
        /// Rôle de l'utilisateur (User ou Manager)
        /// </summary>
        /// <example>User</example>
        [Required(ErrorMessage = "Le rôle est requis")]
        [RegularExpression("^(User|Manager)$", ErrorMessage = "Le rôle doit être 'User' ou 'Manager'")]
        public string Role { get; set; } = "User";
    }

    /// <summary>
    /// DTO pour la connexion d'un utilisateur
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Adresse email de l'utilisateur
        /// </summary>
        /// <example>john.doe@entreprise.com</example>
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Format d'email invalide")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Mot de passe de l'utilisateur
        /// </summary>
        /// <example>MotDePasse123!</example>
        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO pour le renouvellement du token
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// Token de renouvellement
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        [Required(ErrorMessage = "Le refresh token est requis")]
        public string RefreshToken { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO de réponse pour l'authentification
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT pour l'authentification
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c</example>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Token de renouvellement (pour les futures versions)
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// Date d'expiration du token
        /// </summary>
        /// <example>2024-12-31T23:59:59Z</example>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Profil de l'utilisateur connecté
        /// </summary>
        public UserProfileDto User { get; set; } = new UserProfileDto();
    }

    /// <summary>
    /// DTO pour le profil utilisateur
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Adresse email de l'utilisateur
        /// </summary>
        /// <example>john.doe@entreprise.com</example>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Rôle de l'utilisateur
        /// </summary>
        /// <example>User</example>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du compte
        /// </summary>
        /// <example>2024-01-15T10:30:00Z</example>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Profil de l'employé associé (si existant)
        /// </summary>
        public EmployeeProfileDto? Employee { get; set; }
    }
} 