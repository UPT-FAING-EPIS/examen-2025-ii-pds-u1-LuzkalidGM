using System.ComponentModel.DataAnnotations;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.DTOs
{
    /// <summary>
    /// DTO para la creación de una tarea
    /// </summary>
    public class CreateTaskDto
    {
        /// <summary>
        /// Título de la tarea
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la tarea
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// ID del proyecto al que pertenece la tarea
        /// </summary>
        [Required]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// ID del usuario asignado a la tarea
        /// </summary>
        public Guid? AssignedToId { get; set; }

        /// <summary>
        /// Prioridad de la tarea
        /// </summary>
        [Required]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Estimación de horas
        /// </summary>
        [Range(0, 999.99)]
        public decimal? EstimatedHours { get; set; }
    }

    /// <summary>
    /// DTO para la actualización de una tarea
    /// </summary>
    public class UpdateTaskDto
    {
        /// <summary>
        /// Título de la tarea
        /// </summary>
        [StringLength(200, MinimumLength = 3)]
        public string? Title { get; set; }

        /// <summary>
        /// Descripción de la tarea
        /// </summary>
        [StringLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Estado de la tarea
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Prioridad de la tarea
        /// </summary>
        public string? Priority { get; set; }

        /// <summary>
        /// ID del usuario asignado a la tarea
        /// </summary>
        public Guid? AssignedToId { get; set; }

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Estimación de horas
        /// </summary>
        [Range(0, 999.99)]
        public decimal? EstimatedHours { get; set; }

        /// <summary>
        /// Horas trabajadas
        /// </summary>
        [Range(0, 999.99)]
        public decimal? ActualHours { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de una tarea
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// ID de la tarea
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título de la tarea
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descripción de la tarea
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Estado de la tarea
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Prioridad de la tarea
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// ID del proyecto
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// ID del usuario asignado
        /// </summary>
        public Guid? AssignedToId { get; set; }

        /// <summary>
        /// Usuario asignado
        /// </summary>
        public UserDto? AssignedTo { get; set; }

        /// <summary>
        /// Proyecto al que pertenece la tarea
        /// </summary>
        public ProjectSummaryDto? Project { get; set; }

        /// <summary>
        /// Usuario que creó la tarea
        /// </summary>
        public UserSummaryDto? CreatedBy { get; set; }

        /// <summary>
        /// Comentarios de la tarea
        /// </summary>
        public List<TaskCommentDto>? Comments { get; set; }

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Estimación de horas
        /// </summary>
        public decimal? EstimatedHours { get; set; }

        /// <summary>
        /// Horas trabajadas
        /// </summary>
        public decimal? ActualHours { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO de resumen para una tarea (usado en listas de proyectos)
    /// </summary>
    public class TaskSummaryDto
    {
        /// <summary>
        /// ID de la tarea
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Título de la tarea
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Estado de la tarea
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Prioridad de la tarea
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Usuario asignado
        /// </summary>
        public UserSummaryDto? AssignedTo { get; set; }

        /// <summary>
        /// Estimación de horas
        /// </summary>
        public decimal? EstimatedHours { get; set; }

        /// <summary>
        /// Horas trabajadas
        /// </summary>
        public decimal? ActualHours { get; set; }
    }
}