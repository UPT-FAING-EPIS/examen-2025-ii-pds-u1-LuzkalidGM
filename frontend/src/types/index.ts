export interface User {
  id: number;
  username: string;
  email: string;
  fullName: string;
  role: UserRole;
  isActive: boolean;
  createdAt: string;
}

export enum UserRole {
  User = 0,
  Admin = 1,
  ProjectManager = 2
}

export enum TaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

// Alias para compatibilidad
export const Priority = TaskPriority;

export enum ProjectStatus {
  Planning = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
  OnHold = 4
}

export interface Project {
  id: number;
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  managerId: number;
  status: ProjectStatus;
  createdBy: User;
  responsible: User;
  totalTasks: number;
  completedTasks: number;
  progressPercentage: number;
  createdAt: string;
  updatedAt: string;
}

export interface Task {
  id: number;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate?: string;
  projectId: number;
  project?: Project;
  assignedTo?: User;
  assignedToId?: number;
  createdBy: User;
  estimatedHours?: number;
  actualHours: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProjectDto {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  managerId: number;
  status?: ProjectStatus;
}

export interface UpdateProjectDto {
  name: string;
  description: string;
  startDate: string;
  endDate: string;
  managerId: number;
  status: ProjectStatus;
}

export interface CreateTaskDto {
  title: string;
  description: string;
  projectId: number;
  assignedToId: number;
  priority: TaskPriority;
  status?: TaskStatus;
  dueDate?: string;
  estimatedHours?: number;
}

export interface UpdateTaskDto {
  title: string;
  description: string;
  projectId: number;
  assignedToId: number;
  priority: TaskPriority;
  status: TaskStatus;
  dueDate?: string | null;
  estimatedHours?: number;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  user: User;
  expiration: string;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message: string;
}

export interface AuthContextType {
  user: User | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: boolean;
  loading: boolean;
}