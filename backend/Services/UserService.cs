using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Api.Data;
using ProjectManagement.Api.DTOs;
using ProjectManagement.Api.Models;

namespace ProjectManagement.Api.Services
{
    /// <summary>
    /// Interfaz para el servicio de usuarios
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Autentica un usuario
        /// </summary>
        /// <param name="loginDto">Datos de login</param>
        /// <returns>Usuario autenticado o null</returns>
        Task<User?> AuthenticateAsync(LoginDto loginDto);

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="registerUserDto">Datos del usuario a registrar</param>
        /// <returns>Usuario registrado</returns>
        Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto);

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado</returns>
        Task<UserDto?> GetUserByIdAsync(int id);

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="updateUserDto">Datos actualizados del usuario</param>
        /// <returns>Usuario actualizado</returns>
        Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="changePasswordDto">Datos del cambio de contraseña</param>
        /// <returns>True si se cambió correctamente</returns>
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        Task<bool> DeleteUserAsync(int id);

        /// <summary>
        /// Obtiene usuarios activos para asignación
        /// </summary>
        /// <returns>Lista de usuarios activos</returns>
        Task<IEnumerable<UserSummaryDto>> GetActiveUsersAsync();
    }

    /// <summary>
    /// Implementación del servicio de usuarios
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ProjectManagementContext _context;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del servicio de usuarios
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="mapper">Mapeador de objetos</param>
        public UserService(ProjectManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Autentica un usuario
        /// </summary>
        /// <param name="loginDto">Datos de login</param>
        /// <returns>Usuario autenticado o null</returns>
        public async Task<User?> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => 
                    (u.Username == loginDto.UsernameOrEmail || u.Email == loginDto.UsernameOrEmail) 
                    && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="registerUserDto">Datos del usuario a registrar</param>
        /// <returns>Usuario registrado</returns>
        public async Task<UserDto> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            // Verificar si el usuario o email ya existen
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerUserDto.Username || u.Email == registerUserDto.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("El nombre de usuario o email ya está en uso.");
            }

            var user = new User
            {
                Username = registerUserDto.Username,
                Email = registerUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerUserDto.Password),
                FullName = registerUserDto.FullName,
                Role = registerUserDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        /// <summary>
        /// Obtiene un usuario por ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario encontrado</returns>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="updateUserDto">Datos actualizados del usuario</param>
        /// <returns>Usuario actualizado</returns>
        public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            // Verificar si el email ya está en uso por otro usuario
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == updateUserDto.Email && u.Id != id);

            if (existingUser != null)
            {
                throw new InvalidOperationException("El email ya está en uso por otro usuario.");
            }

            user.Email = updateUserDto.Email;
            user.FullName = updateUserDto.FullName;
            user.Role = updateUserDto.Role;
            user.IsActive = updateUserDto.IsActive;

            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="changePasswordDto">Datos del cambio de contraseña</param>
        /// <returns>True si se cambió correctamente</returns>
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="id">ID del usuario a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // En lugar de eliminar físicamente, desactivar el usuario
            user.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Obtiene usuarios activos para asignación
        /// </summary>
        /// <returns>Lista de usuarios activos</returns>
        public async Task<IEnumerable<UserSummaryDto>> GetActiveUsersAsync()
        {
            var users = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            return _mapper.Map<IEnumerable<UserSummaryDto>>(users);
        }
    }
}