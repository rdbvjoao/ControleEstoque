using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public record SituacaoProduto(
        string ProdutoId,
        int Total,
        int Disponivel,
        int Reservada,
        int Bloqueada);
  
}
