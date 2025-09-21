import React, { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';
import { User, Project, Task, UserRole, ProjectStatus, TaskStatus } from '../types';
import { userService, projectService, taskService } from '../services/api';
import ProjectForm from './ProjectForm';
import TaskForm from './TaskForm';

const AdminPanel: React.FC = () => {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<'overview' | 'users' | 'projects' | 'tasks'>('overview');
  const [users, setUsers] = useState<User[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [showProjectForm, setShowProjectForm] = useState(false);
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [editingProject, setEditingProject] = useState<Project | undefined>();
  const [editingTask, setEditingTask] = useState<Task | undefined>();

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [usersData, projectsData, tasksData] = await Promise.all([
        userService.getUsers(),
        projectService.getProjects(),
        taskService.getTasks()
      ]);
      setUsers(usersData);
      setProjects(projectsData);
      setTasks(tasksData);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleProjectSave = (project: Project) => {
    if (editingProject) {
      setProjects(projects.map(p => p.id === project.id ? project : p));
    } else {
      setProjects([...projects, project]);
    }
    setShowProjectForm(false);
    setEditingProject(undefined);
  };

  const handleTaskSave = (task: Task) => {
    if (editingTask) {
      setTasks(tasks.map(t => t.id === task.id ? task : t));
    } else {
      setTasks([...tasks, task]);
    }
    setShowTaskForm(false);
    setEditingTask(undefined);
  };

  const deleteProject = async (projectId: number) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar este proyecto?')) {
      try {
        await projectService.deleteProject(projectId);
        setProjects(projects.filter(p => p.id !== projectId));
      } catch (error) {
        console.error('Error deleting project:', error);
      }
    }
  };

  const deleteTask = async (taskId: number) => {
    if (window.confirm('¿Estás seguro de que quieres eliminar esta tarea?')) {
      try {
        await taskService.deleteTask(taskId);
        setTasks(tasks.filter(t => t.id !== taskId));
      } catch (error) {
        console.error('Error deleting task:', error);
      }
    }
  };

  const getOverviewStats = () => {
    const activeUsers = users.filter(u => u.isActive).length;
    const activeProjects = projects.filter(p => p.status === ProjectStatus.InProgress).length;
    const pendingTasks = tasks.filter(t => t.status === TaskStatus.Pending).length;
    const completedTasks = tasks.filter(t => t.status === TaskStatus.Completed).length;
    const totalTasks = tasks.length;
    const taskCompletionRate = totalTasks > 0 ? Math.round((completedTasks / totalTasks) * 100) : 0;

    return {
      totalUsers: users.length,
      activeUsers,
      totalProjects: projects.length,
      activeProjects,
      totalTasks,
      pendingTasks,
      completedTasks,
      taskCompletionRate
    };
  };

  const getRoleText = (role: UserRole) => {
    switch (role) {
      case UserRole.Admin: return 'Administrador';
      case UserRole.ProjectManager: return 'Gestor de Proyectos';
      case UserRole.User: return 'Usuario';
      default: return 'Desconocido';
    }
  };

  const getStatusText = (status: number, type: 'project' | 'task') => {
    if (type === 'project') {
      switch (status) {
        case ProjectStatus.Planning: return 'Planificación';
        case ProjectStatus.InProgress: return 'En Progreso';
        case ProjectStatus.Completed: return 'Completado';
        case ProjectStatus.Cancelled: return 'Cancelado';
        case ProjectStatus.OnHold: return 'En Pausa';
        default: return 'Desconocido';
      }
    } else {
      switch (status) {
        case TaskStatus.Pending: return 'Pendiente';
        case TaskStatus.InProgress: return 'En Progreso';
        case TaskStatus.Completed: return 'Completado';
        case TaskStatus.Cancelled: return 'Cancelado';
        default: return 'Desconocido';
      }
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Verificar si el usuario es admin
  if (user?.role !== UserRole.Admin) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <span className="text-6xl mb-4 block">🚫</span>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">Acceso Denegado</h2>
          <p className="text-gray-600">No tienes permisos para acceder al panel de administración.</p>
        </div>
      </div>
    );
  }

  const stats = getOverviewStats();

  if (showProjectForm) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <ProjectForm
          project={editingProject}
          onSave={handleProjectSave}
          onCancel={() => {
            setShowProjectForm(false);
            setEditingProject(undefined);
          }}
        />
      </div>
    );
  }

  if (showTaskForm) {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <TaskForm
          task={editingTask}
          onSave={handleTaskSave}
          onCancel={() => {
            setShowTaskForm(false);
            setEditingTask(undefined);
          }}
        />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">
                ⚙️ Panel de Administración
              </h1>
              <p className="text-sm text-gray-600">
                Gestión global del sistema - {user?.fullName}
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Navigation Tabs */}
      <div className="bg-white border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <nav className="flex space-x-8">
            {[
              { key: 'overview', label: '📊 Resumen', icon: '📊' },
              { key: 'users', label: '👥 Usuarios', icon: '👥' },
              { key: 'projects', label: '📁 Proyectos', icon: '📁' },
              { key: 'tasks', label: '📝 Tareas', icon: '📝' }
            ].map(tab => (
              <button
                key={tab.key}
                onClick={() => setActiveTab(tab.key as any)}
                className={`py-4 px-1 border-b-2 font-medium text-sm ${
                  activeTab === tab.key
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                {tab.label}
              </button>
            ))}
          </nav>
        </div>
      </div>

      {/* Content */}
      <div className="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        {activeTab === 'overview' && (
          <div className="space-y-6">
            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
              <div className="bg-white p-6 rounded-lg shadow">
                <div className="flex items-center">
                  <div className="p-3 bg-blue-100 rounded-full">
                    <span className="text-2xl">👥</span>
                  </div>
                  <div className="ml-4">
                    <p className="text-sm font-medium text-gray-500">Usuarios Totales</p>
                    <p className="text-2xl font-semibold text-gray-900">{stats.totalUsers}</p>
                    <p className="text-xs text-green-600">{stats.activeUsers} activos</p>
                  </div>
                </div>
              </div>

              <div className="bg-white p-6 rounded-lg shadow">
                <div className="flex items-center">
                  <div className="p-3 bg-green-100 rounded-full">
                    <span className="text-2xl">📁</span>
                  </div>
                  <div className="ml-4">
                    <p className="text-sm font-medium text-gray-500">Proyectos</p>
                    <p className="text-2xl font-semibold text-gray-900">{stats.totalProjects}</p>
                    <p className="text-xs text-blue-600">{stats.activeProjects} activos</p>
                  </div>
                </div>
              </div>

              <div className="bg-white p-6 rounded-lg shadow">
                <div className="flex items-center">
                  <div className="p-3 bg-yellow-100 rounded-full">
                    <span className="text-2xl">📝</span>
                  </div>
                  <div className="ml-4">
                    <p className="text-sm font-medium text-gray-500">Tareas Totales</p>
                    <p className="text-2xl font-semibold text-gray-900">{stats.totalTasks}</p>
                    <p className="text-xs text-orange-600">{stats.pendingTasks} pendientes</p>
                  </div>
                </div>
              </div>

              <div className="bg-white p-6 rounded-lg shadow">
                <div className="flex items-center">
                  <div className="p-3 bg-purple-100 rounded-full">
                    <span className="text-2xl">📈</span>
                  </div>
                  <div className="ml-4">
                    <p className="text-sm font-medium text-gray-500">Tasa Completado</p>
                    <p className="text-2xl font-semibold text-gray-900">{stats.taskCompletionRate}%</p>
                    <p className="text-xs text-green-600">{stats.completedTasks} completadas</p>
                  </div>
                </div>
              </div>
            </div>

            {/* Recent Activity */}
            <div className="bg-white rounded-lg shadow p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">📋 Actividad Reciente</h3>
              <div className="space-y-4">
                {projects.slice(0, 5).map(project => (
                  <div key={project.id} className="flex items-center justify-between border-b pb-2">
                    <div>
                      <p className="font-medium">{project.name}</p>
                      <p className="text-sm text-gray-500">
                        Progreso: {Math.round(project.progressPercentage)}% - {getStatusText(project.status, 'project')}
                      </p>
                    </div>
                    <div className="text-sm text-gray-400">
                      {new Date(project.updatedAt).toLocaleDateString()}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        {activeTab === 'users' && (
          <div className="bg-white rounded-lg shadow">
            <div className="p-6 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">👥 Gestión de Usuarios</h3>
            </div>
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Usuario
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Email
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Rol
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Estado
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Creado
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {users.map(user => (
                    <tr key={user.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div>
                          <div className="text-sm font-medium text-gray-900">{user.fullName}</div>
                          <div className="text-sm text-gray-500">@{user.username}</div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {user.email}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                          user.role === UserRole.Admin ? 'bg-red-100 text-red-800' :
                          user.role === UserRole.ProjectManager ? 'bg-blue-100 text-blue-800' :
                          'bg-green-100 text-green-800'
                        }`}>
                          {getRoleText(user.role)}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                          user.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                        }`}>
                          {user.isActive ? 'Activo' : 'Inactivo'}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {new Date(user.createdAt).toLocaleDateString()}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {activeTab === 'projects' && (
          <div className="space-y-6">
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-semibold text-gray-900">📁 Gestión de Proyectos</h3>
              <button
                onClick={() => setShowProjectForm(true)}
                className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 transition-colors"
              >
                ➕ Nuevo Proyecto
              </button>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {projects.map(project => (
                <div key={project.id} className="bg-white rounded-lg shadow p-6">
                  <div className="flex justify-between items-start mb-4">
                    <h4 className="text-lg font-semibold text-gray-900">{project.name}</h4>
                    <div className="flex space-x-2">
                      <button
                        onClick={() => {
                          setEditingProject(project);
                          setShowProjectForm(true);
                        }}
                        className="text-blue-600 hover:text-blue-800"
                      >
                        ✏️
                      </button>
                      <button
                        onClick={() => deleteProject(project.id)}
                        className="text-red-600 hover:text-red-800"
                      >
                        🗑️
                      </button>
                    </div>
                  </div>
                  
                  <p className="text-gray-600 mb-4">{project.description}</p>
                  
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-500">Estado:</span>
                      <span className={`px-2 py-1 rounded text-xs font-medium ${
                        project.status === ProjectStatus.Completed ? 'bg-green-100 text-green-800' :
                        project.status === ProjectStatus.InProgress ? 'bg-blue-100 text-blue-800' :
                        project.status === ProjectStatus.Planning ? 'bg-yellow-100 text-yellow-800' :
                        'bg-red-100 text-red-800'
                      }`}>
                        {getStatusText(project.status, 'project')}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-500">Responsable:</span>
                      <span>{project.responsible?.fullName}</span>
                    </div>
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
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {activeTab === 'tasks' && (
          <div className="space-y-6">
            <div className="flex justify-between items-center">
              <h3 className="text-lg font-semibold text-gray-900">📝 Gestión de Tareas</h3>
              <button
                onClick={() => setShowTaskForm(true)}
                className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700 transition-colors"
              >
                ➕ Nueva Tarea
              </button>
            </div>

            <div className="bg-white rounded-lg shadow">
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                  <thead className="bg-gray-50">
                    <tr>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Tarea
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Proyecto
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Asignado a
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Estado
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Prioridad
                      </th>
                      <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                        Acciones
                      </th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-gray-200">
                    {tasks.map(task => (
                      <tr key={task.id} className="hover:bg-gray-50">
                        <td className="px-6 py-4">
                          <div>
                            <div className="text-sm font-medium text-gray-900">{task.title}</div>
                            <div className="text-sm text-gray-500">{task.description.substring(0, 50)}...</div>
                          </div>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {task.project?.name || 'N/A'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                          {task.assignedTo?.fullName || 'Sin asignar'}
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                            task.status === TaskStatus.Completed ? 'bg-green-100 text-green-800' :
                            task.status === TaskStatus.InProgress ? 'bg-blue-100 text-blue-800' :
                            task.status === TaskStatus.Pending ? 'bg-yellow-100 text-yellow-800' :
                            'bg-red-100 text-red-800'
                          }`}>
                            {getStatusText(task.status, 'task')}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap">
                          <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                            task.priority === 3 ? 'bg-red-100 text-red-800' :
                            task.priority === 2 ? 'bg-orange-100 text-orange-800' :
                            task.priority === 1 ? 'bg-yellow-100 text-yellow-800' :
                            'bg-green-100 text-green-800'
                          }`}>
                            {task.priority === 3 ? 'Crítica' : task.priority === 2 ? 'Alta' : task.priority === 1 ? 'Media' : 'Baja'}
                          </span>
                        </td>
                        <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                          <div className="flex space-x-2">
                            <button
                              onClick={() => {
                                setEditingTask(task);
                                setShowTaskForm(true);
                              }}
                              className="text-blue-600 hover:text-blue-800"
                            >
                              ✏️
                            </button>
                            <button
                              onClick={() => deleteTask(task.id)}
                              className="text-red-600 hover:text-red-800"
                            >
                              🗑️
                            </button>
                          </div>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminPanel;