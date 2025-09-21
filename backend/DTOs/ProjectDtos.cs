using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Api.DTOs
{
    /// <summary>
    /// DTO para la creación de un proyecto
    /// </summary>
    public class CreateProjectDto
    {
        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Prioridad del proyecto
        /// </summary>
        [StringLength(10)]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// ID del usuario responsable del proyecto
        /// </summary>
        public Guid? ResponsibleId { get; set; }
    }

    /// <summary>
    /// DTO para la actualización de un proyecto
    /// </summary>
    public class UpdateProjectDto
    {
        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        [StringLength(100, MinimumLength = 3)]
        public string? Name { get; set; }

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Estado del proyecto
        /// </summary>
        [StringLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// Prioridad del proyecto
        /// </summary>
        [StringLength(10)]
        public string? Priority { get; set; }

        /// <summary>
        /// ID del usuario responsable del proyecto
        /// </summary>
        public Guid? ResponsibleId { get; set; }
    }

    /// <summary>
    /// DTO completo de proyecto
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// Identificador único del proyecto
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Estado del proyecto
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Prioridad del proyecto
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// ID del propietario del proyecto
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Propietario del proyecto
        /// </summary>
        public UserDto? Owner { get; set; }

        /// <summary>
        /// Usuario que creó el proyecto (alias para Owner)
        /// </summary>
        public UserSummaryDto? CreatedBy { get; set; }

        /// <summary>
        /// Usuario responsable del proyecto (alias para Owner)
        /// </summary>
        public UserSummaryDto? Responsible { get; set; }

        /// <summary>
        /// Miembros del proyecto
        /// </summary>
        public List<ProjectMemberDto>? Members { get; set; }

        /// <summary>
        /// Tareas del proyecto (resumen)
        /// </summary>
        public List<TaskSummaryDto>? Tasks { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO para miembro de proyecto
    /// </summary>
    public class ProjectMemberDto
    {
        /// <summary>
        /// ID del miembro del proyecto
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID del proyecto
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// ID del usuario
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Rol en el proyecto
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de unión al proyecto
        /// </summary>
        public DateTime JoinedDate { get; set; }

        /// <summary>
        /// Información del usuario
        /// </summary>
        public UserDto? User { get; set; }
    }

    /// <summary>
    /// DTO resumido de proyecto para listados
    /// </summary>
    public class ProjectSummaryDto
    {
        /// <summary>
        /// Identificador único del proyecto
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Estado del proyecto
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Responsable del proyecto
        /// </summary>
        public string Responsible { get; set; } = string.Empty;

        /// <summary>
        /// Total de tareas del proyecto
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Tareas completadas del proyecto
        /// </summary>
        public int CompletedTasks { get; set; }

        /// <summary>
        /// Porcentaje de progreso del proyecto
        /// </summary>
        public double ProgressPercentage { get; set; }
    }
}