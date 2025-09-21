# Documentación de Clases - Project Management API

## Modelos de Dominio

### User
Representa un usuario del sistema con propiedades como Username, Email, PasswordHash, Role.

### Project
Representa un proyecto en el sistema con Name, Description, StartDate, EndDate, Status.

### ProjectTask
Representa una tarea dentro de un proyecto con Title, Description, Status, Priority.

### TaskComment
Representa un comentario en una tarea con Content, TaskId, CreatedById.

## Servicios
- UserService: Lógica de usuarios y autenticación
- ProjectService: Lógica de gestión de proyectos
- TaskService: Lógica de gestión de tareas
- JwtService: Gestión de tokens JWT

## Controladores
- UsersController: Endpoints de usuarios
- ProjectsController: Endpoints de proyectos
- TasksController: Endpoints de tareas

*Documentación generada automáticamente*
