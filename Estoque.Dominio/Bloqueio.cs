using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class Bloqueio
    {
        public Guid Id { get; }
        public int Quantidade { get; }
        public Bloqueio(int quantidade)
        {
            Id = Guid.NewGuid();
            Quantidade = quantidade;
        }
    }
}
