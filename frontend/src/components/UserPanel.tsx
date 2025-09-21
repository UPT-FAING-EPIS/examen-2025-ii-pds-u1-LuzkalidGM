import React, { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';
import { Task, Project, TaskStatus, TaskPriority } from '../types';
import { taskService, projectService } from '../services/api';

const UserPanel: React.FC = () => {
  const { user } = useAuth();
  const [tasks, setTasks] = useState<Task[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'pending' | 'inprogress' | 'completed'>('all');

  useEffect(() => {
    if (user) {
      loadUserData();
    }
  }, [user]);

  const loadUserData = async () => {
    try {
      setLoading(true);
      const [allTasks, allProjects] = await Promise.all([
        taskService.getTasks(),
        projectService.getProjects()
      ]);

      // Filtrar tareas asignadas al usuario actual
      const userTasks = allTasks.filter(task => 
        task.assignedTo?.id === user?.id || task.assignedToId === user?.id
      );

      // Filtrar proyectos donde el usuario es responsable
      const userProjects = allProjects.filter(project => 
        project.responsible?.id === user?.id || project.managerId === user?.id
      );

      setTasks(userTasks);
      setProjects(userProjects);
    } catch (error) {
      console.error('Error loading user data:', error);
    } finally {
      setLoading(false);
    }
  };

  const updateTaskStatus = async (taskId: number, newStatus: TaskStatus) => {
    try {
      const task = tasks.find(t => t.id === taskId);
      if (!task) return;

      const updatedTask = await taskService.updateTask(taskId, {
        ...task,
        status: newStatus,
        assignedToId: task.assignedToId || task.assignedTo?.id || 0
      });

      setTasks(tasks.map(t => t.id === taskId ? updatedTask : t));
    } catch (error) {
      console.error('Error updating task status:', error);
    }
  };

  const getFilteredTasks = () => {
    switch (filter) {
      case 'pending':
        return tasks.filter(task => task.status === TaskStatus.Pending);
      case 'inprogress':
        return tasks.filter(task => task.status === TaskStatus.InProgress);
      case 'completed':
        return tasks.filter(task => task.status === TaskStatus.Completed);
      default:
        return tasks;
    }
  };

  const getStatusIcon = (status: TaskStatus) => {
    switch (status) {
      case TaskStatus.Pending: return '⏳';
      case TaskStatus.InProgress: return '🔄';
      case TaskStatus.Completed: return '✅';
      case TaskStatus.Cancelled: return '❌';
      default: return '❓';
    }
  };

  const getPriorityIcon = (priority: TaskPriority) => {
    switch (priority) {
      case TaskPriority.Low: return '🟢';
      case TaskPriority.Medium: return '🟡';
      case TaskPriority.High: return '🟠';
      case TaskPriority.Critical: return '🔴';
      default: return '⚪';
    }
  };

  const getStatusColor = (status: TaskStatus) => {
    switch (status) {
      case TaskStatus.Pending: return 'bg-yellow-100 text-yellow-800';
      case TaskStatus.InProgress: return 'bg-blue-100 text-blue-800';
      case TaskStatus.Completed: return 'bg-green-100 text-green-800';
      case TaskStatus.Cancelled: return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityColor = (priority: TaskPriority) => {
    switch (priority) {
      case TaskPriority.Low: return 'bg-green-100 text-green-800';
      case TaskPriority.Medium: return 'bg-yellow-100 text-yellow-800';
      case TaskPriority.High: return 'bg-orange-100 text-orange-800';
      case TaskPriority.Critical: return 'bg-red-100 text-red-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getTaskStats = () => {
    const pending = tasks.filter(t => t.status === TaskStatus.Pending).length;
    const inProgress = tasks.filter(t => t.status === TaskStatus.InProgress).length;
    const completed = tasks.filter(t => t.status === TaskStatus.Completed).length;
    const total = tasks.length;
    const completionRate = total > 0 ? Math.round((completed / total) * 100) : 0;

    return { pending, inProgress, completed, total, completionRate };
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  const stats = getTaskStats();
  const filteredTasks = getFilteredTasks();

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-2">
            👤 Mi Panel Personal
          </h1>
          <p className="text-gray-600">
            Bienvenido, {user?.fullName}. Aquí puedes ver y gestionar tus tareas asignadas.
          </p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center">
              <div className="p-3 bg-blue-100 rounded-full">
                <span className="text-2xl">📋</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-500">Total Tareas</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.total}</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center">
              <div className="p-3 bg-yellow-100 rounded-full">
                <span className="text-2xl">⏳</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-500">Pendientes</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.pending}</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center">
              <div className="p-3 bg-blue-100 rounded-full">
                <span className="text-2xl">🔄</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-500">En Progreso</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.inProgress}</p>
              </div>
            </div>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center">
              <div className="p-3 bg-green-100 rounded-full">
                <span className="text-2xl">✅</span>
              </div>
              <div className="ml-4">
                <p className="text-sm font-medium text-gray-500">Completadas</p>
                <p className="text-2xl font-semibold text-gray-900">{stats.completed}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Progress Bar */}
        <div className="bg-white p-6 rounded-lg shadow mb-8">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">📈 Progreso General</h3>
          <div className="flex items-center">
            <div className="flex-1 bg-gray-200 rounded-full h-4">
              <div 
                className="bg-green-600 h-4 rounded-full transition-all duration-300" 
                style={{ width: `${stats.completionRate}%` }}
              ></div>
            </div>
            <span className="ml-4 text-sm font-medium text-gray-700">
              {stats.completionRate}% completado
            </span>
          </div>
        </div>

        {/* Filters */}
        <div className="bg-white p-6 rounded-lg shadow mb-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">🔍 Filtros</h3>
          <div className="flex flex-wrap gap-2">
            {[
              { key: 'all', label: 'Todas', count: stats.total },
              { key: 'pending', label: 'Pendientes', count: stats.pending },
              { key: 'inprogress', label: 'En Progreso', count: stats.inProgress },
              { key: 'completed', label: 'Completadas', count: stats.completed }
            ].map(filterOption => (
              <button
                key={filterOption.key}
                onClick={() => setFilter(filterOption.key as any)}
                className={`px-4 py-2 rounded-md text-sm font-medium transition-colors ${
                  filter === filterOption.key
                    ? 'bg-blue-600 text-white'
                    : 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                }`}
              >
                {filterOption.label} ({filterOption.count})
              </button>
            ))}
          </div>
        </div>

        {/* Tasks List */}
        <div className="bg-white rounded-lg shadow">
          <div className="p-6 border-b border-gray-200">
            <h3 className="text-lg font-semibold text-gray-900">
              📝 Mis Tareas ({filteredTasks.length})
            </h3>
          </div>
          
          {filteredTasks.length === 0 ? (
            <div className="p-8 text-center text-gray-500">
              <span className="text-4xl mb-4 block">📭</span>
              No tienes tareas {filter === 'all' ? 'asignadas' : filter === 'pending' ? 'pendientes' : filter === 'inprogress' ? 'en progreso' : 'completadas'}.
            </div>
          ) : (
            <div className="divide-y divide-gray-200">
              {filteredTasks.map((task) => (
                <div key={task.id} className="p-6 hover:bg-gray-50 transition-colors">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3 mb-2">
                        <h4 className="text-lg font-semibold text-gray-900">{task.title}</h4>
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(task.status)}`}>
                          {getStatusIcon(task.status)} {task.status === TaskStatus.Pending ? 'Pendiente' : 
                                                        task.status === TaskStatus.InProgress ? 'En Progreso' :
                                                        task.status === TaskStatus.Completed ? 'Completada' : 'Cancelada'}
                        </span>
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${getPriorityColor(task.priority)}`}>
                          {getPriorityIcon(task.priority)} {task.priority === TaskPriority.Low ? 'Baja' :
                                                            task.priority === TaskPriority.Medium ? 'Media' :
                                                            task.priority === TaskPriority.High ? 'Alta' : 'Crítica'}
                        </span>
                      </div>
                      
                      <p className="text-gray-600 mb-3">{task.description}</p>
                      
                      <div className="grid grid-cols-2 gap-4 text-sm text-gray-500">
                        <div>
                          <strong>Proyecto:</strong> {task.project?.name || 'N/A'}
                        </div>
                        {task.dueDate && (
                          <div>
                            <strong>Vencimiento:</strong> {new Date(task.dueDate).toLocaleDateString()}
                          </div>
                        )}
                        {task.estimatedHours && (
                          <div>
                            <strong>Horas Estimadas:</strong> {task.estimatedHours}h
                          </div>
                        )}
                        <div>
                          <strong>Creado:</strong> {new Date(task.createdAt).toLocaleDateString()}
                        </div>
                      </div>
                    </div>
                    
                    {/* Actions */}
                    <div className="ml-6 flex flex-col gap-2">
                      {task.status === TaskStatus.Pending && (
                        <button
                          onClick={() => updateTaskStatus(task.id, TaskStatus.InProgress)}
                          className="px-3 py-1 bg-blue-600 text-white rounded text-sm hover:bg-blue-700 transition-colors"
                        >
                          🔄 Iniciar
                        </button>
                      )}
                      {task.status === TaskStatus.InProgress && (
                        <button
                          onClick={() => updateTaskStatus(task.id, TaskStatus.Completed)}
                          className="px-3 py-1 bg-green-600 text-white rounded text-sm hover:bg-green-700 transition-colors"
                        >
                          ✅ Completar
                        </button>
                      )}
                      {(task.status === TaskStatus.Pending || task.status === TaskStatus.InProgress) && (
                        <button
                          onClick={() => updateTaskStatus(task.id, TaskStatus.Cancelled)}
                          className="px-3 py-1 bg-red-600 text-white rounded text-sm hover:bg-red-700 transition-colors"
                        >
                          ❌ Cancelar
                        </button>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {/* Projects Section */}
        {projects.length > 0 && (
          <div className="mt-8 bg-white rounded-lg shadow">
            <div className="p-6 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">
                📁 Proyectos que Gestiono ({projects.length})
              </h3>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 p-6">
              {projects.map((project) => (
                <div key={project.id} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                  <h4 className="font-semibold text-lg mb-2">{project.name}</h4>
                  <p className="text-gray-600 text-sm mb-3">{project.description}</p>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Progreso:</span>
                      <span>{Math.round(project.progressPercentage)}%</span>
                    </div>
                    <div className="w-full bg-gray-200 rounded-full h-2">
                      <div 
                        className="bg-blue-600 h-2 rounded-full" 
                        style={{ width: `${project.progressPercentage}%` }}
                      ></div>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Tareas:</span>
                      <span>{project.completedTasks}/{project.totalTasks}</span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default UserPanel;