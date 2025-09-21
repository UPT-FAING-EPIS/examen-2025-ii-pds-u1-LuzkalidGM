using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Api.Models
{
    /// <summary>
    /// Entidad que representa un comentario en una tarea
    /// </summary>
    public class TaskComment
    {
        /// <summary>
        /// Identificador único del comentario
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Contenido del comentario
        /// </summary>
        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// ID de la tarea a la que pertenece el comentario
        /// </summary>
        [Required]
        public Guid TaskId { get; set; }

        /// <summary>
        /// ID del usuario que creó el comentario
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Fecha de creación del comentario
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Tarea a la que pertenece el comentario
        /// </summary>
        [ForeignKey(nameof(TaskId))]
        public virtual ProjectTask Task { get; set; } = null!;

        /// <summary>
        /// Usuario que creó el comentario
        /// </summary>
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}