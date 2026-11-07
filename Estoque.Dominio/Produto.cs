using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio
{
    public class Produto
    {
        private readonly object _lock = new();
        private readonly Dictionary<Guid, Reserva> _reservas = new(); //guarda pares de chave e valor no dicionário de reservas
        private readonly Dictionary<Guid, Bloqueio> _bloqueios = new(); //guarda pares de chave e valor no dicionário bloqueios
        public string Id { get; }
        public int Total { get; private set; }
        public int Disponivel { get; private set; }
        public int Reservada { get; private set; }
        public int Bloqueada { get; private set; }


        public Produto(string id)
        {
            Validacoes.ExigirTexto(id, "produto");

            Id = id;
        }

        private static void ValidarQuantidade(int quantidade)
        {
            if (quantidade <= 0)
                throw new QuantidadeInvalidaException(quantidade);
        }

        private void ValidarDisponivel(int quantidade)
        {
            if (quantidade > Disponivel)
                throw new EstoqueInsuficienteException(quantidade, Disponivel);
        }

        public void Entrada(int quantidade)
        {
            lock (_lock)
            {
                ValidarQuantidade(quantidade);

                Total += quantidade;
                Disponivel += quantidade;
            }

        }

        public void Saida(int quantidade)
        {
            lock (_lock)
            {
                ValidarQuantidade(quantidade);
                ValidarDisponivel(quantidade);

                Total -= quantidade;
                Disponivel -= quantidade;
            }
        }

        public Reserva Reservar(string pedidoId, int quantidade)
        {
            lock (_lock)
            {

                ValidarQuantidade(quantidade);
                Validacoes.ExigirTexto(pedidoId, "pedido");
                ValidarDisponivel(quantidade);

                var reserva = new Reserva(pedidoId, quantidade);
                _reservas.Add(reserva.Id, reserva); //adicionando o id da reserva do pedido no dicionário

                Disponivel -= quantidade;
                Reservada += quantidade;

                return reserva;
            }

        }

        public Reserva ObterReserva(Guid id)
        {
            lock (_lock)
            {

                if (!_reservas.TryGetValue(id, out var reserva)) //procura o id no dicionário
                    throw new ReservaNaoEncontradaException(id);

                return reserva;
            }
        }

        public void LiberarReserva(Guid id)
        {
            lock (_lock)
            {
                var reserva = ObterReserva(id);

                _reservas.Remove(id); //removendo a reserva do dicionário

                Reservada -= reserva.Quantidade;
                Disponivel += reserva.Quantidade;
            }
        }


        public Bloqueio Bloquear(int quantidade, string? solicitacaoId = null)
        {
            lock (_lock)
            {
                ValidarQuantidade(quantidade);
                if (solicitacaoId is not null)
                    Validacoes.ExigirTexto(solicitacaoId, "solicitação");
                ValidarDisponivel(quantidade);

                var bloqueio = new Bloqueio(quantidade, solicitacaoId);
                _bloqueios.Add(bloqueio.Id, bloqueio); //adicionando o id do bloqueio no dicionário

                Disponivel -= quantidade;
                Bloqueada += quantidade;

                return bloqueio;
            }
        }



        public Bloqueio ObterBloqueio(Guid id)
        {
            lock (_lock)
            {

                if (!_bloqueios.TryGetValue(id, out var bloqueio)) //procura o id no dicionário
                    throw new BloqueioNaoEncontradoException(id);

                return bloqueio;
            }
        }


        public void LiberarBloqueio(Guid id)
        {
            lock (_lock)
            {

                var bloqueio = ObterBloqueio(id);

                _bloqueios.Remove(id); //removendo o bloqueio do dicionário

                Bloqueada -= bloqueio.Quantidade;
                Disponivel += bloqueio.Quantidade;
            }
        }
        public SituacaoProduto ObterSituacao()
        {
            lock (_lock)
            {
                return new SituacaoProduto(Id, Total, Disponivel, Reservada, Bloqueada);
            }
        }
    }
}
