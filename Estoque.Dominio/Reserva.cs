using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class Reserva
    {
        public Guid Id { get; }
        public string PedidoId { get; }
        public int Quantidade { get; }


        public Reserva(string pedidoId, int quantidade) 
        {
        
            Id = Guid.NewGuid();
            PedidoId = pedidoId;
            Quantidade = quantidade;
        }
    }
}
