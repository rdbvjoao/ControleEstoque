using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class QuantidadeInvalidaException : Exception
    {
        public QuantidadeInvalidaException(int quantidade) : base($"A quantidade deve ser maior que zero! Valor informado {quantidade} ")
        {
        
        }
      
    }
}
