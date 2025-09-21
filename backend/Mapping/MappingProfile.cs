using AutoMapper;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.Mapping
{
    /// <summary>
    /// Perfil de mapeo para AutoMapper
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Constructor que configura los mapeos
        /// </summary>
        public MappingProfile()
        {
            // Mapeos de Usuario
            CreateMap<User, UserDto>();
            CreateMap<User, UserSummaryDto>();
            CreateMap<RegisterUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // Mapeos de Proyecto
            CreateMap<Project, ProjectDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Responsible, opt => opt.MapFrom(src => src.Responsible))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));
            
            CreateMap<Project, ProjectSummaryDto>()
                .ForMember(dest => dest.Responsible, opt => opt.MapFrom(src => src.Responsible))
                .ForMember(dest => dest.TotalTasks, opt => opt.MapFrom(src => src.Tasks.Count))
                .ForMember(dest => dest.CompletedTasks, opt => opt.MapFrom(src => src.Tasks.Count(t => t.Status == TaskStatus.Completed)))
                .ForMember(dest => dest.ProgressPercentage, opt => opt.MapFrom(src => 
                    src.Tasks.Count > 0 ? (decimal)src.Tasks.Count(t => t.Status == TaskStatus.Completed) / src.Tasks.Count * 100 : 0));
            
            CreateMap<CreateProjectDto, Project>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Mapeos de Tarea
            CreateMap<ProjectTask, TaskDto>()
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
            
            CreateMap<ProjectTask, TaskSummaryDto>()
                .ForMember(dest => dest.AssignedTo, opt => opt.MapFrom(src => src.AssignedTo));
            
            CreateMap<CreateTaskDto, ProjectTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.ActualHours, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Mapeos de Comentario
            CreateMap<TaskComment, TaskCommentDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
            
            CreateMap<CreateTaskCommentDto, TaskComment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TaskId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}