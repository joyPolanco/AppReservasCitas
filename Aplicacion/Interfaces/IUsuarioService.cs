using Aplicacion.DTOs;
using Dominio.Entidades;
using Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aplicacion.Interfaces
{
    public interface IUsuarioService
    {
        public  Task<UsuarioDTO> GetUsuarioByEmail(string email);
        public Task Add(UsuarioCreateDTO usuario);
        public Task AddAdmin(UsuarioCreateDTO usuario);
        public Task<List<AdminViewDTO>> GetUsuariosAdmin();


        public Task ActualizarContrasena(string correo, string contrasenaNueva);
    }
}
