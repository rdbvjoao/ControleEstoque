using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class Produto
    {
        public string Id { get; set; }
        public int Total { get; private set; }
        public int Disponivel { get; private set; }
        public int Reservada { get; private set; }
        public int Bloqueada { get; private set; }


        public Produto(string id) 
        {
            Id = id;
        }


        private static void ValidarQuantidade(int quantidade) 
        {
            if(quantidade <=0)
                throw new QuantidadeInvalidaException(quantidade);
        }

        public void Entrada(int quantidade) 
        {
            ValidarQuantidade(quantidade);

            Total += quantidade;
            Disponivel += quantidade;
        }


        public void Saida(int quantidade)
        {
            ValidarQuantidade(quantidade);

            if (quantidade > Disponivel)
                throw new EstoqueInsuficienteException(quantidade, Disponivel);

            Total -= quantidade;
            Disponivel -= quantidade;
        }

    }
}
