using System.Text;
using Estoque.Dominio;

Console.OutputEncoding = Encoding.UTF8;

var servico = new EstoqueService(new ProdutoRepositoryEmMemoria());
var passos = new List<(string Operacao, SituacaoProduto Situacao)>();
string? erro = null;

void Registrar(string operacao) =>
    passos.Add((operacao, servico.Consultar("P1")));

servico.Entrada("P1", 100);
Registrar("Entrada de 100");

var reserva = servico.Reservar("P1", "PED-1", 30);
Registrar("Reserva de 30 (PED-1)");

var bloqueio = servico.Bloquear("P1", 20);
Registrar("Bloqueio de 20");

servico.Saida("P1", 40);
Registrar("Saída de 40");

try
{
    servico.Reservar("P1", "PED-2", 20);
}
catch (EstoqueInsuficienteException ex)
{
    erro = ex.Message;
}
Registrar("Reserva de 20 (PED-2) recusada");

servico.LiberarReserva("P1", reserva.Id);
Registrar("Liberação da reserva PED-1");

servico.LiberarBloqueio("P1", bloqueio.Id);
Registrar("Liberação do bloqueio");

var larguraOperacao = Math.Max("Operação".Length, passos.Max(p => p.Operacao.Length));
const int larguraNumero = 10;

string Linha(string operacao, string total, string disponivel, string reservada, string bloqueada) =>
    $"{operacao.PadRight(larguraOperacao)}  {total.PadLeft(larguraNumero)}  {disponivel.PadLeft(larguraNumero)}  {reservada.PadLeft(larguraNumero)}  {bloqueada.PadLeft(larguraNumero)}";

Console.WriteLine(Linha("Operação", "Total", "Disponível", "Reservada", "Bloqueada"));
Console.WriteLine(new string('─', larguraOperacao + 4 * (larguraNumero + 2)));

foreach (var passo in passos)
{
    var s = passo.Situacao;
    Console.WriteLine(Linha(passo.Operacao, s.Total.ToString(), s.Disponivel.ToString(), s.Reservada.ToString(), s.Bloqueada.ToString()));
}

if (erro is not null)
{
    Console.WriteLine();
    Console.WriteLine($"Reserva recusada: {erro}");
}