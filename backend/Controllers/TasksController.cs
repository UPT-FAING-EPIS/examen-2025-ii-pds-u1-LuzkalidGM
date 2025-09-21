using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Services;
using System.Security.Claims;

namespace ProjectManagement.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de tareas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Constructor del controlador de tareas
        /// </summary>
        /// <param name="taskService">Servicio de tareas</param>
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Obtiene tareas filtradas por proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto (opcional)</param>
        /// <returns>Lista de tareas</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), 200)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks([FromQuery] Guid? projectId)
        {
            if (projectId.HasValue)
            {
                var tasks = await _taskService.GetTasksByProjectIdAsync(projectId.Value);
                return Ok(tasks);
            }

            // Si no se especifica proyecto, devolver tareas del usuario actual
            var userId = GetCurrentUserId();
            var userTasks = await _taskService.GetUserTasksAsync(userId);
            return Ok(userTasks);
        }

        /// <summary>
        /// Obtiene una tarea específica por ID
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <returns>Tarea encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TaskDto>> GetTask(Guid id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            
            if (task == null)
            {
                return NotFound($"Tarea con ID {id} no encontrada.");
            }

            return Ok(task);
        }

        /// <summary>
        /// Crea una nueva tarea
        /// </summary>
        /// <param name="createTaskDto">Datos de la tarea a crear</param>
        /// <returns>Tarea creada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var task = await _taskService.CreateTaskAsync(createTaskDto, userId);

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Actualiza una tarea existente
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <param name="updateTaskDto">Datos actualizados de la tarea</param>
        /// <returns>Tarea actualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TaskDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TaskDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto);
            
            if (task == null)
            {
                return NotFound($"Tarea con ID {id} no encontrada.");
            }

            return Ok(task);
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var result = await _taskService.DeleteTaskAsync(id);
            
            if (!result)
            {
                return NotFound($"Tarea con ID {id} no encontrada.");
            }

            return NoContent();
        }

        /// <summary>
        /// Obtiene tareas asignadas al usuario actual
        /// </summary>
        /// <returns>Lista de tareas del usuario</returns>
        [HttpGet("my-tasks")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), 200)]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetMyTasks()
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetUserTasksAsync(userId);
            return Ok(tasks);
        }

        /// <summary>
        /// Agrega un comentario a una tarea
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <param name="createCommentDto">Datos del comentario</param>
        /// <returns>Comentario creado</returns>
        [HttpPost("{id}/comments")]
        [ProducesResponseType(typeof(TaskCommentDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TaskCommentDto>> AddComment(Guid id, [FromBody] CreateTaskCommentDto createCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = GetCurrentUserId();
                var comment = await _taskService.AddCommentAsync(id, createCommentDto, userId);
                return CreatedAtAction(nameof(GetTask), new { id }, comment);
            }
            catch (ArgumentException)
            {
                return NotFound($"Tarea con ID {id} no encontrada.");
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