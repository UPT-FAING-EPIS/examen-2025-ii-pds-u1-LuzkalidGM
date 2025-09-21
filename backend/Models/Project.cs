using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Api.Models
{
    /// <summary>
    /// Entidad que representa un proyecto en el sistema
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Identificador único del proyecto (GUID para Azure SQL)
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada del proyecto
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización prevista del proyecto
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Estado actual del proyecto
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Planning";

        /// <summary>
        /// Prioridad del proyecto
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// ID del usuario propietario del proyecto
        /// </summary>
        [Required]
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Fecha de creación del proyecto
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Usuario propietario del proyecto
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public virtual User Owner { get; set; } = null!;

        /// <summary>
        /// Tareas del proyecto
        /// </summary>
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        /// <summary>
        /// Miembros del proyecto
        /// </summary>
        public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }

    /// <summary>
    /// Enumeración de estados de proyecto
    /// </summary>
    public enum ProjectStatus
    {
        /// <summary>
        /// Proyecto en fase de planificación
        /// </summary>
        Planning = 1,

        /// <summary>
        /// Proyecto en progreso
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Proyecto completado
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Proyecto en pausa
        /// </summary>
        OnHold = 4,

        /// <summary>
        /// Proyecto cancelado
        /// </summary>
        Cancelled = 5
    }
}