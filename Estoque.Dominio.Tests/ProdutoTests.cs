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
    }
}
