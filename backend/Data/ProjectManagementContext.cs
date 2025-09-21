using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.Data
{
    /// <summary>
    /// Contexto de base de datos para la aplicación de gestión de proyectos
    /// </summary>
    public class ProjectManagementContext : DbContext
    {
        /// <summary>
        /// Constructor del contexto
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto</param>
        public ProjectManagementContext(DbContextOptions<ProjectManagementContext> options) : base(options)
        {
        }

        /// <summary>
        /// Conjunto de entidades de usuarios
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Conjunto de entidades de proyectos
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// Conjunto de entidades de tareas
        /// </summary>
        public DbSet<ProjectTask> Tasks { get; set; }

        /// <summary>
        /// Conjunto de entidades de comentarios de tareas
        /// </summary>
        public DbSet<TaskComment> TaskComments { get; set; }

        /// <summary>
        /// Configuración del modelo de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.Property(e => e.Role)
                    .HasConversion<int>();
            });

            // Configuración de Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Status)
                    .HasConversion<int>();

                // Relación con Usuario Creador
                entity.HasOne(p => p.CreatedBy)
                    .WithMany(u => u.CreatedProjects)
                    .HasForeignKey(p => p.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Usuario Responsable
                entity.HasOne(p => p.Responsible)
                    .WithMany(u => u.AssignedProjects)
                    .HasForeignKey(p => p.ResponsibleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ProjectTask
            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.Property(e => e.Status)
                    .HasConversion<int>();

                entity.Property(e => e.Priority)
                    .HasConversion<int>();

                entity.Property(e => e.EstimatedHours)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.ActualHours)
                    .HasColumnType("decimal(18,2)");

                // Relación con Proyecto
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Usuario Asignado
                entity.HasOne(t => t.AssignedTo)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(t => t.AssignedToId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con Usuario Creador
                entity.HasOne(t => t.CreatedBy)
                    .WithMany()
                    .HasForeignKey(t => t.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de TaskComment
            modelBuilder.Entity<TaskComment>(entity =>
            {
                // Relación con Tarea
                entity.HasOne(c => c.Task)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con Usuario Creador
                entity.HasOne(c => c.CreatedBy)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(c => c.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Datos iniciales
            SeedData(modelBuilder);
        }

        /// <summary>
        /// Siembra datos iniciales en la base de datos
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo</param>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Usuario administrador por defecto
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@projectmanagement.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FullName = "Administrador del Sistema",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}