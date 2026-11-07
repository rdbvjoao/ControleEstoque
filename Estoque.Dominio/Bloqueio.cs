using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estoque.Dominio;

    public class Bloqueio
    {
        public Guid Id { get; }
        public int Quantidade { get; }
        public string? SolicitacaoId { get; }

        public Bloqueio(int quantidade, string? solicitacaoId = null)
        {
            Id = Guid.NewGuid();
            Quantidade = quantidade;
            SolicitacaoId = solicitacaoId;
        }
    }