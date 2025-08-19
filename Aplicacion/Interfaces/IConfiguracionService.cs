using Aplicacion.DTOs;
using Aplicacion.DTOs.Planificacion;
using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IConfiguracionService
    {
        public Task Actualizar(AppConfiguracionDTO appConfiguracionDTO);
        public  Task<List<TurnoDTO>> GetTurnoDTOs();

        public Task<AppConfiguracionDTO> Get();


    }
}
