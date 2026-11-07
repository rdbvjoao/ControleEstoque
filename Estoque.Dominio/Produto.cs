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

        private readonly Dictionary<Guid, Reserva> _reservas = new(); //guarda pares de chave e valor no dicionário de reservas
        private readonly Dictionary<Guid, Bloqueio> _bloqueios= new(); //guarda pares de chave e valor no dicionário bloqueios


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

        public Reserva ObterReserva(Guid id) 
        {
            if(!_reservas.TryGetValue(id, out var reserva)) //procura o id no dicionário
                throw new ReservaNaoEncontradaException(id);

            return reserva;
        }

        public void LiberarReserva(Guid id) 
        {
            var reserva = ObterReserva(id);

            _reservas.Remove(id); //removendo a reserva do dicionário

            Reservada -= reserva.Quantidade;
            Disponivel += reserva.Quantidade;

        }



        public Bloqueio Bloquear(int quantidade)
        {
            ValidarQuantidade(quantidade);
            ValidarDisponivel(quantidade);

            var bloqueio = new Bloqueio(quantidade);
            _bloqueios.Add(bloqueio.Id, bloqueio); //adicionando o id do bloqueio no dicionário

            Disponivel -= quantidade;
            Bloqueada += quantidade;

            return bloqueio;
        }


    }
}
