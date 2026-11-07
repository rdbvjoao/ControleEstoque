using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    internal static class Validacoes
    {
        public static void ExigirTexto(string? valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))

                throw new IdentificadorInvalidoException(campo);
        }
    }
}
