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
        /// Identificador único del usuario (GUID para Azure SQL)
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Correo electrónico del usuario (usado como username)
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de usuario (alias para Email)
        /// </summary>
        [NotMapped]
        public string Username => Email;

        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Apellido del usuario
        /// </summary>
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Hash de la contraseña del usuario
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Rol del usuario en el sistema (Admin, ProjectManager, User)
        /// </summary>
        [Required]
        public UserRole Role { get; set; } = UserRole.TeamMember;

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de creación del usuario
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Nombre completo calculado
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        /// <summary>
        /// Proyectos donde es propietario
        /// </summary>
        public virtual ICollection<Project> OwnedProjects { get; set; } = new List<Project>();

        /// <summary>
        /// Tareas asignadas al usuario
        /// </summary>
        public virtual ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();

        /// <summary>
        /// Tareas creadas por el usuario
        /// </summary>
        public virtual ICollection<ProjectTask> CreatedTasks { get; set; } = new List<ProjectTask>();

        /// <summary>
        /// Comentarios del usuario
        /// </summary>
        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

        /// <summary>
        /// Membresías en proyectos
        /// </summary>
        public virtual ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
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