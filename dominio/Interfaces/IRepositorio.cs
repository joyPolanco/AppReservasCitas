using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IRepositorio <T>
    {
        public Task Create(T Objeto);
        public Task Update(T Objeto);
        
        public Task<T> Get(int id);
        public Task<List<T>> GetAll();

        public Task<int >SaveChangesAsync();

    }
}
;