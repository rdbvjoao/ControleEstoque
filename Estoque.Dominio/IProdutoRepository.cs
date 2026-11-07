using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public interface IProdutoRepository
    {
        Produto? Obter(string produtoId);
        void Adicionar(Produto produto);

    }

}
