using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class IdentificadorInvalidoException : Exception
    {
        public IdentificadorInvalidoException(string campo) : base($"O identificador de {campo} é obrigatório" +
            $"e não pode ser vazio! ")
        {
        }
    }
}
