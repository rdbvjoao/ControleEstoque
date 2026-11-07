    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class EstoqueInsuficienteException : Exception
    {
    
        public EstoqueInsuficienteException(int solicitada, int disponivel) : base ($"Quantidade solicitada ({solicitada}) maior que a disponível ({disponivel})")
        {
        }
    }

}
