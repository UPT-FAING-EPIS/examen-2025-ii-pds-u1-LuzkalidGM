using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Services;
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
        private readonly IProjectService _projectService;

        /// <summary>
        /// Constructor del controlador de proyectos
        /// </summary>
        /// <param name="projectService">Servicio de proyectos</param>
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        /// <summary>
        /// Obtiene todos los proyectos
        /// </summary>
        /// <returns>Lista de proyectos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProjectSummaryDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProjectSummaryDto>>> GetProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        /// <summary>
        /// Obtiene un proyecto específico por ID
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <returns>Proyecto encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProjectDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            
            if (project == null)
            {
                return NotFound($"Proyecto con ID {id} no encontrado.");
            }

            return Ok(project);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var project = await _projectService.CreateProjectAsync(createProjectDto, userId);

            return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
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
        public async Task<ActionResult<ProjectDto>> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var project = await _projectService.UpdateProjectAsync(id, updateProjectDto);
            
            if (project == null)
            {
                return NotFound($"Proyecto con ID {id} no encontrado.");
            }

            return Ok(project);
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
        public async Task<IActionResult> DeleteProject(int id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            
            if (!result)
            {
                return NotFound($"Proyecto con ID {id} no encontrado.");
            }

            return NoContent();
        }

        /// <summary>
        /// Obtiene proyectos del usuario actual
        /// </summary>
        /// <returns>Lista de proyectos del usuario</returns>
        [HttpGet("my-projects")]
        [ProducesResponseType(typeof(IEnumerable<ProjectSummaryDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProjectSummaryDto>>> GetMyProjects()
        {
            var userId = GetCurrentUserId();
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Ok(projects);
        }

        /// <summary>
        /// Obtiene el ID del usuario actual desde el token JWT
        /// </summary>
        /// <returns>ID del usuario</returns>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Token de usuario inválido.");
            }

            return userId;
        }
    }
}