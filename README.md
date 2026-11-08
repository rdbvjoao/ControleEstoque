# Controle de Estoque

Solução do case prático de Engenharia de Software (Itaú Investment Services), implementada em **C# / .NET 8**.

## Visão geral

A solução controla o estoque de produtos considerando quatro quantidades por produto:

- **Total**: todas as unidades que existem no estoque
- **Disponível**: unidades livres para uso
- **Reservada**: unidades separadas para um pedido (cada reserva tem identificador)
- **Bloqueada**: unidades fora de uso (cada bloqueio tem identificador)

A regra central, que vale depois de qualquer operação (inclusive as que falham) é:

```
total = disponível + reservada + bloqueada
```

Operações disponíveis: **Entrada**, **Saída**, **Reservar**, **Bloquear**, **Liberar** (reserva ou bloqueio), **Consultar** o produto e consultar uma **reserva** ou um **bloqueio** pelo identificador.

| Operação | Efeito |
|---|---|
| Entrada | aumenta total e disponível |
| Saída | reduz total e disponível (somente o que está disponível) |
| Reservar | move do disponível para reservada e devolve a reserva criada |
| Bloquear | move do disponível para bloqueada e devolve o bloqueio criado |
| Liberar | remove a reserva ou o bloqueio e devolve a quantidade ao disponível |
| Consultar | devolve total, disponível, reservada e bloqueada |

## Arquitetura e estrutura

```
ControleEstoque.sln
├── Estoque.Dominio        regras de negócio, serviço e repositório em memória
├── Estoque.Dominio.Tests  testes automatizados (xUnit)
└── Estoque.Cli            console de demonstração
```

Principais classes de `Estoque.Dominio`:

- **`Produto`**: guarda as quantidades, as reservas e os bloqueios, e contém todas as regras. As quantidades só podem ser alteradas por seus métodos (`private set`)
- **`Reserva` e `Bloqueio`**: objetos imutáveis, com `Id` (`Guid`) gerado na criação. A reserva guarda o `PedidoId`, e o bloqueio pode guardar um `SolicitacaoId` opcional
- **`EstoqueService`**: porta de entrada da solução. Localiza o produto e repassa a operação, sem repetir regras de negócio
- **`IProdutoRepository` e `ProdutoRepositoryEmMemoria`**: acesso aos produtos atrás de uma interface
- **`SituacaoProduto`**: `record` com a foto da situação do produto, devolvido pela consulta
- **Exceções de domínio**: `QuantidadeInvalidaException`, `EstoqueInsuficienteException`, `ReservaNaoEncontradaException`, `BloqueioNaoEncontradoException`, `ProdutoNaoEncontradoException` e `IdentificadorInvalidoException`

O domínio não depende do projeto de testes nem do console. Quem depende dele são os outros dois.

## Principais decisões técnicas

- **Validar antes de alterar.** Toda operação faz as validações primeiro e só depois muda os números. Assim, uma operação inválida não deixa o estoque inconsistente.
- **Estado protegido por encapsulamento.** As quantidades só mudam por métodos do `Produto` (`private set`), o `Id` é somente leitura e validado no construtor, e `Reserva`, `Bloqueio` e `SituacaoProduto` são imutáveis. Isso impede alterações externas que quebrem a regra `total = disponível + reservada + bloqueada`.
- **Regras dentro do `Produto`.** O serviço localiza o produto (criando-o na primeira entrada) e repassa a operação, sem repetir regras de estoque. Isso mantém as regras em um único lugar e fáceis de testar.
- **Exceções próprias para cada regra violada.** Elas nomeiam o problema de negócio, impedem que o erro seja ignorado e permitem que os testes verifiquem exatamente o erro esperado. Em um cenário de alto volume, em que falhas esperadas fossem frequentes, um tipo de resultado (sem exceções) seria uma alternativa.
- **Identificadores com `Guid`.** Reservas e bloqueios têm ids únicos gerados automaticamente, sem contador nem banco de dados. Reservas e bloqueios ficam em coleções separadas, então o id de um tipo não libera o outro.
- **Persistência em memória atrás de uma interface.** O foco do case é a lógica de negócio. A interface `IProdutoRepository` permite trocar por um banco sem alterar as regras.
- **Consulta devolve uma cópia dos números.** `SituacaoProduto` é um `record` imutável, então quem consulta não consegue alterar o produto. A leitura das quatro quantidades é feita em uma única operação, para formar uma foto consistente.
- **Concorrência.** Cada `Produto` usa um `lock` próprio em todas as operações, e a validação e a alteração acontecem juntas dentro dele, então duas operações simultâneas no mesmo produto nunca se misturam. Operações em produtos diferentes não se bloqueiam, com uma exceção: a `Entrada` passa por um `lock` global no serviço, que torna "procurar, criar e adicionar" um passo indivisível quando o produto é novo. O repositório usa `ConcurrentDictionary`. Os limites estão em "Limitações conhecidas".
- **Desenvolvimento guiado por testes.** Cada operação foi construída escrevendo o teste primeiro, o que deu uma suíte que protege a regra de consistência e expôs defeitos reais, como a saída com quantidade negativa e a corrida na criação do produto.
- **Console em vez de API REST.** O enunciado deixa a forma de exposição livre e avalia principalmente o domínio, então o console serve apenas para demonstrar a solução rodando.
- **Refatoração pontual.** Validações repetidas foram extraídas (`ValidarQuantidade`, `ValidarDisponivel`, `Validacoes.ExigirTexto`). Já `Reservar` e `Bloquear` ficaram parecidos de propósito: unificá-los traria mais abstração sem benefício claro.

## Premissas consideradas

O enunciado deixa alguns pontos em aberto. Estas foram as escolhas feitas:

1. Quantidades são números inteiros maiores que zero. Zero ou negativo gera erro.
2. A **Entrada cria o produto** se ele ainda não existir. As demais operações em produto inexistente geram erro.
3. A **Saída** só retira unidades disponíveis, então ela não consome unidades reservadas nem bloqueadas.
4. O sistema gera o id de cada reserva e de cada bloqueio. A reserva também guarda o id do pedido, informado por quem reserva, e um pedido pode ter mais de uma reserva.
5. **Liberar** devolve a quantidade ao disponível e remove a reserva ou o bloqueio. Liberar um id inexistente ou já liberado gera erro, o que evita devolver a mesma quantidade duas vezes.
6. O id de uma reserva não libera um bloqueio, e vice-versa. Cada tipo só reconhece os próprios ids.
7. Os ids de produto e de pedido são obrigatórios: nulo, vazio ou só espaços geram erro.
8. Toda operação inválida lança exceção e não altera o estoque. Se a Entrada falhar em um produto novo, o produto não chega a ser criado.
9. O enunciado não deixa claro se o bloqueio também se associa a solicitações, então a associação é **opcional**: o bloqueio pode ser criado com ou sem `SolicitacaoId` e, quando informado, o valor não pode ser vazio nem só espaços.

## Limitações conhecidas

Cada limitação abaixo é uma escolha de escopo e vem com o caminho de evolução:

- **Dados somente em memória.** O foco do case é a lógica de negócio, e o acesso aos dados fica atrás de `IProdutoRepository`. Os dados se perdem ao encerrar a aplicação. Em produção, seria um banco relacional.
- **Consistência garantida apenas dentro de um processo.** O `lock` de cada produto evita estados inconsistentes em uma instância, mas não coordena várias instâncias. Evolução: transações do banco com controle otimista de versão.
- **Entradas serializadas por um lock global.** Toda chamada de `Entrada` passa por um único `lock` no serviço, para tornar "procurar, criar e adicionar" um passo indivisível. Entradas de produtos diferentes esperam umas pelas outras (as demais operações não). Evolução: restringir o lock global à criação do produto.
- **Efetivar uma reserva exige dois passos.** Para retirar unidades reservadas, hoje é preciso liberar a reserva e fazer a saída, e entre os dois passos outra operação poderia ocupar as unidades liberadas. Evolução: uma operação única que consuma a reserva de forma atômica.
- **Liberação sempre total.** Libera-se a reserva ou o bloqueio inteiro. Evolução: aceitar uma quantidade e reduzir a reserva ou o bloqueio.
- **Sem histórico e sem motivo no bloqueio.** O bloqueio guarda apenas a solicitação opcional. Evolução: registro das movimentações (data, quantidade e responsável) e motivo no bloqueio.
- **Sem proteção contra estouro do tipo `int`.** Quantidades muito grandes poderiam estourar sem erro. Evolução: aritmética verificada (`checked`) ou `long`, com um limite máximo validado.
- **Console com cenário fixo.** Não há API nem entrada interativa de dados, porque o enunciado deixa a forma de exposição livre e avalia principalmente o domínio. Evolução: uma API fina sobre o `EstoqueService`.
- **Testes de concorrência probabilísticos.** Um resultado verde aumenta a confiança, mas não prova a ausência de falhas. Evolução: repeti-los em pipeline de integração contínua e testar contra o banco real.

## Estratégia de testes

Os testes automatizados (xUnit) cobrem:

- o caminho feliz de cada operação, conferindo as quatro quantidades após a execução;
- valores limite (por exemplo, reservar um acima do disponível);
- entradas inválidas (zero, negativo, ids vazios), conferindo que o **estoque não mudou** após o erro;
- vários bloqueios e reservas simultâneos no mesmo produto (inclusive de solicitações diferentes, e bloqueios com e sem solicitação), e a regra de que saída só retira o disponível;
- liberação de ids inexistentes, repetidos e de tipo trocado;
- o serviço: criação de produto, produto inexistente, e um ciclo completo de reservar, bloquear, consultar e liberar;
- concorrência, com `Parallel.For`: reservas simultâneas não ultrapassam o disponível, e entradas simultâneas em produto novo não se perdem.

## Como executar

Pré-requisito: **.NET 8 SDK**. Execute os comandos na raiz do repositório (a pasta onde está o `ControleEstoque.sln`):

```
dotnet build
dotnet test
dotnet run --project Estoque.Cli
```

O console executa um **cenário fixo de demonstração**, sem entrada de dados pelo teclado: entrada, reserva, bloqueio, saída, uma reserva recusada por falta de estoque disponível e as liberações. No final, imprime uma tabela com as quatro quantidades após cada passo.

O enunciado não exige entrada interativa, então a solução não oferece um menu. Para ver outros cenários, os testes em `Estoque.Dominio.Tests` mostram os casos de cada operação (inclusive os de erro), e o `EstoqueService` pode ser usado diretamente por outro programa.

## O que mudaria em um ambiente de produção

Em ordem de prioridade, começando pelo que protege a correção do estoque:

- **Banco de dados relacional.** Hoje os dados ficam em memória e somem ao fechar o programa. Em produção, cada operação seria uma **transação** (tudo é salvo ou nada é salvo). A regra "não passar do disponível" seria feita pelo próprio banco, em um único comando que só altera se houver quantidade disponível suficiente. O banco também reforçaria a regra `total = disponível + reservada + bloqueada`. Assim a consistência vale mesmo com várias instâncias da aplicação, e o `lock` em memória deixa de ser necessário.
- **Idempotência.** Se quem chama repetir uma requisição (por exemplo, depois de uma falha de rede), o sistema não pode criar duas reservas. Cada operação receberia um código único, e a repetição devolveria o resultado já produzido.
- **Expiração de reservas.** Reservas esquecidas prendem o estoque para sempre. Elas teriam prazo de validade e seriam liberadas automaticamente.
- **Operações que o enunciado não pediu, mas o negócio precisa:** efetivar uma reserva em um único passo (consumi-la em uma saída), liberar apenas parte de uma reserva ou bloqueio, e registrar o motivo e o responsável de cada bloqueio.
- **Histórico de movimentações.** Registrar cada entrada, saída, reserva, bloqueio e liberação, com data, quantidade e responsável, para auditoria e para descobrir o que aconteceu quando os números divergirem.
- **API.** Expor as operações por HTTP, com validação dos dados recebidos, erros de negócio traduzidos para códigos HTTP (por exemplo, 404 para produto inexistente e 409 para estoque insuficiente) e controle de acesso por perfil.
- **Limite nas quantidades.** Usar `long` ou validar um valor máximo, para evitar estouro.
- **Observabilidade e entrega.** Logs, métricas e alertas, além de testes e atualizações do banco rodando automaticamente em um pipeline de integração contínua.
- **Testes de concorrência e de carga contra o banco real**, em vez de apenas em memória.

## Documentos adicionais

- [Uso de IA](docs/USO_DE_IA.md)
- [Visão de engenharia](docs/VISAO_DE_ENGENHARIA.md)