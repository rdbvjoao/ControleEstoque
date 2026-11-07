using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;    

namespace Estoque.Dominio.Tests
{
    public class EstoqueServiceTests
    {
        private static EstoqueService CriarServico() => new(new ProdutoRepositoryEmMemoria());

        [Fact]
        public void Entrada_ProdutoNovo_CriaProdutoComEstoque() 
        {

            var servico = CriarServico();

            servico.Entrada("P1", 100);

            var situacao = servico.Consultar("P1");

            Assert.Equal("P1", situacao.ProdutoId);
            Assert.Equal(100, situacao.Total);
            Assert.Equal(100, situacao.Disponivel);
            Assert.Equal(0, situacao.Reservada);
            Assert.Equal(0, situacao.Bloqueada);
        }


        [Fact]
        public void Entrada_ProdutoExistente_SomaNoEstoque()
        {
            var servico = CriarServico();

            servico.Entrada("P1", 100);

            servico.Entrada("P1", 50);

            var situacao = servico.Consultar("P1");

            Assert.Equal(150, situacao.Total);
            Assert.Equal(150, situacao.Disponivel);
        }


        [Fact]
        public void Entrada_QuantidadeInvalidaEmProdutoNovo_NaoCriaOProduto()
        {
            var servico = CriarServico();

            Assert.Throws<QuantidadeInvalidaException>(() => servico.Entrada("P1", 0));

            Assert.Throws<ProdutoNaoEncontradoException>(() => servico.Consultar("P1"));

        }

        [Fact]
        public void Saida_ProdutoInexistente_LancaErro()
        {
            var servico = CriarServico();

            Assert.Throws<ProdutoNaoEncontradoException>(() => servico.Saida("P1", 1));
        }

        [Fact]
        public void Consultar_ProdutoInexistente_LancaErro()
        {
            var servico = CriarServico();

            Assert.Throws<ProdutoNaoEncontradoException>(() => servico.Consultar("P1"));
        }

        [Fact]
        public void ReservarEBloquear_PeloServico_AtualizamSituacaoELiberacaoDevolve()
        {
            var servico = CriarServico();

            servico.Entrada("P1", 100);

            var reserva = servico.Reservar("P1", "PED-1", 30);

            var bloqueio = servico.Bloquear("P1", 20);

            var situacao = servico.Consultar("P1");

            Assert.Equal(100, situacao.Total);
            Assert.Equal(50, situacao.Disponivel);
            Assert.Equal(30, situacao.Reservada);
            Assert.Equal(20, situacao.Bloqueada);   

            Assert.Same(reserva, servico.ObterReserva("P1", reserva.Id));
            Assert.Same(bloqueio, servico.ObterBloqueio("P1", bloqueio.Id));

            servico.LiberarReserva("P1", reserva.Id);
            servico.LiberarBloqueio("P1", bloqueio.Id);

            var final = servico.Consultar("P1");

            Assert.Equal(100, final.Total);
            Assert.Equal(100, final.Disponivel);
            Assert.Equal(0, final.Reservada);
            Assert.Equal(0, final.Bloqueada);
        }


    }
}
