using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class BloqueioNaoEncontradoException :Exception 
    {
        public BloqueioNaoEncontradoException(Guid id) : base($"Bloqueio não foi encontrado: {id} ")
        {
        }
    }
}
