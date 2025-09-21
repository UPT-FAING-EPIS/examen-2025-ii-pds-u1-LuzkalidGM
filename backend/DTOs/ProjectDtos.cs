using System.ComponentModel.DataAnnotations;
using ProjectManagement.Api.Models;

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
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// ID del usuario responsable del proyecto
        /// </summary>
        [Required]
        public int ResponsibleId { get; set; }
    }

    /// <summary>
    /// DTO para la actualización de un proyecto
    /// </summary>
    public class UpdateProjectDto
    {
        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Estado del proyecto
        /// </summary>
        [Required]
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// ID del usuario responsable del proyecto
        /// </summary>
        [Required]
        public int ResponsibleId { get; set; }
    }

    /// <summary>
    /// DTO para la respuesta de un proyecto
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// ID del proyecto
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre del proyecto
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del proyecto
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del proyecto
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del proyecto
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Estado del proyecto
        /// </summary>
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Usuario que creó el proyecto
        /// </summary>
        public UserSummaryDto CreatedBy { get; set; } = null!;

        /// <summary>
        /// Usuario responsable del proyecto
        /// </summary>
        public UserSummaryDto Responsible { get; set; } = null!;

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Lista de tareas del proyecto
        /// </summary>
        public List<TaskSummaryDto> Tasks { get; set; } = new List<TaskSummaryDto>();
    }

    /// <summary>
    /// DTO resumido de proyecto para listas
    /// </summary>
    public class ProjectSummaryDto
    {
        /// <summary>
        /// ID del proyecto
        /// </summary>
        public int Id { get; set; }

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
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Fecha de inicio
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Usuario responsable
        /// </summary>
        public UserSummaryDto Responsible { get; set; } = null!;

        /// <summary>
        /// Número total de tareas
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Número de tareas completadas
        /// </summary>
        public int CompletedTasks { get; set; }

        /// <summary>
        /// Porcentaje de progreso
        /// </summary>
        public decimal ProgressPercentage { get; set; }
    }
}