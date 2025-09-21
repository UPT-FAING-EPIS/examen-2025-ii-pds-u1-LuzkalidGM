import React, { useState, useEffect } from 'react';
import { useAuth } from './AuthContext';
import { Project, Task, UserRole } from '../types';
import { projectService, taskService } from '../services/api';
import AdminPanel from './AdminPanel';
import UserPanel from './UserPanel';
import ProjectForm from './ProjectForm';
import TaskForm from './TaskForm';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const [activeView, setActiveView] = useState<'dashboard' | 'admin' | 'user' | 'project-form' | 'task-form'>('dashboard');
  const [projects, setProjects] = useState<Project[]>([]);
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'projects' | 'tasks'>('projects');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [projectsData, tasksData] = await Promise.all([
        projectService.getProjects(),
        taskService.getTasks()
      ]);
      setProjects(projectsData);
      setTasks(tasksData);
    } catch (error) {
      console.error('Error loading data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleProjectSave = (project: Project) => {
    setProjects([...projects, project]);
    setActiveView('dashboard');
  };

  const handleTaskSave = (task: Task) => {
    setTasks([...tasks, task]);
    setActiveView('dashboard');
  };

  // Si está en vista de administración
  if (activeView === 'admin') {
    return <AdminPanel />;
  }

  // Si está en vista de usuario personal
  if (activeView === 'user') {
    return <UserPanel />;
  }

  // Si está creando un proyecto
  if (activeView === 'project-form') {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <ProjectForm
          onSave={handleProjectSave}
          onCancel={() => setActiveView('dashboard')}
        />
      </div>
    );
  }

  // Si está creando una tarea
  if (activeView === 'task-form') {
    return (
      <div className="min-h-screen bg-gray-50 p-6">
        <TaskForm
          onSave={handleTaskSave}
          onCancel={() => setActiveView('dashboard')}
        />
      </div>
    );
  }

  const getStatusColor = (status: number, type: 'project' | 'task') => {
    if (type === 'project') {
      switch (status) {
        case 0: return 'bg-yellow-100 text-yellow-800'; // Planning
        case 1: return 'bg-blue-100 text-blue-800'; // InProgress
        case 2: return 'bg-green-100 text-green-800'; // Completed
        case 3: return 'bg-red-100 text-red-800'; // Cancelled
        case 4: return 'bg-gray-100 text-gray-800'; // OnHold
        default: return 'bg-gray-100 text-gray-800';
      }
    } else {
      switch (status) {
        case 0: return 'bg-yellow-100 text-yellow-800'; // Pending
        case 1: return 'bg-blue-100 text-blue-800'; // InProgress
        case 2: return 'bg-green-100 text-green-800'; // Completed
        case 3: return 'bg-red-100 text-red-800'; // Cancelled
        default: return 'bg-gray-100 text-gray-800';
      }
    }
  };

  const getStatusText = (status: number, type: 'project' | 'task') => {
    if (type === 'project') {
      switch (status) {
        case 0: return 'Planificación';
        case 1: return 'En Progreso';
        case 2: return 'Completado';
        case 3: return 'Cancelado';
        case 4: return 'En Pausa';
        default: return 'Desconocido';
      }
    } else {
      switch (status) {
        case 0: return 'Pendiente';
        case 1: return 'En Progreso';
        case 2: return 'Completado';
        case 3: return 'Cancelado';
        default: return 'Desconocido';
      }
    }
  };

  const getPriorityColor = (priority: number) => {
    switch (priority) {
      case 0: return 'bg-green-100 text-green-800'; // Low
      case 1: return 'bg-yellow-100 text-yellow-800'; // Medium
      case 2: return 'bg-orange-100 text-orange-800'; // High
      case 3: return 'bg-red-100 text-red-800'; // Critical
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getPriorityText = (priority: number) => {
    switch (priority) {
      case 0: return 'Baja';
      case 1: return 'Media';
      case 2: return 'Alta';
      case 3: return 'Crítica';
      default: return 'Desconocida';
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <div>
              <h1 className="text-3xl font-bold text-gray-900">
                🎯 Project Management System
              </h1>
              <p className="text-sm text-gray-600">
                Bienvenido, {user?.fullName}
              </p>
            </div>
            <div className="flex items-center space-x-4">
              {/* Navigation Buttons */}
              <button
                onClick={() => setActiveView('user')}
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                👤 Mi Panel
              </button>
              
              {user?.role === UserRole.Admin && (
                <button
                  onClick={() => setActiveView('admin')}
                  className="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md text-sm font-medium"
                >
                  ⚙️ Admin
                </button>
              )}
              
              <button
                onClick={logout}
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md text-sm font-medium"
              >
                Cerrar Sesión
              </button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        {/* Quick Actions */}
        <div className="mb-8 bg-white rounded-lg shadow p-6">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">🚀 Acciones Rápidas</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <button
              onClick={() => setActiveView('project-form')}
              className="flex items-center justify-center p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
            >
              <span className="text-2xl mr-2">📁</span>
              <span className="font-medium text-blue-700">Nuevo Proyecto</span>
            </button>
            
            <button
              onClick={() => setActiveView('task-form')}
              className="flex items-center justify-center p-4 bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
            >
              <span className="text-2xl mr-2">📝</span>
              <span className="font-medium text-green-700">Nueva Tarea</span>
            </button>
            
            <button
              onClick={() => setActiveView('user')}
              className="flex items-center justify-center p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
            >
              <span className="text-2xl mr-2">👤</span>
              <span className="font-medium text-purple-700">Mis Tareas</span>
            </button>
            
            {user?.role === UserRole.Admin && (
              <button
                onClick={() => setActiveView('admin')}
                className="flex items-center justify-center p-4 bg-orange-50 rounded-lg hover:bg-orange-100 transition-colors"
              >
                <span className="text-2xl mr-2">⚙️</span>
                <span className="font-medium text-orange-700">Administrar</span>
              </button>
            )}
          </div>
        </div>

        {/* Stats */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white overflow-hidden shadow rounded-lg">
            <div className="p-5">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <div className="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                    <span className="text-white font-bold">📁</span>
                  </div>
                </div>
                <div className="ml-5 w-0 flex-1">
                  <dl>
                    <dt className="text-sm font-medium text-gray-500 truncate">
                      Total Proyectos
                    </dt>
                    <dd className="text-lg font-medium text-gray-900">
                      {projects.length}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white overflow-hidden shadow rounded-lg">
            <div className="p-5">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <div className="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                    <span className="text-white font-bold">✅</span>
                  </div>
                </div>
                <div className="ml-5 w-0 flex-1">
                  <dl>
                    <dt className="text-sm font-medium text-gray-500 truncate">
                      Total Tareas
                    </dt>
                    <dd className="text-lg font-medium text-gray-900">
                      {tasks.length}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white overflow-hidden shadow rounded-lg">
            <div className="p-5">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <div className="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                    <span className="text-white font-bold">⏳</span>
                  </div>
                </div>
                <div className="ml-5 w-0 flex-1">
                  <dl>
                    <dt className="text-sm font-medium text-gray-500 truncate">
                      Tareas Pendientes
                    </dt>
                    <dd className="text-lg font-medium text-gray-900">
                      {tasks.filter(t => t.status === 0 || t.status === 1).length}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Tabs */}
        <div className="bg-white shadow rounded-lg">
          <div className="border-b border-gray-200">
            <nav className="-mb-px flex space-x-8" aria-label="Tabs">
              <button
                onClick={() => setActiveTab('projects')}
                className={`py-2 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'projects'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                📁 Proyectos Recientes
              </button>
              <button
                onClick={() => setActiveTab('tasks')}
                className={`py-2 px-1 border-b-2 font-medium text-sm ${
                  activeTab === 'tasks'
                    ? 'border-blue-500 text-blue-600'
                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }`}
              >
                ✅ Tareas Recientes
              </button>
            </nav>
          </div>

          <div className="p-6">
            {activeTab === 'projects' ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {projects.slice(0, 6).map((project) => (
                  <div key={project.id} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                    <div className="flex justify-between items-start mb-2">
                      <h3 className="font-semibold text-lg">{project.name}</h3>
                      <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(project.status, 'project')}`}>
                        {getStatusText(project.status, 'project')}
                      </span>
                    </div>
                    <p className="text-gray-600 text-sm mb-3">{project.description}</p>
                    <div className="space-y-2 text-sm">
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
                {projects.length === 0 && (
                  <div className="col-span-full text-center py-8 text-gray-500">
                    <span className="text-4xl mb-4 block">📁</span>
                    No hay proyectos disponibles
                    <br />
                    <button
                      onClick={() => setActiveView('project-form')}
                      className="mt-4 text-blue-600 hover:text-blue-800 font-medium"
                    >
                      Crear el primer proyecto
                    </button>
                  </div>
                )}
              </div>
            ) : (
              <div className="space-y-4">
                {tasks.slice(0, 10).map((task) => (
                  <div key={task.id} className="border rounded-lg p-4 hover:shadow-md transition-shadow">
                    <div className="flex justify-between items-start mb-2">
                      <h3 className="font-semibold">{task.title}</h3>
                      <div className="flex space-x-2">
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${getPriorityColor(task.priority)}`}>
                          {getPriorityText(task.priority)}
                        </span>
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(task.status, 'task')}`}>
                          {getStatusText(task.status, 'task')}
                        </span>
                      </div>
                    </div>
                    <p className="text-gray-600 text-sm mb-3">{task.description}</p>
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      <div>
                        <span className="text-gray-500">Asignado a:</span>
                        <span className="ml-1">{task.assignedTo?.fullName || 'Sin asignar'}</span>
                      </div>
                      <div>
                        <span className="text-gray-500">Proyecto:</span>
                        <span className="ml-1">{task.project?.name || 'N/A'}</span>
                      </div>
                      {task.dueDate && (
                        <div>
                          <span className="text-gray-500">Vencimiento:</span>
                          <span className="ml-1">{new Date(task.dueDate).toLocaleDateString()}</span>
                        </div>
                      )}
                    </div>
                  </div>
                ))}
                {tasks.length === 0 && (
                  <div className="text-center py-8 text-gray-500">
                    <span className="text-4xl mb-4 block">📝</span>
                    No hay tareas disponibles
                    <br />
                    <button
                      onClick={() => setActiveView('task-form')}
                      className="mt-4 text-blue-600 hover:text-blue-800 font-medium"
                    >
                      Crear la primera tarea
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default Dashboard;