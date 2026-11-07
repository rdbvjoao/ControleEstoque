using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class ProdutoRepositoryEmMemoria : IProdutoRepository
    {
        private readonly ConcurrentDictionary<string, Produto> _produtos = new();

        public Produto? Obter(string produtoId) => _produtos.TryGetValue(produtoId, out var produto) ? produto : null;

        public void Adicionar(Produto produto)
        {
            _produtos[produto.Id] = produto;
        }

    }
}
