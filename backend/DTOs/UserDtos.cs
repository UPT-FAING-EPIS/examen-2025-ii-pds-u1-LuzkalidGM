using System.ComponentModel.DataAnnotations;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.DTOs
{
    /// <summary>
    /// DTO para el registro de usuario
    /// </summary>
    public class RegisterUserDto
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario
        /// </summary>
        [Required]
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// DTO para el inicio de sesión
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Nombre de usuario o email
        /// </summary>
        [Required]
        public string UsernameOrEmail { get; set; } = string.Empty;

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para la respuesta de autenticación
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Información del usuario
        /// </summary>
        public UserDto User { get; set; } = null!;

        /// <summary>
        /// Fecha de expiración del token
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO para la actualización de usuario
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Correo electrónico
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario
        /// </summary>
        [Required]
        public UserRole Role { get; set; }

        /// <summary>
        /// Estado activo del usuario
        /// </summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de usuario
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// Estado activo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO resumido de usuario
    /// </summary>
    public class UserSummaryDto
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario
        /// </summary>
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// DTO para cambio de contraseña
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Contraseña actual
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// Nueva contraseña
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;
    }
}