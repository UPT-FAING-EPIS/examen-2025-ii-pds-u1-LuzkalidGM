using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations;



namespace ProjectManagement.Api.DTOsnamespace ProjectManagement.Api.DTOs

{{

    /// <summary>    /// <summary>

    /// DTO para la creación de un proyecto    /// DTO para la creación de un proyecto

    /// </summary>    /// </summary>

    public class CreateProjectDto    public class CreateProjectDto

    {    {

        /// <summary>        /// <summary>

        /// Nombre del proyecto        /// Nombre del proyecto

        /// </summary>        /// </summary>

        [Required]        [Required]

        [StringLength(100, MinimumLength = 3)]        [StringLength(100, MinimumLength = 3)]

        public string Name { get; set; } = string.Empty;        public string Name { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Descripción del proyecto        /// Descripción del proyecto

        /// </summary>        /// </summary>

        [StringLength(500)]        [StringLength(500)]

        public string Description { get; set; } = string.Empty;        public string Description { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Fecha de inicio del proyecto        /// Fecha de inicio del proyecto

        /// </summary>        /// </summary>

        [Required]        [Required]

        public DateTime StartDate { get; set; }        public DateTime StartDate { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de finalización del proyecto (opcional)        /// Fecha de finalización del proyecto (opcional)

        /// </summary>        /// </summary>

        public DateTime? EndDate { get; set; }        public DateTime? EndDate { get; set; }



        /// <summary>        /// <summary>

        /// Prioridad del proyecto        /// Prioridad del proyecto

        /// </summary>        /// </summary>

        [StringLength(10)]        [StringLength(10)]

        public string Priority { get; set; } = "Medium";        public string Priority { get; set; } = "Medium";

    }    }



    /// <summary>    /// <summary>

    /// DTO para la actualización de un proyecto    /// DTO para la actualización de un proyecto

    /// </summary>    /// </summary>

    public class UpdateProjectDto    public class UpdateProjectDto

    {    {

        /// <summary>        /// <summary>

        /// Nombre del proyecto        /// Nombre del proyecto

        /// </summary>        /// </summary>

        [StringLength(100, MinimumLength = 3)]        [StringLength(100, MinimumLength = 3)]

        public string? Name { get; set; }        public string? Name { get; set; }



        /// <summary>        /// <summary>

        /// Descripción del proyecto        /// Descripción del proyecto

        /// </summary>        /// </summary>

        [StringLength(500)]        [StringLength(500)]

        public string? Description { get; set; }        public string? Description { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de inicio del proyecto        /// Fecha de inicio del proyecto

        /// </summary>        /// </summary>

        public DateTime? StartDate { get; set; }        public DateTime? StartDate { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de finalización del proyecto        /// Fecha de finalización del proyecto

        /// </summary>        /// </summary>

        public DateTime? EndDate { get; set; }        public DateTime? EndDate { get; set; }



        /// <summary>        /// <summary>

        /// Estado del proyecto        /// Estado del proyecto

        /// </summary>        /// </summary>

        [StringLength(20)]        [StringLength(20)]

        public string? Status { get; set; }        public string? Status { get; set; }



        /// <summary>        /// <summary>

        /// Prioridad del proyecto        /// Prioridad del proyecto

        /// </summary>        /// </summary>

        [StringLength(10)]        [StringLength(10)]

        public string? Priority { get; set; }        public string? Priority { get; set; }

    }    }



    /// <summary>    /// <summary>

    /// DTO para la respuesta de un proyecto    /// DTO para la respuesta de un proyecto

    /// </summary>    /// </summary>

    public class ProjectDto    public class ProjectDto

    {    {

        /// <summary>        /// <summary>

        /// ID del proyecto        /// ID del proyecto

        /// </summary>        /// </summary>

        public Guid Id { get; set; }        public Guid Id { get; set; }



        /// <summary>        /// <summary>

        /// Nombre del proyecto        /// Nombre del proyecto

        /// </summary>        /// </summary>

        public string Name { get; set; } = string.Empty;        public string Name { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Descripción del proyecto        /// Descripción del proyecto

        /// </summary>        /// </summary>

        public string Description { get; set; } = string.Empty;        public string Description { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Fecha de inicio del proyecto        /// Fecha de inicio del proyecto

        /// </summary>        /// </summary>

        public DateTime StartDate { get; set; }        public DateTime StartDate { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de finalización del proyecto        /// Fecha de finalización del proyecto

        /// </summary>        /// </summary>

        public DateTime? EndDate { get; set; }        public DateTime? EndDate { get; set; }



        /// <summary>        /// <summary>

        /// Estado del proyecto        /// Estado del proyecto

        /// </summary>        /// </summary>

        public string Status { get; set; } = string.Empty;        public string Status { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Prioridad del proyecto        /// Prioridad del proyecto

        /// </summary>        /// </summary>

        public string Priority { get; set; } = string.Empty;        public string Priority { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// ID del propietario        /// ID del propietario

        /// </summary>        /// </summary>

        public Guid OwnerId { get; set; }        public Guid OwnerId { get; set; }



        /// <summary>        /// <summary>

        /// Nombre del propietario        /// Nombre del propietario

        /// </summary>        /// </summary>

        public string OwnerName { get; set; } = string.Empty;        public string OwnerName { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Fecha de creación        /// Fecha de creación

        /// </summary>        /// </summary>

        public DateTime CreatedAt { get; set; }        public DateTime CreatedAt { get; set; }



        /// <summary>        /// <summary>

        /// Fecha de actualización        /// Fecha de actualización

        /// </summary>        /// </summary>

        public DateTime UpdatedAt { get; set; }        public DateTime UpdatedAt { get; set; }



        /// <summary>        /// <summary>

        /// Número de tareas        /// Número de tareas

        /// </summary>        /// </summary>

        public int TaskCount { get; set; }        public int TaskCount { get; set; }



        /// <summary>        /// <summary>

        /// Número de miembros        /// Número de miembros

        /// </summary>        /// </summary>

        public int MemberCount { get; set; }        public int MemberCount { get; set; }

    }    }



    /// <summary>    /// <summary>

    /// DTO para respuesta detallada de proyecto    /// DTO para respuesta detallada de proyecto

    /// </summary>    /// </summary>

    public class ProjectDetailDto : ProjectDto    public class ProjectDetailDto : ProjectDto

    {    {

        /// <summary>        /// <summary>

        /// Lista de tareas del proyecto        /// Lista de tareas del proyecto

        /// </summary>        /// </summary>

        public List<TaskDto> Tasks { get; set; } = new();        public List<TaskDto> Tasks { get; set; } = new();



        /// <summary>        /// <summary>

        /// Lista de miembros del proyecto        /// Lista de miembros del proyecto

        /// </summary>        /// </summary>

        public List<ProjectMemberDto> Members { get; set; } = new();        public List<ProjectMemberDto> Members { get; set; } = new();

    }    }



    /// <summary>    /// <summary>

    /// DTO para miembro de proyecto    /// DTO para miembro de proyecto

    /// </summary>    /// </summary>

    public class ProjectMemberDto    public class ProjectMemberDto

    {    {

        /// <summary>        /// <summary>

        /// ID del usuario        /// ID del usuario

        /// </summary>        /// </summary>

        public Guid UserId { get; set; }        public Guid UserId { get; set; }



        /// <summary>        /// <summary>

        /// Nombre del usuario        /// Nombre del usuario

        /// </summary>        /// </summary>

        public string UserName { get; set; } = string.Empty;        public string UserName { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Email del usuario        /// Email del usuario

        /// </summary>        /// </summary>

        public string Email { get; set; } = string.Empty;        public string Email { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Rol en el proyecto        /// Rol en el proyecto

        /// </summary>        /// </summary>

        public string Role { get; set; } = string.Empty;        public string Role { get; set; } = string.Empty;



        /// <summary>        /// <summary>

        /// Fecha de ingreso al proyecto        /// Fecha de ingreso al proyecto

        /// </summary>        /// </summary>

        public DateTime JoinedAt { get; set; }        public DateTime JoinedAt { get; set; }

    }    }



    /// <summary>    /// <summary>

    /// DTO para agregar miembro al proyecto    /// DTO para agregar miembro al proyecto

    /// </summary>    /// </summary>

    public class AddProjectMemberDto    public class AddProjectMemberDto

    {    {

        /// <summary>        /// <summary>

        /// ID del usuario a agregar        /// ID del usuario a agregar

        /// </summary>        /// </summary>

        [Required]        [Required]

        public Guid UserId { get; set; }        public Guid UserId { get; set; }



        /// <summary>        /// <summary>

        /// Rol en el proyecto        /// Rol en el proyecto

        /// </summary>        /// </summary>

        [StringLength(20)]        [StringLength(20)]

        public string Role { get; set; } = "Member";        public string Role { get; set; } = "Member";

    }    }

}}
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