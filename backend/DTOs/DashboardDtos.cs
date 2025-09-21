namespace ProjectManagement.Api.DTOs
{
    /// <summary>
    /// DTO para estadísticas del dashboard
    /// </summary>
    public class DashboardStatsDto
    {
        /// <summary>
        /// Total de proyectos
        /// </summary>
        public int TotalProjects { get; set; }

        /// <summary>
        /// Proyectos activos
        /// </summary>
        public int ActiveProjects { get; set; }

        /// <summary>
        /// Total de tareas
        /// </summary>
        public int TotalTasks { get; set; }

        /// <summary>
        /// Tareas pendientes
        /// </summary>
        public int PendingTasks { get; set; }

        /// <summary>
        /// Tareas completadas
        /// </summary>
        public int CompletedTasks { get; set; }
    }
}