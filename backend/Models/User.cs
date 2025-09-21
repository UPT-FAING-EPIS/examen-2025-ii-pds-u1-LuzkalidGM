using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Api.Models
{
    /// <summary>
    /// Entidad que representa un usuario del sistema
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre de usuario único
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Hash de la contraseña del usuario
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario en el sistema
        /// </summary>
        [Required]
        public UserRole Role { get; set; }

        /// <summary>
        /// Fecha de creación del usuario
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        /// <summary>
        /// Proyectos creados por el usuario
        /// </summary>
        public virtual ICollection<Project> CreatedProjects { get; set; } = new List<Project>();

        /// <summary>
        /// Proyectos asignados al usuario
        /// </summary>
        public virtual ICollection<Project> AssignedProjects { get; set; } = new List<Project>();

        /// <summary>
        /// Tareas asignadas al usuario
        /// </summary>
        public virtual ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();

        /// <summary>
        /// Comentarios creados por el usuario
        /// </summary>
        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }

    /// <summary>
    /// Enumeración de roles de usuario
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Usuario administrador del sistema
        /// </summary>
        Admin = 1,

        /// <summary>
        /// Gestor de proyectos
        /// </summary>
        ProjectManager = 2,

        /// <summary>
        /// Miembro del equipo
        /// </summary>
        TeamMember = 3
    }
}