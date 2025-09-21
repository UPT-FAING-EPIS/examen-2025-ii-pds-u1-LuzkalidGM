using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.Services
{
    /// <summary>
    /// Interfaz para el servicio de proyectos
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Obtiene todos los proyectos
        /// </summary>
        /// <returns>Lista de proyectos</returns>
        Task<IEnumerable<ProjectSummaryDto>> GetAllProjectsAsync();

        /// <summary>
        /// Obtiene un proyecto por ID
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <returns>Proyecto encontrado</returns>
        Task<ProjectDto?> GetProjectByIdAsync(Guid id);

        /// <summary>
        /// Crea un nuevo proyecto
        /// </summary>
        /// <param name="createProjectDto">Datos del proyecto a crear</param>
        /// <param name="createdById">ID del usuario que crea el proyecto</param>
        /// <returns>Proyecto creado</returns>
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, Guid createdById);

        /// <summary>
        /// Actualiza un proyecto existente
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <param name="updateProjectDto">Datos actualizados del proyecto</param>
        /// <returns>Proyecto actualizado</returns>
        Task<ProjectDto?> UpdateProjectAsync(Guid id, UpdateProjectDto updateProjectDto);

        /// <summary>
        /// Elimina un proyecto
        /// </summary>
        /// <param name="id">ID del proyecto a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteProjectAsync(int id);

        /// <summary>
        /// Obtiene proyectos asignados a un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de proyectos del usuario</returns>
        Task<IEnumerable<ProjectSummaryDto>> GetUserProjectsAsync(Guid userId);
    }

    /// <summary>
    /// Implementación del servicio de proyectos
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly ProjectManagementContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del servicio de proyectos
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="mapper">Mapeador de objetos</param>
        public ProjectService(ProjectManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los proyectos
        /// </summary>
        /// <returns>Lista de proyectos</returns>
        public async Task<IEnumerable<ProjectSummaryDto>> GetAllProjectsAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Responsible)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return projects.Select(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Responsible = p.Responsible?.FullName ?? "Sin asignar",
                TotalTasks = p.Tasks.Count,
                CompletedTasks = p.Tasks.Count(t => t.Status == Models.TaskStatus.Completed.ToString()),
                ProgressPercentage = p.Tasks.Count > 0 ? 
                    (double)p.Tasks.Count(t => t.Status == Models.TaskStatus.Completed.ToString()) / p.Tasks.Count * 100 : 0
            });
        }

        /// <summary>
        /// Obtiene un proyecto por ID
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <returns>Proyecto encontrado</returns>
        public async Task<ProjectDto?> GetProjectByIdAsync(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Responsible)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.AssignedTo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                CreatedBy = _mapper.Map<UserSummaryDto>(project.CreatedBy),
                Responsible = _mapper.Map<UserSummaryDto>(project.Responsible),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                Tasks = project.Tasks.Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    AssignedTo = t.AssignedTo != null ? _mapper.Map<UserSummaryDto>(t.AssignedTo) : null,
                    EstimatedHours = t.EstimatedHours,
                    ActualHours = t.ActualHours
                }).ToList()
            };
        }

        /// <summary>
        /// Crea un nuevo proyecto
        /// </summary>
        /// <param name="createProjectDto">Datos del proyecto a crear</param>
        /// <param name="createdById">ID del usuario que crea el proyecto</param>
        /// <returns>Proyecto creado</returns>
        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto, Guid createdById)
        {
            var project = new Project
            {
                Name = createProjectDto.Name,
                Description = createProjectDto.Description,
                StartDate = createProjectDto.StartDate,
                EndDate = createProjectDto.EndDate,
                OwnerId = createProjectDto.ResponsibleId ?? createdById,
                Status = ProjectStatus.Planning.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Cargar las relaciones para el resultado
            await _context.Entry(project)
                .Reference(p => p.CreatedBy)
                .LoadAsync();
            await _context.Entry(project)
                .Reference(p => p.Responsible)
                .LoadAsync();

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                CreatedBy = _mapper.Map<UserSummaryDto>(project.CreatedBy),
                Responsible = _mapper.Map<UserSummaryDto>(project.Responsible),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                Tasks = new List<TaskSummaryDto>()
            };
        }

        /// <summary>
        /// Actualiza un proyecto existente
        /// </summary>
        /// <param name="id">ID del proyecto</param>
        /// <param name="updateProjectDto">Datos actualizados del proyecto</param>
        /// <returns>Proyecto actualizado</returns>
        public async Task<ProjectDto?> UpdateProjectAsync(Guid id, UpdateProjectDto updateProjectDto)
        {
            var project = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Responsible)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return null;

            project.Name = updateProjectDto.Name ?? project.Name;
            project.Description = updateProjectDto.Description ?? project.Description;
            project.StartDate = updateProjectDto.StartDate ?? project.StartDate;
            project.EndDate = updateProjectDto.EndDate;
            project.Status = updateProjectDto.Status ?? project.Status;
            project.OwnerId = updateProjectDto.ResponsibleId ?? project.OwnerId;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Recargar el responsable si cambió
            if (project.ResponsibleId != updateProjectDto.ResponsibleId)
            {
                await _context.Entry(project)
                    .Reference(p => p.Responsible)
                    .LoadAsync();
            }

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                CreatedBy = _mapper.Map<UserSummaryDto>(project.CreatedBy),
                Responsible = _mapper.Map<UserSummaryDto>(project.Responsible),
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                Tasks = new List<TaskSummaryDto>()
            };
        }

        /// <summary>
        /// Elimina un proyecto
        /// </summary>
        /// <param name="id">ID del proyecto a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene proyectos asignados a un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de proyectos del usuario</returns>
        public async Task<IEnumerable<ProjectSummaryDto>> GetUserProjectsAsync(Guid userId)
        {
            var projects = await _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.Responsible)
                .Include(p => p.Tasks)
                .Where(p => p.ResponsibleId == userId || p.CreatedById == userId || 
                           p.Tasks.Any(t => t.AssignedToId == userId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return projects.Select(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Responsible = p.Responsible?.FullName ?? "Sin asignar",
                TotalTasks = p.Tasks.Count,
                CompletedTasks = p.Tasks.Count(t => t.Status == Models.TaskStatus.Completed.ToString()),
                ProgressPercentage = p.Tasks.Count > 0 ? 
                    (double)p.Tasks.Count(t => t.Status == Models.TaskStatus.Completed.ToString()) / p.Tasks.Count * 100 : 0
            });
        }
    }
}
