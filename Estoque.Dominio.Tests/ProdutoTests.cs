using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Estoque.Dominio.Tests
{
    public class ProdutoTests
    {
        [Fact]
        public void Entrada_QuantidadeValida_AumentaTotalDisponivel()

        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            Assert.Equal(100, produto.Total);
            Assert.Equal(100, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
            Assert.Equal(0, produto.Bloqueada);

        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Entrada_QuantidadeZeroOuNegativa_LancaErroEMantemEstoque(int quantidade)
        {
            var produto = new Produto("P1");
            produto.Entrada(10);

            Assert.Throws<QuantidadeInvalidaException>(() => produto.Entrada(quantidade));

            Assert.Equal(10, produto.Total);
            Assert.Equal(10, produto.Disponivel);
        }


        [Fact]
        public void Saida_QuantidadeMenorQueDisponivel_ReduzTotalEDisponivel()
        {
            var produto = new Produto("P1");
            
            produto.Entrada(100);

            produto.Saida(30);

            Assert.Equal(70, produto.Total);
            Assert.Equal(70, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
            Assert.Equal(0, produto.Bloqueada);
        }

        [Fact]
        public void Saida_QuantidadeMaiorQueDisponivel_LancaErroEMantemEstoque()
        {
            var produto = new Produto("P1");

            produto.Entrada(10);

            Assert.Throws<EstoqueInsuficienteException>(() => produto.Saida(11));

            Assert.Equal(10, produto.Total);
            Assert.Equal(10, produto.Disponivel);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Saida_QuantidadeZeroOuNegativa_LancaErroEMantemEstoque(int quantidade)
        {
            var produto = new Produto("P1");
            produto.Entrada(10);

            Assert.Throws<QuantidadeInvalidaException>(() => produto.Saida(quantidade));
            
            Assert.Equal(10, produto.Total);
            Assert.Equal(10, produto.Disponivel);
        }


        [Fact]
        public void Reservar_QuantidadeMenorQueDisponivel_MoveDisponivelParaReservada()
        {
            var produto = new Produto("P1");
            
            produto.Entrada(100);

            var reserva = produto.Reservar("PED-1", 30);

            Assert.Equal(100, produto.Total);
            Assert.Equal(70, produto.Disponivel);
            Assert.Equal(30, produto.Reservada);
            Assert.Equal(0, produto.Bloqueada);
            Assert.NotEqual(Guid.Empty, reserva.Id);
            Assert.Equal("PED-1", reserva.PedidoId);
            Assert.Equal(30, reserva.Quantidade);
        }


        [Fact]
        public void Reservar_VariasReservas_SomamNaReservadaComIdsDiferentes() 
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var reserva1 = produto.Reservar("PED-1", 30);
            var reserva2 = produto.Reservar("PED-2", 20);

            Assert.Equal(50, produto.Reservada);
            Assert.Equal(50, produto.Disponivel);
            Assert.Equal(100, produto.Total);
            Assert.NotEqual(reserva1.Id, reserva2.Id);

        }


        [Fact]
        public void Reservar_QuantidadeMaiorQueDisponivel_LancaErroEMantemEstoque()
        {
            var produto = new Produto("P1");

            produto.Entrada(10);

            Assert.Throws<EstoqueInsuficienteException>(() => produto.Reservar("PED-1", 11));

            Assert.Equal(10, produto.Total);
            Assert.Equal(10, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
    
        public void Reservar_QuantidadeZeroOuNegativa_LancaErroEMantemEstoque(int quantidade)
        {
            var produto = new Produto("P1");

            produto.Entrada(10);

            Assert.Throws<QuantidadeInvalidaException>(() => produto.Reservar("PED-1", quantidade));

            Assert.Equal(10, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
        }
    }
}
