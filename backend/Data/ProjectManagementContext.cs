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
        /// Conjunto de entidades de miembros de proyecto
        /// </summary>
        public DbSet<ProjectMember> ProjectMembers { get; set; }

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
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasDefaultValue("User");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configuración de Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("Planning");
                entity.Property(e => e.Priority).HasDefaultValue("Medium");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Relación con Owner
                entity.HasOne(p => p.Owner)
                    .WithMany(u => u.OwnedProjects)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ProjectTask
            modelBuilder.Entity<ProjectTask>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("Pending");
                entity.Property(e => e.Priority).HasDefaultValue("Medium");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Relación con Project
                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con AssignedTo (opcional)
                entity.HasOne(t => t.AssignedTo)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(t => t.AssignedToId)
                    .OnDelete(DeleteBehavior.SetNull);

                // Relación con CreatedBy
                entity.HasOne(t => t.CreatedBy)
                    .WithMany(u => u.CreatedTasks)
                    .HasForeignKey(t => t.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de TaskComment
            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Relación con Task
                entity.HasOne(c => c.Task)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con User
                entity.HasOne(c => c.User)
                    .WithMany(u => u.TaskComments)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ProjectMember
            modelBuilder.Entity<ProjectMember>(entity =>
            {
                // Clave compuesta
                entity.HasKey(pm => new { pm.ProjectId, pm.UserId });
                entity.Property(e => e.Role).HasDefaultValue("Member");
                entity.Property(e => e.JoinedAt).HasDefaultValueSql("GETUTCDATE()");

                // Relación con Project
                entity.HasOne(pm => pm.Project)
                    .WithMany(p => p.Members)
                    .HasForeignKey(pm => pm.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación con User
                entity.HasOne(pm => pm.User)
                    .WithMany(u => u.ProjectMemberships)
                    .HasForeignKey(pm => pm.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Índices adicionales para rendimiento
            modelBuilder.Entity<Project>()
                .HasIndex(p => p.OwnerId);

            modelBuilder.Entity<ProjectTask>()
                .HasIndex(t => t.ProjectId);

            modelBuilder.Entity<ProjectTask>()
                .HasIndex(t => t.AssignedToId);

            modelBuilder.Entity<TaskComment>()
                .HasIndex(c => c.TaskId);

            modelBuilder.Entity<ProjectMember>()
                .HasIndex(pm => pm.UserId);
        }
    }
}