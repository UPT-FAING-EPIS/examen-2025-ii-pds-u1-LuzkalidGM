using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.Services
{
    /// <summary>
    /// Interfaz para el servicio de tareas
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Obtiene todas las tareas de un proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de tareas</returns>
        Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId);

        /// <summary>
        /// Obtiene una tarea por ID
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <returns>Tarea encontrada</returns>
        Task<TaskDto?> GetTaskByIdAsync(int id);

        /// <summary>
        /// Crea una nueva tarea
        /// </summary>
        /// <param name="createTaskDto">Datos de la tarea a crear</param>
        /// <param name="createdById">ID del usuario que crea la tarea</param>
        /// <returns>Tarea creada</returns>
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, int createdById);

        /// <summary>
        /// Actualiza una tarea existente
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <param name="updateTaskDto">Datos actualizados de la tarea</param>
        /// <returns>Tarea actualizada</returns>
        Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto);

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteTaskAsync(int id);

        /// <summary>
        /// Obtiene tareas asignadas a un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de tareas del usuario</returns>
        Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId);

        /// <summary>
        /// Agrega un comentario a una tarea
        /// </summary>
        /// <param name="taskId">ID de la tarea</param>
        /// <param name="createCommentDto">Datos del comentario</param>
        /// <param name="createdById">ID del usuario que crea el comentario</param>
        /// <returns>Comentario creado</returns>
        Task<TaskCommentDto> AddCommentAsync(int taskId, CreateTaskCommentDto createCommentDto, int createdById);
    }

    /// <summary>
    /// Implementación del servicio de tareas
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly ProjectManagementContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del servicio de tareas
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="mapper">Mapeador de objetos</param>
        public TaskService(ProjectManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todas las tareas de un proyecto
        /// </summary>
        /// <param name="projectId">ID del proyecto</param>
        /// <returns>Lista de tareas</returns>
        public async Task<IEnumerable<TaskDto>> GetTasksByProjectIdAsync(int projectId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Responsible)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedBy)
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(MapToTaskDto);
        }

        /// <summary>
        /// Obtiene una tarea por ID
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <returns>Tarea encontrada</returns>
        public async Task<TaskDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Responsible)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            return task != null ? MapToTaskDto(task) : null;
        }

        /// <summary>
        /// Crea una nueva tarea
        /// </summary>
        /// <param name="createTaskDto">Datos de la tarea a crear</param>
        /// <param name="createdById">ID del usuario que crea la tarea</param>
        /// <returns>Tarea creada</returns>
        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, int createdById)
        {
            var task = new ProjectTask
            {
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                ProjectId = createTaskDto.ProjectId,
                AssignedToId = createTaskDto.AssignedToId,
                Priority = createTaskDto.Priority,
                DueDate = createTaskDto.DueDate,
                EstimatedHours = createTaskDto.EstimatedHours,
                CreatedById = createdById,
                Status = TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Cargar las relaciones para el resultado
            await _context.Entry(task)
                .Reference(t => t.Project)
                .LoadAsync();
            await _context.Entry(task.Project)
                .Reference(p => p.Responsible)
                .LoadAsync();
            await _context.Entry(task)
                .Reference(t => t.AssignedTo)
                .LoadAsync();
            await _context.Entry(task)
                .Reference(t => t.CreatedBy)
                .LoadAsync();

            return MapToTaskDto(task);
        }

        /// <summary>
        /// Actualiza una tarea existente
        /// </summary>
        /// <param name="id">ID de la tarea</param>
        /// <param name="updateTaskDto">Datos actualizados de la tarea</param>
        /// <returns>Tarea actualizada</returns>
        public async Task<TaskDto?> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Responsible)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null) return null;

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.Priority = updateTaskDto.Priority;
            task.AssignedToId = updateTaskDto.AssignedToId;
            task.DueDate = updateTaskDto.DueDate;
            task.EstimatedHours = updateTaskDto.EstimatedHours;
            task.ActualHours = updateTaskDto.ActualHours;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Recargar el usuario asignado si cambió
            if (task.AssignedToId != updateTaskDto.AssignedToId)
            {
                await _context.Entry(task)
                    .Reference(t => t.AssignedTo)
                    .LoadAsync();
            }

            return MapToTaskDto(task);
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <param name="id">ID de la tarea a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene tareas asignadas a un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de tareas del usuario</returns>
        public async Task<IEnumerable<TaskDto>> GetUserTasksAsync(int userId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Project)
                    .ThenInclude(p => p.Responsible)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.CreatedBy)
                .Where(t => t.AssignedToId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(MapToTaskDto);
        }

        /// <summary>
        /// Agrega un comentario a una tarea
        /// </summary>
        /// <param name="taskId">ID de la tarea</param>
        /// <param name="createCommentDto">Datos del comentario</param>
        /// <param name="createdById">ID del usuario que crea el comentario</param>
        /// <returns>Comentario creado</returns>
        public async Task<TaskCommentDto> AddCommentAsync(int taskId, CreateTaskCommentDto createCommentDto, int createdById)
        {
            var comment = new TaskComment
            {
                Content = createCommentDto.Content,
                TaskId = taskId,
                CreatedById = createdById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TaskComments.Add(comment);
            await _context.SaveChangesAsync();

            // Cargar las relaciones para el resultado
            await _context.Entry(comment)
                .Reference(c => c.CreatedBy)
                .LoadAsync();

            return new TaskCommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedBy = _mapper.Map<UserSummaryDto>(comment.CreatedBy),
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        /// <summary>
        /// Mapea una tarea a DTO
        /// </summary>
        /// <param name="task">Tarea a mapear</param>
        /// <returns>DTO de la tarea</returns>
        private TaskDto MapToTaskDto(ProjectTask task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                Project = new ProjectSummaryDto
                {
                    Id = task.Project.Id,
                    Name = task.Project.Name,
                    Description = task.Project.Description,
                    Status = task.Project.Status,
                    StartDate = task.Project.StartDate,
                    EndDate = task.Project.EndDate,
                    Responsible = _mapper.Map<UserSummaryDto>(task.Project.Responsible),
                    TotalTasks = 0, // Se puede calcular si es necesario
                    CompletedTasks = 0,
                    ProgressPercentage = 0
                },
                AssignedTo = task.AssignedTo != null ? _mapper.Map<UserSummaryDto>(task.AssignedTo) : null,
                CreatedBy = _mapper.Map<UserSummaryDto>(task.CreatedBy),
                EstimatedHours = task.EstimatedHours,
                ActualHours = task.ActualHours,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                Comments = task.Comments.Select(c => new TaskCommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedBy = _mapper.Map<UserSummaryDto>(c.CreatedBy),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).OrderBy(c => c.CreatedAt).ToList()
            };
        }
    }
}