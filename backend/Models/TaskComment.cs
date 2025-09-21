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
        public int Id { get; set; }

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
        public int TaskId { get; set; }

        /// <summary>
        /// ID del usuario que creó el comentario
        /// </summary>
        [Required]
        public int CreatedById { get; set; }

        /// <summary>
        /// Fecha de creación del comentario
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización del comentario
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Tarea a la que pertenece el comentario
        /// </summary>
        [ForeignKey("TaskId")]
        public virtual ProjectTask Task { get; set; } = null!;

        /// <summary>
        /// Usuario que creó el comentario
        /// </summary>
        [ForeignKey("CreatedById")]
        public virtual User CreatedBy { get; set; } = null!;
    }
}