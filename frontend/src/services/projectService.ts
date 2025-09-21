import api from './api';

export interface Project {
  id: string;
  name: string;
  description: string;
  status: 'Planning' | 'InProgress' | 'Completed' | 'OnHold';
  priority: 'Low' | 'Medium' | 'High';
  startDate: string;
  endDate: string;
  ownerId: string;
  owner?: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
  };
  members?: ProjectMember[];
  tasks?: Task[];
  createdDate: string;
  updatedDate: string;
}

export interface ProjectMember {
  id: string;
  projectId: string;
  userId: string;
  role: string;
  joinedDate: string;
  user?: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
  };
}

export interface Task {
  id: string;
  title: string;
  description: string;
  status: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled';
  priority: 'Low' | 'Medium' | 'High';
  projectId: string;
  assigneeId?: string;
  estimatedHours?: number;
  actualHours?: number;
  dueDate?: string;
  createdDate: string;
  updatedDate: string;
  assignee?: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
  };
  project?: {
    id: string;
    name: string;
  };
}

export interface CreateProjectRequest {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  priority: 'Low' | 'Medium' | 'High';
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
  status?: 'Planning' | 'InProgress' | 'Completed' | 'OnHold';
  priority?: 'Low' | 'Medium' | 'High';
  startDate?: string;
  endDate?: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  priority: 'Low' | 'Medium' | 'High';
  projectId: string;
  assigneeId?: string;
  estimatedHours?: number;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  status?: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled';
  priority?: 'Low' | 'Medium' | 'High';
  assigneeId?: string;
  estimatedHours?: number;
  actualHours?: number;
  dueDate?: string;
}

// Project Services
export class ProjectService {
  static async getProjects(): Promise<Project[]> {
    const response = await api.get('/projects');
    return response.data;
  }

  static async getProjectById(id: string): Promise<Project> {
    const response = await api.get(`/projects/${id}`);
    return response.data;
  }

  static async createProject(project: CreateProjectRequest): Promise<Project> {
    const response = await api.post('/projects', project);
    return response.data;
  }

  static async updateProject(id: string, project: UpdateProjectRequest): Promise<Project> {
    const response = await api.put(`/projects/${id}`, project);
    return response.data;
  }

  static async deleteProject(id: string): Promise<void> {
    await api.delete(`/projects/${id}`);
  }

  static async getProjectMembers(projectId: string): Promise<ProjectMember[]> {
    const response = await api.get(`/projects/${projectId}/members`);
    return response.data;
  }

  static async addProjectMember(projectId: string, userId: string, role: string): Promise<ProjectMember> {
    const response = await api.post(`/projects/${projectId}/members`, { userId, role });
    return response.data;
  }

  static async removeProjectMember(projectId: string, memberId: string): Promise<void> {
    await api.delete(`/projects/${projectId}/members/${memberId}`);
  }
}

// Task Services
export class TaskService {
  static async getTasks(projectId?: string): Promise<Task[]> {
    const url = projectId ? `/tasks?projectId=${projectId}` : '/tasks';
    const response = await api.get(url);
    return response.data;
  }

  static async getTaskById(id: string): Promise<Task> {
    const response = await api.get(`/tasks/${id}`);
    return response.data;
  }

  static async createTask(task: CreateTaskRequest): Promise<Task> {
    const response = await api.post('/tasks', task);
    return response.data;
  }

  static async updateTask(id: string, task: UpdateTaskRequest): Promise<Task> {
    const response = await api.put(`/tasks/${id}`, task);
    return response.data;
  }

  static async deleteTask(id: string): Promise<void> {
    await api.delete(`/tasks/${id}`);
  }

  static async getTasksByProject(projectId: string): Promise<Task[]> {
    const response = await api.get(`/projects/${projectId}/tasks`);
    return response.data;
  }

  static async assignTask(taskId: string, userId: string): Promise<Task> {
    const response = await api.post(`/tasks/${taskId}/assign`, { userId });
    return response.data;
  }

  static async unassignTask(taskId: string): Promise<Task> {
    const response = await api.post(`/tasks/${taskId}/unassign`);
    return response.data;
  }
}