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
        /// Identificador único del proyecto
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada del proyecto
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización prevista del proyecto
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Estado actual del proyecto
        /// </summary>
        [Required]
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;

        /// <summary>
        /// ID del usuario creador del proyecto
        /// </summary>
        [Required]
        public int CreatedById { get; set; }

        /// <summary>
        /// ID del usuario responsable del proyecto
        /// </summary>
        [Required]
        public int ResponsibleId { get; set; }

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
        /// Usuario que creó el proyecto
        /// </summary>
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;

        /// <summary>
        /// Usuario responsable del proyecto
        /// </summary>
        [ForeignKey("ResponsibleId")]
        public virtual User Responsible { get; set; } = null!;

        /// <summary>
        /// Tareas asociadas al proyecto
        /// </summary>
        public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
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