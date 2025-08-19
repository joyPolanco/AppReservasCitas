using Aplicacion.DTOs;
using Aplicacion.Interfaces;
using Dominio.Entidades;
using Dominio.Entidades.Models;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aplicacion.Servicios
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private IPasswordManager _passwordManager;
        public UsuarioService(IUsuarioRepository repo, IPasswordManager passwordManager)
        {
            _repo = repo;
            _passwordManager = passwordManager;
        }

        public Task ActualizarContrasena(string correo, string contrasenaNueva)
        {
            throw new NotImplementedException();
        }

        public async Task Add(UsuarioCreateDTO usuario)
    {
        // Validar nombre
        if (string.IsNullOrWhiteSpace(usuario.Nombre) || usuario.Nombre.Length < 2)
        {
            throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
        }

        // Validar correo
        if (string.IsNullOrWhiteSpace(usuario.Correo) ||
            !Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("El correo electrónico no es válido.");
        }

        // Validar contraseña
        if (string.IsNullOrWhiteSpace(usuario.Contrasena) || usuario.Contrasena.Length < 8)
        {
            throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
        }
            var userExists = await _repo.GetUsuarioByEmail(usuario.Correo);
            if (userExists != null) throw new ArgumentException("El email ya tiene una cuenta asociada");
       
        
        var user = new Usuario
        {
            Nombre = usuario.Nombre,
            Email = usuario.Correo,
            HashContrasena = _passwordManager.Encrypt(usuario.Contrasena),
            IdRol = 2
        };

        await _repo.Create(user);
    }
        public async Task AddAdmin(UsuarioCreateDTO usuario)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(usuario.Nombre) || usuario.Nombre.Length < 2)
            {
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres.");
            }

            // Validar correo
            if (string.IsNullOrWhiteSpace(usuario.Correo) ||
                !Regex.IsMatch(usuario.Correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                throw new ArgumentException("El correo electrónico no es válido.");
            }

            // Validar contraseña
            if (string.IsNullOrWhiteSpace(usuario.Contrasena) || usuario.Contrasena.Length < 8)
            {
                throw new ArgumentException("La contraseña debe tener al menos 8 caracteres.");
            }
            var userExists = await _repo.GetUsuarioByEmail(usuario.Correo);
            if (userExists != null) throw new ArgumentException("El email ya tiene una cuenta asociada");


            var user = new Usuario
            {
                Nombre = usuario.Nombre,
                Email = usuario.Correo,
                HashContrasena = _passwordManager.Encrypt(usuario.Contrasena),
                IdRol = 1
            };

            await _repo.Create(user);
        }

        public async Task<List<AdminViewDTO>> GetUsuariosAdmin()
        {
            var usuarios = await _repo.GetAll();

            return usuarios.Select(u => new AdminViewDTO
            {
                Nombre = u.Nombre,
                Correo = u.Email,
            
            }).ToList();
        }
           

        public async  Task<UsuarioDTO> GetUsuarioByEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("El email no puede ser nulo");
            var user=await _repo.GetUsuarioByEmail(email);
           

            if (user == null) throw new ArgumentException("No existe el usuario con ese email");
            var usuariofinal= user==null? null : 
            new UsuarioDTO()
            {
                Email = user.Email,
                ContrasenaHasheada = user.HashContrasena,
                PublicId = user.PublicId,
                Rol = user.IdRolNavigation.Nombre
            };
            return usuariofinal;
          
        }

     
    }
}
