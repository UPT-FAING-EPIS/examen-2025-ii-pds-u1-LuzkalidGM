using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.Models;
using ProjectManagement.Api.DTOs;
using System.Security.Claims;

namespace ProjectManagement.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de proyectos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ProjectManagementContext _context;
        private readonly ILogger<ProjectsController> _logger;

        /// <summary>
        /// Constructor del controlador de proyectos
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="logger">Logger</param>
        public ProjectsController(ProjectManagementContext context, ILogger<ProjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los proyectos
        /// </summary>
        /// <returns>Lista de proyectos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            try
            {
                var projects = await _context.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Members)
                        .ThenInclude(m => m.User)
                    .Include(p => p.Tasks)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                var projectDtos = projects.Select(MapToProjectDto).ToList();
                return Ok(projectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene un proyecto específico por ID
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <returns>Proyecto encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProjectDto>> GetProject(Guid id)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Members)
                        .ThenInclude(m => m.User)
                    .Include(p => p.Tasks)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return NotFound($"Proyecto con ID {id} no encontrado.");
                }

                return Ok(MapToProjectDto(project));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Crea un nuevo proyecto
        /// </summary>
        /// <param name="createProjectDto">Datos del proyecto a crear</param>
        /// <returns>Proyecto creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProjectDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    Name = createProjectDto.Name,
                    Description = createProjectDto.Description,
                    StartDate = createProjectDto.StartDate,
                    EndDate = createProjectDto.EndDate,
                    Priority = createProjectDto.Priority,
                    Status = "Planning",
                    OwnerId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                // Reload with includes
                var createdProject = await _context.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Members)
                        .ThenInclude(m => m.User)
                    .Include(p => p.Tasks)
                    .FirstOrDefaultAsync(p => p.Id == project.Id);

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, MapToProjectDto(createdProject!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Actualiza un proyecto existente
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <param name="updateProjectDto">Datos actualizados del proyecto</param>
        /// <returns>Proyecto actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProjectDto>> UpdateProject(Guid id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var project = await _context.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Members)
                        .ThenInclude(m => m.User)
                    .Include(p => p.Tasks)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return NotFound($"Proyecto con ID {id} no encontrado.");
                }

                // Update properties
                if (!string.IsNullOrEmpty(updateProjectDto.Name))
                    project.Name = updateProjectDto.Name;
                
                if (!string.IsNullOrEmpty(updateProjectDto.Description))
                    project.Description = updateProjectDto.Description;
                
                if (updateProjectDto.StartDate.HasValue)
                    project.StartDate = updateProjectDto.StartDate.Value;
                
                if (updateProjectDto.EndDate.HasValue)
                    project.EndDate = updateProjectDto.EndDate.Value;
                
                if (!string.IsNullOrEmpty(updateProjectDto.Priority))
                    project.Priority = updateProjectDto.Priority;
                
                if (!string.IsNullOrEmpty(updateProjectDto.Status))
                    project.Status = updateProjectDto.Status;

                project.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(MapToProjectDto(project));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Elimina un proyecto
        /// </summary>
        /// <param name="id">ID del proyecto a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                var project = await _context.Projects
                    .Include(p => p.Tasks)
                    .Include(p => p.Members)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (project == null)
                {
                    return NotFound($"Proyecto con ID {id} no encontrado.");
                }

                _context.Tasks.RemoveRange(project.Tasks);
                _context.ProjectMembers.RemoveRange(project.Members);
                _context.Projects.Remove(project);
                
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene proyectos del usuario actual
        /// </summary>
        /// <returns>Lista de proyectos del usuario</returns>
        [HttpGet("my-projects")]
        [ProducesResponseType(typeof(IEnumerable<ProjectDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetMyProjects()
        {
            try
            {
                var userId = GetCurrentUserId();
                var projects = await _context.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Members)
                        .ThenInclude(m => m.User)
                    .Include(p => p.Tasks)
                    .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                var projectDtos = projects.Select(MapToProjectDto).ToList();
                return Ok(projectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user projects");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Mapea una entidad Project a ProjectDto
        /// </summary>
        /// <param name="project">Entidad Project</param>
        /// <returns>ProjectDto</returns>
        private ProjectDto MapToProjectDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Status = project.Status,
                Priority = project.Priority,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                OwnerId = project.OwnerId,
                Owner = project.Owner != null ? new UserDto
                {
                    Id = project.Owner.Id,
                    FirstName = project.Owner.FirstName,
                    LastName = project.Owner.LastName,
                    Email = project.Owner.Email
                } : null,
                Members = project.Members?.Select(m => new ProjectMemberDto
                {
                    Id = m.UserId, // Use UserId as the member ID for simplicity
                    ProjectId = m.ProjectId,
                    UserId = m.UserId,
                    Role = m.Role,
                    JoinedDate = m.JoinedAt, // Map JoinedAt to JoinedDate
                    User = m.User != null ? new UserDto
                    {
                        Id = m.User.Id,
                        FirstName = m.User.FirstName,
                        LastName = m.User.LastName,
                        Email = m.User.Email
                    } : null
                }).ToList(),
                Tasks = project.Tasks?.Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours,
                    AssignedTo = t.AssignedTo != null ? new UserSummaryDto
                    {
                        Id = t.AssignedTo.Id,
                        Username = t.AssignedTo.Email,
                        FullName = $"{t.AssignedTo.FirstName} {t.AssignedTo.LastName}",
                        Role = t.AssignedTo.Role
                    } : null
                }).ToList(),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt
            };
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el token JWT
        /// </summary>
        /// <returns>ID del usuario</returns>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                throw new UnauthorizedAccessException("Token de usuario inválido.");
            }

            return userId;
        }
    }
}