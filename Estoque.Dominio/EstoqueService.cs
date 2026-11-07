using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class EstoqueService
    {
        private readonly IProdutoRepository _repositorio; 

        public EstoqueService(IProdutoRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public void Entrada(string produtoId, int quantidade) 
        {
        
            var produto = _repositorio.Obter(produtoId);

            if (produto is null) 
            {
                produto = new Produto(produtoId);
                produto.Entrada(quantidade);
                _repositorio.Adicionar(produto);
                return;
            }

            produto.Entrada(quantidade);
        }

        public void Saida(string produtoId, int quantidade) => ObterProduto(produtoId).Saida(quantidade);


        public Reserva Reservar(string produtoId, string pedidoId, int quantidade) => ObterProduto(produtoId).Reservar(pedidoId, quantidade);


        public Bloqueio Bloquear(string produtoId, int quantidade) => ObterProduto(produtoId).Bloquear(quantidade);


        public void LiberarReserva(string produtoId, Guid reservaId) => ObterProduto(produtoId).LiberarReserva(reservaId);


        public void LiberarBloqueio(string produtoId, Guid bloqueioId) => ObterProduto(produtoId).LiberarBloqueio(bloqueioId);


        public Reserva ObterReserva(string produtoId, Guid reservaId) => ObterProduto(produtoId).ObterReserva(reservaId);


        public Bloqueio ObterBloqueio(string produtoId, Guid bloqueioId) => ObterProduto(produtoId).ObterBloqueio(bloqueioId);


        public SituacaoProduto Consultar(string produtoId) 
        { 
            var prod = ObterProduto(produtoId);

            return new SituacaoProduto(prod.Id, prod.Total, prod.Disponivel, prod.Reservada, prod.Bloqueada);

        }

        private Produto ObterProduto(string produtoId) => _repositorio.Obter(produtoId) ?? throw new ProdutoNaoEncontradoException(produtoId);
    }
}
