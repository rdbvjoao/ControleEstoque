using System.Text;
using Estoque.Dominio;

    Console.OutputEncoding = Encoding.UTF8;

    var servico = new EstoqueService(new ProdutoRepositoryEmMemoria());

    void Mostrar(string titulo)
    {
      var serv = servico.Consultar("P1");

        Console.WriteLine(
          $"{titulo,-30} Total={serv.Total,4}  Disponivel={serv.Disponivel,4}" +
          $"  Reservada={serv.Reservada,4}  Bloqueada={serv.Bloqueada,4}");

    }

    servico.Entrada("P1", 100);
    Mostrar("Entrada de 100");


    var reserva = servico.Reservar("P1","PED-1", 30);
    Mostrar("Reserva de 30 (PED-1)");


    var bloqueio = servico.Bloquear("P1", 20);
    Mostrar("Bloqueio de 20");


    servico.Saida("P1", 40);
    Mostrar("Saida de 40");


    try
    {   
        servico.Reservar("P1", "PED-2", 20);
    }
    catch (EstoqueInsuficienteException ex)
    {
        Console.WriteLine($"Reserva recusada: {ex.Message}");
    }

    Mostrar("Após a reserva ser recusada");

    servico.LiberarReserva("P1", reserva.Id);
    Mostrar("Liberação da reserva do PED-1");


    servico.LiberarBloqueio("P1", bloqueio.Id);
    Mostrar("Liberaçao do bloqueio");




