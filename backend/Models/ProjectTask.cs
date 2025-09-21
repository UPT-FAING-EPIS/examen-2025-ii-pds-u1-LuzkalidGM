using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations.Schema;using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Api.Models

{

    /// <summary>

    /// Entidad que representa una tarea dentro de un proyectonamespace ProjectManagement.Api.Modelsnamespace ProjectManagement.Api.Models

    /// </summary>

    public class ProjectTask{{

    {

        /// <summary>    /// <summary>    /// <summary>

        /// Identificador único de la tarea

        /// </summary>    /// Entidad que representa una tarea dentro de un proyecto    /// Entidad que representa una tarea dentro de un proyecto

        [Key]

        public Guid Id { get; set; } = Guid.NewGuid();    /// </summary>    /// </summary>



        /// <summary>    public class ProjectTask    public class ProjectTask

        /// Título de la tarea

        /// </summary>    {    {

        [Required]

        [StringLength(100)]        /// <summary>        /// <summary>

        public string Title { get; set; } = string.Empty;

        /// Identificador único de la tarea (GUID para Azure SQL)        /// Identificador único de la tarea (GUID para Azure SQL)

        /// <summary>

        /// Descripción detallada de la tarea        /// </summary>        /// </summary>

        /// </summary>

        [StringLength(1000)]        [Key]        [Key]

        public string Description { get; set; } = string.Empty;

        public Guid Id { get; set; } = Guid.NewGuid();        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>

        /// Estado actual de la tarea

        /// </summary>

        [Required]        /// <summary>        /// <summary>

        [StringLength(20)]

        public string Status { get; set; } = "Pending";        /// Título de la tarea        /// Título de la tarea



        /// <summary>        /// </summary>        /// </summary>

        /// Prioridad de la tarea

        /// </summary>        [Required]        [Required]

        [Required]

        [StringLength(10)]        [StringLength(100)]        [StringLength(100)]

        public string Priority { get; set; } = "Medium";

        public string Title { get; set; } = string.Empty;        public string Title { get; set; } = string.Empty;

        /// <summary>

        /// Fecha de inicio de la tarea

        /// </summary>

        public DateTime? StartDate { get; set; }        /// <summary>        /// <summary>



        /// <summary>        /// Descripción detallada de la tarea        /// Descripción detallada de la tarea

        /// Fecha de vencimiento de la tarea

        /// </summary>        /// </summary>        /// </summary>

        public DateTime? DueDate { get; set; }

        [StringLength(1000)]        [StringLength(1000)]

        /// <summary>

        /// Fecha de finalización de la tarea        public string Description { get; set; } = string.Empty;        public string Description { get; set; } = string.Empty;

        /// </summary>

        public DateTime? CompletedAt { get; set; }



        /// <summary>        /// <summary>        /// <summary>

        /// Horas estimadas para completar la tarea

        /// </summary>        /// Estado actual de la tarea (Pending, InProgress, Completed, Blocked)        /// Estado actual de la tarea

        [Column(TypeName = "decimal(5,2)")]

        public decimal? EstimatedHours { get; set; }        /// </summary>        /// </summary>



        /// <summary>        [Required]        [Required]

        /// Horas reales trabajadas en la tarea

        /// </summary>        [StringLength(20)]        [StringLength(20)]

        [Column(TypeName = "decimal(5,2)")]

        public decimal? ActualHours { get; set; }        public string Status { get; set; } = "Pending";        public string Status { get; set; } = "Pending";



        /// <summary>

        /// ID del proyecto al que pertenece la tarea

        /// </summary>        /// <summary>        /// <summary>

        [Required]

        public Guid ProjectId { get; set; }        /// Prioridad de la tarea (Low, Medium, High)        /// Prioridad de la tarea



        /// <summary>        /// </summary>        /// </summary>

        /// ID del usuario asignado a la tarea

        /// </summary>        [Required]        [Required]

        public Guid? AssignedToId { get; set; }

        [StringLength(10)]        [StringLength(10)]

        /// <summary>

        /// ID del usuario que creó la tarea        public string Priority { get; set; } = "Medium";        public string Priority { get; set; } = "Medium";

        /// </summary>

        [Required]

        public Guid CreatedById { get; set; }

        /// <summary>        /// <summary>

        /// <summary>

        /// Fecha de creación de la tarea        /// Fecha de inicio de la tarea        /// Fecha de inicio de la tarea

        /// </summary>

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;        /// </summary>        /// </summary>



        /// <summary>        public DateTime? StartDate { get; set; }        public DateTime? StartDate { get; set; }

        /// Fecha de última actualización

        /// </summary>

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>        /// <summary>

        // Navigation properties

        /// <summary>        /// Fecha de vencimiento de la tarea        /// Fecha de vencimiento de la tarea

        /// Proyecto al que pertenece la tarea

        /// </summary>        /// </summary>        /// </summary>

        [ForeignKey(nameof(ProjectId))]

        public virtual Project Project { get; set; } = null!;        public DateTime? DueDate { get; set; }        public DateTime? DueDate { get; set; }



        /// <summary>

        /// Usuario asignado a la tarea

        /// </summary>        /// <summary>        /// <summary>

        [ForeignKey(nameof(AssignedToId))]

        public virtual User? AssignedTo { get; set; }        /// Fecha de finalización de la tarea        /// Fecha de finalización de la tarea



        /// <summary>        /// </summary>        /// </summary>

        /// Usuario que creó la tarea

        /// </summary>        public DateTime? CompletedAt { get; set; }        public DateTime? CompletedAt { get; set; }

        [ForeignKey(nameof(CreatedById))]

        public virtual User CreatedBy { get; set; } = null!;



        /// <summary>        /// <summary>        /// <summary>

        /// Comentarios de la tarea

        /// </summary>        /// Horas estimadas para completar la tarea        /// Horas estimadas para completar la tarea

        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();

    }        /// </summary>        /// </summary>

}
        [Column(TypeName = "decimal(5,2)")]        [Column(TypeName = "decimal(5,2)")]

        public decimal? EstimatedHours { get; set; }        public decimal? EstimatedHours { get; set; }



        /// <summary>        /// <summary>

        /// Horas reales trabajadas en la tarea        /// Horas reales trabajadas en la tarea

        /// </summary>        /// </summary>

        [Column(TypeName = "decimal(5,2)")]        [Column(TypeName = "decimal(5,2)")]

        public decimal? ActualHours { get; set; }        public decimal? ActualHours { get; set; }



        /// <summary>        /// <summary>

        /// ID del proyecto al que pertenece la tarea        /// ID del proyecto al que pertenece la tarea

        /// </summary>        /// </summary>

        [Required]        [Required]

        public Guid ProjectId { get; set; }        public int ProjectId { get; set; }



        /// <summary>        /// <summary>

        /// ID del usuario asignado a la tarea        /// ID del usuario asignado a la tarea

        /// </summary>        /// </summary>

        public Guid? AssignedToId { get; set; }        public int? AssignedToId { get; set; }



        /// <summary>        /// <summary>

        /// ID del usuario que creó la tarea        /// ID del usuario que creó la tarea

        /// </summary>        /// </summary>

        [Required]        [Required]

        public Guid CreatedById { get; set; }        public int CreatedById { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de creación de la tarea        /// Estimación de horas para completar la tarea

        /// </summary>        /// </summary>

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;        public decimal? EstimatedHours { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de última actualización        /// Horas trabajadas en la tarea

        /// </summary>        /// </summary>

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;        public decimal ActualHours { get; set; } = 0;



        // Navigation properties        /// <summary>

        /// <summary>        /// Fecha de creación de la tarea

        /// Proyecto al que pertenece la tarea        /// </summary>

        /// </summary>        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(ProjectId))]

        public virtual Project Project { get; set; } = null!;        /// <summary>

        /// Fecha de última actualización

        /// <summary>        /// </summary>

        /// Usuario asignado a la tarea        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// </summary>

        [ForeignKey(nameof(AssignedToId))]        // Navigation properties

        public virtual User? AssignedTo { get; set; }        /// <summary>

        /// Proyecto al que pertenece la tarea

        /// <summary>        /// </summary>

        /// Usuario que creó la tarea        [ForeignKey("ProjectId")]

        /// </summary>        public virtual Project Project { get; set; } = null!;

        [ForeignKey(nameof(CreatedById))]

        public virtual User CreatedBy { get; set; } = null!;        /// <summary>

        /// Usuario asignado a la tarea

        /// <summary>        /// </summary>

        /// Comentarios de la tarea        [ForeignKey("AssignedToId")]

        /// </summary>        public virtual User? AssignedTo { get; set; }

        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();

    }        /// <summary>

}        /// Usuario que creó la tarea
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