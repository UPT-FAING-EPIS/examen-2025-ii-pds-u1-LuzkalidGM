import React, { useState } from 'react';
import {
  CheckCircleIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  PlusIcon,
  MagnifyingGlassIcon,
  FunnelIcon,
  UserIcon,
  TagIcon
} from '@heroicons/react/24/outline';

interface Task {
  id: string;
  title: string;
  description: string;
  status: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled';
  priority: 'Low' | 'Medium' | 'High';
  project: string;
  projectId: string;
  assignee: string;
  assigneeId: string;
  createdDate: string;
  dueDate: string;
  estimatedHours: number;
  actualHours?: number;
  tags: string[];
}

const TasksPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filterStatus, setFilterStatus] = useState<string>('all');
  const [filterPriority, setFilterPriority] = useState<string>('all');
  const [filterProject, setFilterProject] = useState<string>('all');
  const [viewMode, setViewMode] = useState<'list' | 'board'>('list');
  const [showCreateModal, setShowCreateModal] = useState(false);

  // Mock data - esto se reemplazaría con datos reales del API
  const tasks: Task[] = [
    {
      id: '1',
      title: 'Implementar autenticación JWT',
      description: 'Desarrollo del sistema de autenticación usando JWT tokens para la API',
      status: 'InProgress',
      priority: 'High',
      project: 'Sistema de Gestión de Proyectos',
      projectId: '1',
      assignee: 'Carlos Sanchez',
      assigneeId: '1',
      createdDate: '2024-01-05',
      dueDate: '2024-01-12',
      estimatedHours: 16,
      actualHours: 12,
      tags: ['backend', 'security', 'api']
    },
    {
      id: '2',
      title: 'Crear componentes React',
      description: 'Desarrollo de componentes reutilizables para el dashboard',
      status: 'Pending',
      priority: 'Medium',
      project: 'Sistema de Gestión de Proyectos',
      projectId: '1',
      assignee: 'Ana Lopez',
      assigneeId: '2',
      createdDate: '2024-01-06',
      dueDate: '2024-01-15',
      estimatedHours: 20,
      tags: ['frontend', 'react', 'ui']
    },
    {
      id: '3',
      title: 'Configurar Terraform',
      description: 'Setup de infraestructura como código para Azure',
      status: 'InProgress',
      priority: 'High',
      project: 'Migración a la Nube',
      projectId: '2',
      assignee: 'Miguel Rodriguez',
      assigneeId: '3',
      createdDate: '2024-01-08',
      dueDate: '2024-01-20',
      estimatedHours: 24,
      actualHours: 8,
      tags: ['infrastructure', 'azure', 'terraform']
    },
    {
      id: '4',
      title: 'Diseño de base de datos',
      description: 'Diseño del esquema de base de datos para analytics',
      status: 'Completed',
      priority: 'Medium',
      project: 'Dashboard Analytics',
      projectId: '3',
      assignee: 'Sofia Gutierrez',
      assigneeId: '4',
      createdDate: '2024-01-03',
      dueDate: '2024-01-10',
      estimatedHours: 12,
      actualHours: 10,
      tags: ['database', 'design', 'analytics']
    },
    {
      id: '5',
      title: 'Implementar gráficos',
      description: 'Desarrollo de componentes de visualización de datos',
      status: 'Pending',
      priority: 'Low',
      project: 'Dashboard Analytics',
      projectId: '3',
      assignee: 'Carlos Sanchez',
      assigneeId: '1',
      createdDate: '2024-01-10',
      dueDate: '2024-01-25',
      estimatedHours: 18,
      tags: ['frontend', 'charts', 'data-viz']
    }
  ];

  const projects = [
    { id: '1', name: 'Sistema de Gestión de Proyectos' },
    { id: '2', name: 'Migración a la Nube' },
    { id: '3', name: 'Dashboard Analytics' }
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed':
        return 'bg-green-100 text-green-800';
      case 'InProgress':
        return 'bg-blue-100 text-blue-800';
      case 'Pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'Cancelled':
        return 'bg-red-100 text-red-800';
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

  const getPriorityIcon = (priority: string) => {
    switch (priority) {
      case 'High':
        return <ExclamationTriangleIcon className="h-4 w-4" />;
      case 'Medium':
        return <ClockIcon className="h-4 w-4" />;
      case 'Low':
        return <CheckCircleIcon className="h-4 w-4" />;
      default:
        return <ClockIcon className="h-4 w-4" />;
    }
  };

  const filteredTasks = tasks.filter(task => {
    const matchesSearch = task.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         task.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         task.tags.some(tag => tag.toLowerCase().includes(searchTerm.toLowerCase()));
    const matchesStatus = filterStatus === 'all' || task.status === filterStatus;
    const matchesPriority = filterPriority === 'all' || task.priority === filterPriority;
    const matchesProject = filterProject === 'all' || task.projectId === filterProject;
    
    return matchesSearch && matchesStatus && matchesPriority && matchesProject;
  });

  const isOverdue = (dueDate: string, status: string) => {
    if (status === 'Completed') return false;
    return new Date(dueDate) < new Date();
  };

  const TaskCard: React.FC<{ task: Task }> = ({ task }) => (
    <div className="bg-white border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow duration-200">
      <div className="flex items-start justify-between mb-3">
        <div className="flex-1">
          <h3 className="text-sm font-medium text-gray-900 mb-1">{task.title}</h3>
          <p className="text-xs text-gray-600 line-clamp-2">{task.description}</p>
        </div>
        <div className="flex items-center ml-4">
          <div className={`${getPriorityColor(task.priority)} mr-2`}>
            {getPriorityIcon(task.priority)}
          </div>
          <span className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(task.status)}`}>
            {task.status}
          </span>
        </div>
      </div>

      <div className="space-y-2 mb-3">
        <div className="flex items-center text-xs text-gray-500">
          <TagIcon className="h-3 w-3 mr-1" />
          <span className="truncate">{task.project}</span>
        </div>
        <div className="flex items-center text-xs text-gray-500">
          <UserIcon className="h-3 w-3 mr-1" />
          <span>{task.assignee}</span>
        </div>
        <div className="flex items-center justify-between text-xs text-gray-500">
          <span>Vence: {task.dueDate}</span>
          {isOverdue(task.dueDate, task.status) && (
            <span className="text-red-600 font-medium">Vencida</span>
          )}
        </div>
      </div>

      <div className="flex flex-wrap gap-1 mb-3">
        {task.tags.slice(0, 3).map((tag, index) => (
          <span key={index} className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800">
            {tag}
          </span>
        ))}
        {task.tags.length > 3 && (
          <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-gray-100 text-gray-800">
            +{task.tags.length - 3}
          </span>
        )}
      </div>

      <div className="flex items-center justify-between text-xs text-gray-500">
        <span>Est: {task.estimatedHours}h</span>
        {task.actualHours && (
          <span>Real: {task.actualHours}h</span>
        )}
      </div>

      <div className="mt-3 flex space-x-2">
        <button className="flex-1 bg-blue-50 text-blue-700 hover:bg-blue-100 px-2 py-1 rounded text-xs font-medium">
          Ver
        </button>
        <button className="flex-1 bg-gray-50 text-gray-700 hover:bg-gray-100 px-2 py-1 rounded text-xs font-medium">
          Editar
        </button>
      </div>
    </div>
  );

  const KanbanColumn: React.FC<{ status: string; tasks: Task[]; title: string }> = ({ status, tasks, title }) => (
    <div className="flex-1 min-w-80">
      <div className="bg-gray-50 rounded-lg p-4">
        <div className="flex items-center justify-between mb-4">
          <h3 className="font-medium text-gray-900">{title}</h3>
          <span className="text-sm text-gray-500">({tasks.filter(t => t.status === status).length})</span>
        </div>
        <div className="space-y-3">
          {tasks.filter(task => task.status === status).map(task => (
            <TaskCard key={task.id} task={task} />
          ))}
        </div>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white shadow">
        <div className="px-4 sm:px-6 lg:px-8">
          <div className="py-6">
            <div className="flex items-center justify-between">
              <div>
                <h1 className="text-2xl font-bold text-gray-900">Tareas</h1>
                <p className="mt-1 text-sm text-gray-500">
                  Gestiona y realiza seguimiento de todas las tareas
                </p>
              </div>
              <div className="flex items-center space-x-3">
                <div className="flex bg-gray-100 rounded-md p-1">
                  <button
                    onClick={() => setViewMode('list')}
                    className={`px-3 py-1 text-sm font-medium rounded ${
                      viewMode === 'list' ? 'bg-white text-gray-900 shadow-sm' : 'text-gray-500'
                    }`}
                  >
                    Lista
                  </button>
                  <button
                    onClick={() => setViewMode('board')}
                    className={`px-3 py-1 text-sm font-medium rounded ${
                      viewMode === 'board' ? 'bg-white text-gray-900 shadow-sm' : 'text-gray-500'
                    }`}
                  >
                    Tablero
                  </button>
                </div>
                <button
                  onClick={() => setShowCreateModal(true)}
                  className="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
                >
                  <PlusIcon className="-ml-1 mr-2 h-5 w-5" />
                  Nueva Tarea
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div className="px-4 sm:px-6 lg:px-8 py-8">
        {/* Filters */}
        <div className="mb-6 flex flex-col lg:flex-row gap-4">
          <div className="relative flex-1">
            <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
              <MagnifyingGlassIcon className="h-5 w-5 text-gray-400" />
            </div>
            <input
              type="text"
              placeholder="Buscar tareas, descripción o tags..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
            />
          </div>
          <div className="flex gap-2">
            <select
              value={filterStatus}
              onChange={(e) => setFilterStatus(e.target.value)}
              className="block w-40 px-3 py-2 border border-gray-300 rounded-md leading-5 bg-white focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="all">Todos los estados</option>
              <option value="Pending">Pendiente</option>
              <option value="InProgress">En progreso</option>
              <option value="Completed">Completada</option>
              <option value="Cancelled">Cancelada</option>
            </select>
            <select
              value={filterPriority}
              onChange={(e) => setFilterPriority(e.target.value)}
              className="block w-40 px-3 py-2 border border-gray-300 rounded-md leading-5 bg-white focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="all">Todas las prioridades</option>
              <option value="High">Alta</option>
              <option value="Medium">Media</option>
              <option value="Low">Baja</option>
            </select>
            <select
              value={filterProject}
              onChange={(e) => setFilterProject(e.target.value)}
              className="block w-48 px-3 py-2 border border-gray-300 rounded-md leading-5 bg-white focus:outline-none focus:ring-1 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="all">Todos los proyectos</option>
              {projects.map(project => (
                <option key={project.id} value={project.id}>{project.name}</option>
              ))}
            </select>
          </div>
        </div>

        {/* Content */}
        {viewMode === 'list' ? (
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
            {filteredTasks.map((task) => (
              <TaskCard key={task.id} task={task} />
            ))}
          </div>
        ) : (
          <div className="flex gap-6 overflow-x-auto pb-4">
            <KanbanColumn status="Pending" tasks={filteredTasks} title="Pendientes" />
            <KanbanColumn status="InProgress" tasks={filteredTasks} title="En Progreso" />
            <KanbanColumn status="Completed" tasks={filteredTasks} title="Completadas" />
            <KanbanColumn status="Cancelled" tasks={filteredTasks} title="Canceladas" />
          </div>
        )}

        {filteredTasks.length === 0 && (
          <div className="text-center py-12">
            <CheckCircleIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No se encontraron tareas</h3>
            <p className="mt-1 text-sm text-gray-500">
              Intenta ajustar los filtros o crear una nueva tarea.
            </p>
          </div>
        )}
      </div>

      {/* Create Task Modal - Placeholder */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
          <div className="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white">
            <div className="mt-3">
              <h3 className="text-lg font-medium text-gray-900 text-center">
                Crear Nueva Tarea
              </h3>
              <div className="mt-4 text-sm text-gray-500 text-center">
                Modal de creación de tarea - Implementar formulario completo
              </div>
              <div className="items-center px-4 py-3 mt-4">
                <button
                  onClick={() => setShowCreateModal(false)}
                  className="px-4 py-2 bg-gray-500 text-white text-base font-medium rounded-md w-full shadow-sm hover:bg-gray-600"
                >
                  Cerrar
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default TasksPage;