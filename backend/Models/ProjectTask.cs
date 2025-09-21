using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Api.Models
{
    /// <summary>
    /// Entidad que representa una tarea dentro de un proyecto
    /// </summary>
    public class ProjectTask
    {
        /// <summary>
        /// Identificador único de la tarea
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Título de la tarea
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descripción detallada de la tarea
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Estado actual de la tarea
        /// </summary>
        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        /// <summary>
        /// Prioridad de la tarea
        /// </summary>
        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        /// <summary>
        /// Fecha de vencimiento de la tarea
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// ID del proyecto al que pertenece la tarea
        /// </summary>
        [Required]
        public int ProjectId { get; set; }

        /// <summary>
        /// ID del usuario asignado a la tarea
        /// </summary>
        public int? AssignedToId { get; set; }

        /// <summary>
        /// ID del usuario que creó la tarea
        /// </summary>
        [Required]
        public int CreatedById { get; set; }

        /// <summary>
        /// Estimación de horas para completar la tarea
        /// </summary>
        public decimal? EstimatedHours { get; set; }

        /// <summary>
        /// Horas trabajadas en la tarea
        /// </summary>
        public decimal ActualHours { get; set; } = 0;

        /// <summary>
        /// Fecha de creación de la tarea
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Proyecto al que pertenece la tarea
        /// </summary>
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// Usuario asignado a la tarea
        /// </summary>
        [ForeignKey("AssignedToId")]
        public virtual User? AssignedTo { get; set; }

        /// <summary>
        /// Usuario que creó la tarea
        /// </summary>
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;

        /// <summary>
        /// Comentarios de la tarea
        /// </summary>
        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    }

    /// <summary>
    /// Enumeración de estados de tarea
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// Tarea pendiente de iniciar
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Tarea en progreso
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Tarea completada
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Tarea bloqueada
        /// </summary>
        Blocked = 4
    }

    /// <summary>
    /// Enumeración de prioridades de tarea
    /// </summary>
    public enum TaskPriority
    {
        /// <summary>
        /// Prioridad baja
        /// </summary>
        Low = 1,

        /// <summary>
        /// Prioridad media
        /// </summary>
        Medium = 2,

        /// <summary>
        /// Prioridad alta
        /// </summary>
        High = 3,

        /// <summary>
        /// Prioridad crítica
        /// </summary>
        Critical = 4
    }
}