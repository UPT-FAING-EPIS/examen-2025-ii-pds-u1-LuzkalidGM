using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.DTOs;
using System.Security.Claims;

namespace ProjectManagement.Api.Controllers
{
    /// <summary>
    /// Controlador para estadísticas del dashboard
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ProjectManagementContext _context;
        private readonly ILogger<DashboardController> _logger;

        /// <summary>
        /// Constructor del controlador de dashboard
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="logger">Logger</param>
        public DashboardController(ProjectManagementContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene estadísticas del dashboard
        /// </summary>
        /// <returns>Estadísticas del dashboard</returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(DashboardStatsDto), 200)]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var userId = GetCurrentUserId();

                // Contar proyectos
                var totalProjects = await _context.Projects
                    .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
                    .CountAsync();

                var activeProjects = await _context.Projects
                    .Where(p => (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)) && 
                               p.Status == "InProgress")
                    .CountAsync();

                // Contar tareas
                var totalTasks = await _context.Tasks
                    .Where(t => t.Project.OwnerId == userId || 
                               t.Project.Members.Any(m => m.UserId == userId))
                    .CountAsync();

                var pendingTasks = await _context.Tasks
                    .Where(t => (t.Project.OwnerId == userId || 
                               t.Project.Members.Any(m => m.UserId == userId)) && 
                               t.Status == "Pending")
                    .CountAsync();

                var completedTasks = await _context.Tasks
                    .Where(t => (t.Project.OwnerId == userId || 
                               t.Project.Members.Any(m => m.UserId == userId)) && 
                               t.Status == "Completed")
                    .CountAsync();

                var stats = new DashboardStatsDto
                {
                    TotalProjects = totalProjects,
                    ActiveProjects = activeProjects,
                    TotalTasks = totalTasks,
                    PendingTasks = pendingTasks,
                    CompletedTasks = completedTasks
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard stats");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene proyectos recientes
        /// </summary>
        /// <returns>Lista de proyectos recientes</returns>
        [HttpGet("recent-projects")]
        [ProducesResponseType(typeof(IEnumerable<ProjectDto>), 200)]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetRecentProjects()
        {
            try
            {
                var userId = GetCurrentUserId();

                var recentProjects = await _context.Projects
                    .Include(p => p.Owner)
                    .Include(p => p.Members)
                        .ThenInclude(m => m.User)
                    .Include(p => p.Tasks)
                    .Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId))
                    .OrderByDescending(p => p.UpdatedAt)
                    .Take(5)
                    .ToListAsync();

                var projectDtos = recentProjects.Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Status = p.Status,
                    Priority = p.Priority,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    OwnerId = p.OwnerId,
                    Owner = p.Owner != null ? new UserDto
                    {
                        Id = p.Owner.Id,
                        FirstName = p.Owner.FirstName,
                        LastName = p.Owner.LastName,
                        Email = p.Owner.Email
                    } : null,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                }).ToList();

                return Ok(projectDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent projects");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Obtiene tareas recientes
        /// </summary>
        /// <returns>Lista de tareas recientes</returns>
        [HttpGet("recent-tasks")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), 200)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetRecentTasks()
        {
            try
            {
                var userId = GetCurrentUserId();

                var recentTasks = await _context.Tasks
                    .Include(t => t.Project)
                        .ThenInclude(p => p.Owner)
                    .Where(t => t.Project.OwnerId == userId || 
                               t.Project.Members.Any(m => m.UserId == userId))
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(10)
                    .ToListAsync();

                var taskDtos = recentTasks.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority,
                    ProjectId = t.ProjectId,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToList();

                return Ok(taskDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent tasks");
                return StatusCode(500, "Internal server error");
            }
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