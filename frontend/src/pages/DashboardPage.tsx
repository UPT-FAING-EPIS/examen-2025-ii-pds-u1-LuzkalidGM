import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useDashboardData } from '../hooks/useApi';
import { 
  ChartBarIcon, 
  FolderIcon, 
  UserGroupIcon, 
  ClockIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon
} from '@heroicons/react/24/outline';

const DashboardPage: React.FC = () => {
  const { user } = useAuth();
  const { projects, tasks, stats: dashboardStats, isLoading, error } = useDashboardData();

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Error al cargar datos</h2>
          <p className="text-gray-600">Por favor, intenta recargar la página</p>
        </div>
      </div>
    );
  }

  // Mock data para la UI - esto se puede usar como fallback
  const statsData = [
    {
      name: 'Proyectos Activos',
      value: dashboardStats.activeProjects.toString(),
      icon: FolderIcon,
      color: 'bg-blue-500',
      change: `${dashboardStats.totalProjects} total`
    },
    {
      name: 'Tareas Pendientes',
      value: dashboardStats.pendingTasks.toString(),
      icon: ClockIcon,
      color: 'bg-yellow-500',
      change: `${dashboardStats.inProgressTasks} en progreso`
    },
    {
      name: 'Tareas Completadas',
      value: dashboardStats.completedTasks.toString(),
      icon: CheckCircleIcon,
      color: 'bg-green-500',
      change: `${dashboardStats.totalTasks} total`
    },
    {
      name: 'Total Proyectos',
      value: dashboardStats.totalProjects.toString(),
      icon: UserGroupIcon,
      color: 'bg-purple-500',
      change: `${dashboardStats.activeProjects} activos`
    }
  ];

  const recentProjects = projects.slice(0, 3).map(project => ({
    id: project.id,
    name: project.name,
    status: project.status,
    progress: 75, // TODO: Calculate real progress based on tasks
    members: project.members?.length || 0,
    dueDate: new Date(project.endDate).toLocaleDateString()
  }));

  const recentTasks = tasks.slice(0, 3).map(task => ({
    id: task.id,
    title: task.title,
    project: task.project?.name || 'Sin proyecto',
    status: task.status,
    priority: task.priority,
    assignee: task.assignee ? `${task.assignee.firstName} ${task.assignee.lastName}` : 'Sin asignar'
  }));

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'InProgress':
        return 'bg-blue-100 text-blue-800';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Planning':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityColor = (priority: string) => {
    switch (priority) {
      case 'High':
        return 'text-red-600';
      case 'Medium':
        return 'text-yellow-600';
      case 'Low':
        return 'text-green-600';
      default:
        return 'text-gray-600';
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow">
        <div className="px-4 sm:px-6 lg:px-8">
          <div className="py-6">
            <h1 className="text-2xl font-bold text-gray-900">
              Bienvenido, {user?.firstName} {user?.lastName}
            </h1>
            <p className="mt-1 text-sm text-gray-500">
              Panel de control - {user?.role}
            </p>
          </div>
        </div>
      </div>

      <div className="px-4 sm:px-6 lg:px-8 py-8">
        {/* Stats */}
        <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
          {statsData.map((stat) => (
            <div key={stat.name} className="bg-white overflow-hidden shadow rounded-lg">
              <div className="p-5">
                <div className="flex items-center">
                  <div className="flex-shrink-0">
                    <div className={`p-3 rounded-md ${stat.color}`}>
                      <stat.icon className="h-6 w-6 text-white" />
                    </div>
                  </div>
                  <div className="ml-5 w-0 flex-1">
                    <dl>
                      <dt className="text-sm font-medium text-gray-500 truncate">
                        {stat.name}
                      </dt>
                      <dd className="text-2xl font-semibold text-gray-900">
                        {stat.value}
                      </dd>
                    </dl>
                  </div>
                </div>
                <div className="mt-3">
                  <span className="text-sm text-gray-500">{stat.change}</span>
                </div>
              </div>
            </div>
          ))}
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Recent Projects */}
          <div className="bg-white shadow rounded-lg">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-medium text-gray-900">Proyectos Recientes</h3>
            </div>
            <div className="p-6">
              <div className="space-y-4">
                {recentProjects.map((project) => (
                  <div key={project.id} className="border rounded-lg p-4">
                    <div className="flex items-center justify-between mb-2">
                      <h4 className="text-sm font-medium text-gray-900">{project.name}</h4>
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(project.status)}`}>
                        {project.status}
                      </span>
                    </div>
                    <div className="mb-3">
                      <div className="flex justify-between text-sm text-gray-600 mb-1">
                        <span>Progreso</span>
                        <span>{project.progress}%</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-2">
                        <div 
                          className="bg-blue-600 h-2 rounded-full" 
                          style={{ width: `${project.progress}%` }}
                        ></div>
                      </div>
                    </div>
                    <div className="flex justify-between text-sm text-gray-500">
                      <span>{project.members} miembros</span>
                      <span>Vence: {project.dueDate}</span>
                    </div>
                  </div>
                ))}
              </div>
              <div className="mt-4">
                <button className="text-blue-600 hover:text-blue-500 text-sm font-medium">
                  Ver todos los proyectos →
                </button>
              </div>
            </div>
          </div>

          {/* Recent Tasks */}
          <div className="bg-white shadow rounded-lg">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-medium text-gray-900">Tareas Recientes</h3>
            </div>
            <div className="p-6">
              <div className="space-y-4">
                {recentTasks.map((task) => (
                  <div key={task.id} className="border rounded-lg p-4">
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <h4 className="text-sm font-medium text-gray-900">{task.title}</h4>
                        <p className="text-xs text-gray-500 mt-1">{task.project}</p>
                      </div>
                      <div className="flex items-center space-x-2 ml-4">
                        <ExclamationTriangleIcon className={`h-4 w-4 ${getPriorityColor(task.priority)}`} />
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(task.status)}`}>
                          {task.status}
                        </span>
                      </div>
                    </div>
                    <div className="mt-2 text-xs text-gray-500">
                      Asignado a: {task.assignee}
                    </div>
                  </div>
                ))}
              </div>
              <div className="mt-4">
                <button className="text-blue-600 hover:text-blue-500 text-sm font-medium">
                  Ver todas las tareas →
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="mt-8 bg-white shadow rounded-lg">
          <div className="px-6 py-4 border-b border-gray-200">
            <h3 className="text-lg font-medium text-gray-900">Acciones Rápidas</h3>
          </div>
          <div className="p-6">
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
              <button className="flex items-center justify-center px-4 py-3 border border-gray-300 rounded-md shadow-sm bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
                <FolderIcon className="h-5 w-5 mr-2 text-gray-400" />
                Nuevo Proyecto
              </button>
              <button className="flex items-center justify-center px-4 py-3 border border-gray-300 rounded-md shadow-sm bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
                <CheckCircleIcon className="h-5 w-5 mr-2 text-gray-400" />
                Nueva Tarea
              </button>
              <button className="flex items-center justify-center px-4 py-3 border border-gray-300 rounded-md shadow-sm bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
                <UserGroupIcon className="h-5 w-5 mr-2 text-gray-400" />
                Invitar Usuario
              </button>
              <button className="flex items-center justify-center px-4 py-3 border border-gray-300 rounded-md shadow-sm bg-white text-sm font-medium text-gray-700 hover:bg-gray-50">
                <ChartBarIcon className="h-5 w-5 mr-2 text-gray-400" />
                Ver Reportes
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DashboardPage;