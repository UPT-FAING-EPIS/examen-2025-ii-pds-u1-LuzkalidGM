using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Api.Models
{
    /// <summary>
    /// Entidad que representa la membresía de un usuario en un proyecto
    /// </summary>
    public class ProjectMember
    {
        /// <summary>
        /// ID del proyecto
        /// </summary>
        [Required]
        public Guid ProjectId { get; set; }

        /// <summary>
        /// ID del usuario
        /// </summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>
        /// Rol del usuario en el proyecto
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Member";

        /// <summary>
        /// Fecha en que se unió al proyecto
        /// </summary>
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Proyecto al que pertenece la membresía
        /// </summary>
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// Usuario que es miembro del proyecto
        /// </summary>
        public virtual User User { get; set; } = null!;
    }
}