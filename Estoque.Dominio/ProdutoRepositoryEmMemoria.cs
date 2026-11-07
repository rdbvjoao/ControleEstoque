using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class ProdutoRepositoryEmMemoria : IProdutoRepository
    {
        private readonly Dictionary<string, Produto> _produtos = new Dictionary<string, Produto>();

        public Produto? Obter(string produtoId) => _produtos.TryGetValue(produtoId, out var produto) ? produto : null;

        public void Adicionar(Produto produto)
        {
            _produtos[produto.Id] = produto;
        }

    }
}
