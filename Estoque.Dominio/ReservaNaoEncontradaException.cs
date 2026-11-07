using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class ReservaNaoEncontradaException : Exception
    {
        public ReservaNaoEncontradaException(Guid id) : base($"Reserva não encontrada: {id} ")
        {
        }

    }
}
