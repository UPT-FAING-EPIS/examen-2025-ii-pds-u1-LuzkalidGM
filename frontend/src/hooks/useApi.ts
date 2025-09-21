import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { 
  ProjectService, 
  TaskService, 
  Project, 
  Task, 
  CreateProjectRequest, 
  UpdateProjectRequest,
  CreateTaskRequest,
  UpdateTaskRequest,
  ProjectMember
} from '../services/projectService';

// Project Hooks
export const useProjects = () => {
  return useQuery({
    queryKey: ['projects'],
    queryFn: ProjectService.getProjects,
    staleTime: 5 * 60 * 1000, // 5 minutes
  });
};

export const useProject = (id: string) => {
  return useQuery({
    queryKey: ['project', id],
    queryFn: () => ProjectService.getProjectById(id),
    enabled: !!id,
  });
};

export const useCreateProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (project: CreateProjectRequest) => ProjectService.createProject(project),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    },
  });
};

export const useUpdateProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, project }: { id: string; project: UpdateProjectRequest }) => 
      ProjectService.updateProject(id, project),
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      queryClient.invalidateQueries({ queryKey: ['project', variables.id] });
    },
  });
};

export const useDeleteProject = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: string) => ProjectService.deleteProject(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
    },
  });
};

export const useProjectMembers = (projectId: string) => {
  return useQuery({
    queryKey: ['project-members', projectId],
    queryFn: () => ProjectService.getProjectMembers(projectId),
    enabled: !!projectId,
  });
};

export const useAddProjectMember = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ projectId, userId, role }: { projectId: string; userId: string; role: string }) =>
      ProjectService.addProjectMember(projectId, userId, role),
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['project-members', variables.projectId] });
      queryClient.invalidateQueries({ queryKey: ['project', variables.projectId] });
    },
  });
};

// Task Hooks
export const useTasks = (projectId?: string) => {
  return useQuery({
    queryKey: projectId ? ['tasks', 'project', projectId] : ['tasks'],
    queryFn: () => TaskService.getTasks(projectId),
    staleTime: 2 * 60 * 1000, // 2 minutes
  });
};

export const useTask = (id: string) => {
  return useQuery({
    queryKey: ['task', id],
    queryFn: () => TaskService.getTaskById(id),
    enabled: !!id,
  });
};

export const useCreateTask = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (task: CreateTaskRequest) => TaskService.createTask(task),
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      queryClient.invalidateQueries({ queryKey: ['tasks', 'project', data.projectId] });
      queryClient.invalidateQueries({ queryKey: ['project', data.projectId] });
    },
  });
};

export const useUpdateTask = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ id, task }: { id: string; task: UpdateTaskRequest }) => 
      TaskService.updateTask(id, task),
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      queryClient.invalidateQueries({ queryKey: ['task', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['tasks', 'project', data.projectId] });
      queryClient.invalidateQueries({ queryKey: ['project', data.projectId] });
    },
  });
};

export const useDeleteTask = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (id: string) => TaskService.deleteTask(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
    },
  });
};

export const useAssignTask = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: ({ taskId, userId }: { taskId: string; userId: string }) =>
      TaskService.assignTask(taskId, userId),
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      queryClient.invalidateQueries({ queryKey: ['task', variables.taskId] });
      queryClient.invalidateQueries({ queryKey: ['tasks', 'project', data.projectId] });
    },
  });
};

export const useUnassignTask = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: (taskId: string) => TaskService.unassignTask(taskId),
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries({ queryKey: ['tasks'] });
      queryClient.invalidateQueries({ queryKey: ['task', variables] });
      queryClient.invalidateQueries({ queryKey: ['tasks', 'project', data.projectId] });
    },
  });
};

// Dashboard Hooks
export const useDashboardData = () => {
  const projectsQuery = useProjects();
  const tasksQuery = useTasks();

  const dashboardStats = {
    totalProjects: projectsQuery.data?.length || 0,
    activeProjects: projectsQuery.data?.filter(p => p.status === 'InProgress').length || 0,
    totalTasks: tasksQuery.data?.length || 0,
    completedTasks: tasksQuery.data?.filter(t => t.status === 'Completed').length || 0,
    pendingTasks: tasksQuery.data?.filter(t => t.status === 'Pending').length || 0,
    inProgressTasks: tasksQuery.data?.filter(t => t.status === 'InProgress').length || 0,
  };

  return {
    projects: projectsQuery.data || [],
    tasks: tasksQuery.data || [],
    stats: dashboardStats,
    isLoading: projectsQuery.isLoading || tasksQuery.isLoading,
    error: projectsQuery.error || tasksQuery.error,
  };
};