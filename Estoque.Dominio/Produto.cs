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

        private readonly Dictionary<Guid, Reserva> _reservas = new(); //guarda pares de chave e valor
        public Produto(string id) 
        {
            Id = id;
        }


        private static void ValidarQuantidade(int quantidade) 
        {
            if(quantidade <=0)
                throw new QuantidadeInvalidaException(quantidade);
        }

        private void ValidarDisponivel(int quantidade) 
        {
            if(quantidade> Disponivel)
                throw new EstoqueInsuficienteException(quantidade, Disponivel);
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
            ValidarDisponivel(quantidade);

            if (quantidade > Disponivel)
                throw new EstoqueInsuficienteException(quantidade, Disponivel);

            Total -= quantidade;
            Disponivel -= quantidade;
        }

        public Reserva Reservar(string pedidoId, int quantidade) 
        {
            ValidarQuantidade(quantidade);
            ValidarDisponivel(quantidade);

            var reserva =  new Reserva(pedidoId, quantidade);
            _reservas.Add(reserva.Id, reserva); //adicionando o id da reserva do pedido no dicionário

            Disponivel -= quantidade;
            Reservada += quantidade;

            return reserva;

        }


    }
}
