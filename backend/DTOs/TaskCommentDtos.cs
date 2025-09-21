using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Api.DTOs
{
    /// <summary>
    /// DTO para la creación de un comentario
    /// </summary>
    public class CreateTaskCommentDto
    {
        /// <summary>
        /// Contenido del comentario
        /// </summary>
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para la respuesta de un comentario
    /// </summary>
    public class TaskCommentDto
    {
        /// <summary>
        /// ID del comentario
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Contenido del comentario
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Usuario que creó el comentario
        /// </summary>
        public UserSummaryDto CreatedBy { get; set; } = null!;

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}