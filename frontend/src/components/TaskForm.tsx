import React, { useState, useEffect } from 'react';
import { Task, User, Project, CreateTaskDto, UpdateTaskDto, TaskStatus, Priority } from '../types';
import { taskService, userService, projectService } from '../services/api';

interface TaskFormProps {
  task?: Task;
  projectId?: number;
  onSave: (task: Task) => void;
  onCancel: () => void;
}

const TaskForm: React.FC<TaskFormProps> = ({ task, projectId, onSave, onCancel }) => {
  const [formData, setFormData] = useState({
    title: task?.title || '',
    description: task?.description || '',
    projectId: task?.projectId || projectId || 0,
    assignedToId: task?.assignedToId || 0,
    priority: task?.priority || Priority.Medium,
    status: task?.status || TaskStatus.Pending,
    dueDate: task?.dueDate?.split('T')[0] || '',
    estimatedHours: task?.estimatedHours || 0
  });
  const [users, setUsers] = useState<User[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<{ [key: string]: string }>({});

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [usersResponse, projectsResponse] = await Promise.all([
        userService.getUsers(),
        projectService.getProjects()
      ]);
      setUsers(usersResponse);
      setProjects(projectsResponse);
    } catch (error) {
      console.error('Error loading data:', error);
    }
  };

  const validateForm = (): boolean => {
    const newErrors: { [key: string]: string } = {};

    if (!formData.title.trim()) {
      newErrors.title = 'El título de la tarea es requerido';
    }

    if (!formData.description.trim()) {
      newErrors.description = 'La descripción es requerida';
    }

    if (!formData.projectId) {
      newErrors.projectId = 'Debe seleccionar un proyecto';
    }

    if (!formData.assignedToId) {
      newErrors.assignedToId = 'Debe asignar la tarea a un usuario';
    }

    if (formData.estimatedHours < 0) {
      newErrors.estimatedHours = 'Las horas estimadas no pueden ser negativas';
    }

    if (formData.dueDate && new Date(formData.dueDate) < new Date()) {
      newErrors.dueDate = 'La fecha de vencimiento debe ser futura';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setLoading(true);

    try {
      let savedTask;
      const taskData = {
        ...formData,
        dueDate: formData.dueDate ? new Date(formData.dueDate).toISOString() : null
      };

      if (task) {
        const updateData: UpdateTaskDto = taskData;
        savedTask = await taskService.updateTask(task.id, updateData);
      } else {
        const createData: CreateTaskDto = {
          title: taskData.title,
          description: taskData.description,
          projectId: taskData.projectId,
          assignedToId: taskData.assignedToId,
          priority: taskData.priority,
          status: taskData.status,
          dueDate: taskData.dueDate || undefined,
          estimatedHours: taskData.estimatedHours
        };
        savedTask = await taskService.createTask(createData);
      }
      onSave(savedTask);
    } catch (error) {
      console.error('Error saving task:', error);
      setErrors({ general: 'Error al guardar la tarea. Intente nuevamente.' });
    } finally {
      setLoading(false);
    }
  };

  const handleInputChange = (field: string, value: any) => {
    setFormData({ ...formData, [field]: value });
    // Limpiar error del campo cuando el usuario empiece a escribir
    if (errors[field]) {
      setErrors({ ...errors, [field]: '' });
    }
  };

  const getStatusOptions = () => [
    { value: TaskStatus.Pending, label: '⏳ Pendiente' },
    { value: TaskStatus.InProgress, label: '🔄 En Progreso' },
    { value: TaskStatus.Completed, label: '✅ Completado' },
    { value: TaskStatus.Cancelled, label: '❌ Cancelado' }
  ];

  const getPriorityOptions = () => [
    { value: Priority.Low, label: '🟢 Baja' },
    { value: Priority.Medium, label: '🟡 Media' },
    { value: Priority.High, label: '🟠 Alta' },
    { value: Priority.Critical, label: '🔴 Crítica' }
  ];

  return (
    <div className="bg-white p-6 rounded-lg shadow-lg max-w-2xl mx-auto">
      <h2 className="text-2xl font-bold mb-6 text-gray-800">
        {task ? '📝 Editar Tarea' : '➕ Crear Nueva Tarea'}
      </h2>
      
      {errors.general && (
        <div className="mb-4 p-3 bg-red-100 border border-red-400 text-red-700 rounded">
          {errors.general}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-6">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Título de la Tarea *
          </label>
          <input
            type="text"
            value={formData.title}
            onChange={(e) => handleInputChange('title', e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
              errors.title ? 'border-red-500' : 'border-gray-300'
            }`}
            placeholder="Ingrese el título de la tarea"
          />
          {errors.title && <p className="mt-1 text-sm text-red-600">{errors.title}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Descripción *
          </label>
          <textarea
            value={formData.description}
            onChange={(e) => handleInputChange('description', e.target.value)}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
              errors.description ? 'border-red-500' : 'border-gray-300'
            }`}
            rows={4}
            placeholder="Describa los detalles de la tarea"
          />
          {errors.description && <p className="mt-1 text-sm text-red-600">{errors.description}</p>}
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Proyecto *
            </label>
            <select
              value={formData.projectId}
              onChange={(e) => handleInputChange('projectId', parseInt(e.target.value))}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.projectId ? 'border-red-500' : 'border-gray-300'
              }`}
              disabled={!!projectId} // Deshabilitar si ya viene preseleccionado
            >
              <option value="">Seleccionar proyecto</option>
              {projects.map(project => (
                <option key={project.id} value={project.id}>
                  {project.name}
                </option>
              ))}
            </select>
            {errors.projectId && <p className="mt-1 text-sm text-red-600">{errors.projectId}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Asignado a *
            </label>
            <select
              value={formData.assignedToId}
              onChange={(e) => handleInputChange('assignedToId', parseInt(e.target.value))}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.assignedToId ? 'border-red-500' : 'border-gray-300'
              }`}
            >
              <option value="">Seleccionar usuario</option>
              {users.map(user => (
                <option key={user.id} value={user.id}>
                  {user.fullName} ({user.username})
                </option>
              ))}
            </select>
            {errors.assignedToId && <p className="mt-1 text-sm text-red-600">{errors.assignedToId}</p>}
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Prioridad
            </label>
            <select
              value={formData.priority}
              onChange={(e) => handleInputChange('priority', parseInt(e.target.value))}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {getPriorityOptions().map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Estado
            </label>
            <select
              value={formData.status}
              onChange={(e) => handleInputChange('status', parseInt(e.target.value))}
              className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {getStatusOptions().map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Fecha de Vencimiento
            </label>
            <input
              type="date"
              value={formData.dueDate}
              onChange={(e) => handleInputChange('dueDate', e.target.value)}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.dueDate ? 'border-red-500' : 'border-gray-300'
              }`}
            />
            {errors.dueDate && <p className="mt-1 text-sm text-red-600">{errors.dueDate}</p>}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Horas Estimadas
            </label>
            <input
              type="number"
              min="0"
              step="0.5"
              value={formData.estimatedHours}
              onChange={(e) => handleInputChange('estimatedHours', parseFloat(e.target.value) || 0)}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 ${
                errors.estimatedHours ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="0"
            />
            {errors.estimatedHours && <p className="mt-1 text-sm text-red-600">{errors.estimatedHours}</p>}
          </div>
        </div>

        <div className="flex justify-end space-x-3 pt-6 border-t">
          <button
            type="button"
            onClick={onCancel}
            className="px-6 py-2 text-gray-600 bg-gray-200 rounded-md hover:bg-gray-300 transition-colors"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={loading}
            className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors disabled:bg-blue-300 flex items-center"
          >
            {loading ? (
              <>
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                Guardando...
              </>
            ) : (
              <>
                {task ? '📝 Actualizar' : '➕ Crear'} Tarea
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default TaskForm;