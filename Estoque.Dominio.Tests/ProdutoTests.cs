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


        [Fact]
        public void Bloquear_QuantidadeMenorQueDisponivel_MoveDisponivelParaBloqueada()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var bloqueio = produto.Bloquear(20);

            Assert.Equal(100, produto.Total);
            Assert.Equal(80, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
            Assert.Equal(20, produto.Bloqueada);
            Assert.NotEqual(Guid.Empty, bloqueio.Id);
            Assert.Equal(20, bloqueio.Quantidade);

        }


        [Fact]
        public void Bloquear_VariosBloqueios_SomamNaBloqueadaComIdsDiferentes()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var bloqueio1 = produto.Bloquear(20);
            var bloqueio2 = produto.Bloquear(10);

            Assert.Equal(30, produto.Bloqueada);
            Assert.Equal(70, produto.Disponivel);
            Assert.Equal(100, produto.Total);
            Assert.NotEqual(bloqueio1.Id, bloqueio2.Id);
        }


        [Fact]
        public void Bloquear_QuantidadeMaiorQueDisponivel_LancaErroEMantemEstoque()
        {
            var produto = new Produto("P1");

            produto.Entrada(10);

            Assert.Throws<EstoqueInsuficienteException>(() => produto.Bloquear(11));

            Assert.Equal(10, produto.Total);
            Assert.Equal(10, produto.Disponivel);
            Assert.Equal(0, produto.Bloqueada);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Bloquear_QuantidadeZeroOuNegativa_LancaErroEMantemEstoque(int quantidade) 
        {

            var produto = new Produto("P1");
            
            produto.Entrada(10);


            Assert.Throws<QuantidadeInvalidaException>(() => produto.Bloquear(quantidade));

            Assert.Equal(10, produto.Disponivel);
            Assert.Equal(0, produto.Bloqueada);

        }


        [Fact]
        public void Saida_ComReservaEBloqueio_RetiraApenasDoDisponivel() 
        {

            var produto = new Produto("P1");
            
            produto.Entrada(100);

            produto.Reservar("PED-1", 30);
            
            produto.Bloquear(20);

            Assert.Throws<EstoqueInsuficienteException>(() => produto.Saida(51));

            produto.Saida(50);

            Assert.Equal(50, produto.Total);
            Assert.Equal(0, produto.Disponivel);
            Assert.Equal(30, produto.Reservada);
            Assert.Equal(20, produto.Bloqueada);

        }

    }
}
