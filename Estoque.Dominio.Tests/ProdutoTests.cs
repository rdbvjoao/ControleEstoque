using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Frameworks;
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

        [Fact]
        public void LiberarReserva_ReservaExistente_DevolveAoDisponivelEMantemOutras() 
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var reserva1 = produto.Reservar("PED-1", 30);
            var reserva2 = produto.Reservar("PED-2", 20);

            produto.LiberarReserva(reserva1.Id);

            Assert.Equal(100, produto.Total);
            Assert.Equal(80, produto.Disponivel);
            Assert.Equal(20, produto.Reservada);
            Assert.Equal(0, produto.Bloqueada);
            Assert.Same(reserva2, produto.ObterReserva(reserva2.Id));

        }

        [Fact]
        public void LiberarReserva_IdInexistente_LancaErroMantemEstoque() 
        {
            var produto = new Produto("P1");
            
            produto.Entrada(100);

            produto.Reservar("PED-1", 30);

            Assert.Throws<ReservaNaoEncontradaException>(() => produto.LiberarReserva(Guid.NewGuid()));

            Assert.Equal(70, produto.Disponivel);
            Assert.Equal(30, produto.Reservada);

        }

        [Fact]
        public void LiberarReserva_JaLiberada_LancaErroMantemEstoque()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var reserva = produto.Reservar("PED-1", 30);

            produto.LiberarReserva(reserva.Id);

            Assert.Throws<ReservaNaoEncontradaException>(() => produto.LiberarReserva(reserva.Id));

            Assert.Equal(100, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
        }


        [Fact]
        public void ObterReserva_IdExistente_RetornaReserva()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var reserva = produto.Reservar("PED-1", 30);

            var encontrada = produto.ObterReserva(reserva.Id);

            Assert.Same(reserva, encontrada);
        }

        [Fact]
        public void ObterReserva_IdInexistente_LancaErro()
        {
            var produto = new Produto("P1");

            Assert.Throws<ReservaNaoEncontradaException>(() => produto.ObterReserva(Guid.NewGuid()));
        }


        [Fact]
        public void LiberarBloqueio_BloqueioExistente_DevolveAoDisponivelEMantemOutros() 
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var bloqueio1 = produto.Bloquear(20);

            var bloqueio2 = produto.Bloquear(10);


            produto.LiberarBloqueio(bloqueio1.Id);

            Assert.Equal(100, produto.Total);
            Assert.Equal(90, produto.Disponivel);
            Assert.Equal(0, produto.Reservada);
            Assert.Equal(10, produto.Bloqueada);
            Assert.Same(bloqueio2, produto.ObterBloqueio(bloqueio2.Id));

        }


        [Fact]
        public void LiberarBloqueio_IdInexistente_LancaErroEMantemEstoque() 
        {

            var produto = new Produto("P1");
            
            produto.Entrada(100);
            produto.Bloquear(20);

            Assert.Throws<BloqueioNaoEncontradoException>(() => produto.LiberarBloqueio(Guid.NewGuid()));

            Assert.Equal(80, produto.Disponivel);
            Assert.Equal(20, produto.Bloqueada);

        }

        [Fact]
        public void LiberarBloqueio_JaLiberado_LancaErroMantemEstoque()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var bloqueio = produto.Bloquear(20);

            produto.LiberarBloqueio(bloqueio.Id);

            Assert.Throws<BloqueioNaoEncontradoException>(() => produto.LiberarBloqueio(bloqueio.Id));

            Assert.Equal(100, produto.Disponivel);
            Assert.Equal(0, produto.Bloqueada);
        }


        [Fact]
        public void LiberarBloqueio_IdDeReserva_LancaErroMantemEstoque()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var reserva = produto.Reservar("PED-1", 30);

            Assert.Throws<BloqueioNaoEncontradoException>(() => produto.LiberarBloqueio(reserva.Id));

            Assert.Equal(70, produto.Disponivel);
            Assert.Equal(30, produto.Reservada);
        }

        [Fact]
        public void ObterBloqueio_IdExistente_RetornaBloqueio()
        {
            var produto = new Produto("P1");

            produto.Entrada(100);

            var bloqueio = produto.Bloquear(20);

            var encontrado = produto.ObterBloqueio(bloqueio.Id);

            Assert.Same(bloqueio, encontrado);
        }

        [Fact]
        public void ObterBloqueio_IdInexistente_LancaErro() 
        {

            var produto = new Produto("P1");

            Assert.Throws<BloqueioNaoEncontradoException>(() => produto.ObterBloqueio(Guid.NewGuid()));

        }

        [Fact]
        public void Reservar_Concorrente_NaoReservaMaisQueODisponivel()
        {
            var produto = new Produto("P1");

            produto.Entrada(1000);

            var sucessos = 0;

            var falhas = 0;

            Parallel.For(0, 5000, _ => //executa o bloco 5000 vezes
            {
                try
                {
                    produto.Reservar("PED", 1);

                    Interlocked.Increment(ref sucessos);
                }
                
                catch (EstoqueInsuficienteException)
                {
                    Interlocked.Increment(ref falhas);
                }
            });

            Assert.Equal(1000, sucessos);
            Assert.Equal(4000, falhas);
            Assert.Equal(0, produto.Disponivel);
            Assert.Equal(1000, produto.Reservada);
            Assert.Equal(1000, produto.Total);
        }


    }
}
