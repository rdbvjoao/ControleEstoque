# Uso de Inteligência Artificial

## Ferramenta e forma de uso

Usei o **Claude** (chat da Anthropic) como apoio durante todo o case, em dois papéis: **tutor**, para interpretar o enunciado e decidir pontos em aberto, e **par de programação**, para construir a solução em ciclos curtos de teste primeiro (escrever o teste, vê-lo falhar, escrever o mínimo de código para passar e refatorar).

O código foi proposto pelo Claude em pequenos passos. Em cada passo, eu criei os arquivos no Visual Studio, executei os testes, conferi o resultado (vermelho quando esperado, verde depois) e pedi explicações quando algo não estava claro. Também revisei o código e os documentos por conta própria e fiz ajustes, como a correção de erros de digitação que apareceram na compilação e na revisão.

## Principais prompts

Selecionei os prompts que tiveram impacto nas decisões ou na implementação, como o enunciado do case orienta. Eles estão levemente resumidos. Além deles, usei o Claude para esclarecer conceitos à medida que apareciam (xUnit, `lock`, `Guid` e `record`, entre outros).

**1. Interpretar o case e conferir os requisitos**

- Pedi ajuda para interpretar o enunciado e escolher a linguagem entre as três permitidas.
  *Impacto:* defini o modelo mental (total = disponível + reservada + bloqueada) e escolhi C#, por ser a linguagem do meu currículo e do meu TCC e também uma das linguagens que a vaga valoriza.
- "Poderia, por favor, fazer um double-check dos requisitos do case técnico?"
  *Impacto:* a revisão apontou que reserva e bloqueio também precisam ser **consultados pelo identificador**, e que eu precisava de um ponto de entrada executável, não só testes.
- "O PDF menciona bloqueios associados a diferentes solicitações? No código escrito é possível observar isso?"
  *Impacto:* percebi uma ambiguidade do enunciado que a modelagem inicial não tinha tratado. O bloqueio passou a ter uma solicitação opcional.

**2. Decisões técnicas e de escopo**

- "No PDF do case é mencionado alguma coisa a respeito de banco de dados?"
  *Impacto:* optei por persistência em memória atrás de uma interface, documentando isso no README.
- "Tenho dúvidas se utilizar a API REST e isso fazer com que ficasse a solução complexa."
  *Impacto:* decidi priorizar o domínio e os testes e usar um console de demonstração. A API ficou como evolução.
- "É realmente necessário um menu interativo ou o console em si já está dentro do que foi pedido no case?"
  *Impacto:* conferi o PDF, que não exige entrada interativa, e optei por um cenário fixo de demonstração, explicando essa escolha no README.
- "Considerando-se os requisitos do case e também da vaga, seria bom eu demonstrar o meu conhecimento em xUnit e em testes unitários?"
  *Impacto:* priorizei os testes unitários e o uso correto de C# e .NET, que são o que a vaga pede de forma explícita.
- "Concordo com as premissas, desde que estejam de acordo com os requisitos pedidos no documento do case técnico."
  *Impacto:* conferi cada premissa contra o PDF e separei o que vem do enunciado do que foi escolha minha.

**3. Construção guiada por testes**

- "Vamos fazer passo a passo para eu entender também o que foi foi proposto", e pedidos para explicar o código antes de seguir.
  *Impacto:* o Claude propôs os testes de cada operação antes do código, e eu os revisei, executei e acompanhei cada ciclo. Depois, escrevi por conta própria testes de valor limite e de invariante, e usei o Claude para revisá-los.

**4. Revisão e acabamento**

- Revisão de seção por seção do README.
  *Impacto:* a revisão encontrou imprecisões, como a descrição do `lock` da `Entrada`, e deixou as limitações e a seção de produção coerentes entre si.

## Análise

### Onde a IA mais ajudou

- **Interpretar o enunciado:** identificar os pontos em aberto (quantidade zero, produto inexistente, liberação repetida, saída com reserva) e transformá-los em premissas documentadas.
- **Conferir os requisitos contra o PDF**, o que mostrou itens que eu poderia ter deixado passar, como a consulta de reserva e bloqueio por identificador.
- **Estruturar o trabalho em ciclos de teste primeiro**, o que me deu confiança a cada passo e uma suíte que protege a regra de consistência.
- **Explicar conceitos novos no momento em que eram usados** (`[Theory]`, `Guid`, `lock`, `ConcurrentDictionary`, `record`).
- **Antecipar problemas de concorrência**, propondo testes que de fato falharam antes da correção (uma exceção por coleção alterada por várias threads e 823 entradas em vez de 1000 em um produto novo).

### Sugestões que modifiquei ou não utilizei

- **API REST:** foi apresentada como opcional. Decidi não implementá-la, para manter o foco no domínio e nos testes, e a descrevi como evolução no README.
- **Menu interativo no console:** foi apresentado como um possível acabamento. Não implementei, porque o enunciado não pede e queria concentrar o tempo no que é avaliado.
- **Motivo do bloqueio:** foi mencionado como campo opcional. Não implementei, porque o enunciado não pede. Está listado nas limitações.
- **Saída do console:** a primeira proposta foi uma tabela com bordas, troquei pela versão minimalista, sem bordas.
- **Fixação do SDK (`global.json`):** a primeira versão  prendia a versão exata `8.0.427`, o que poderia impedir a execução em máquinas com outro SDK 8. A própria IA percebeu o problema ao escrever as instruções de execução e ajustei para aceitar qualquer SDK 8.0.x.

### Erros e limitações identificados

- **Saída sem validação de quantidade negativa:** a primeira versão do método `Saida` aceitava `-5`, o que **aumentaria** o estoque. A falha foi apontada e comprovada por um teste que ficou vermelho, e foi corrigida extraindo a validação para um método reutilizado.
- **Ambiguidade do enunciado não sinalizada:** a primeira modelagem assumiu que o bloqueio não se associa a solicitações, sem avisar que o PDF era ambíguo nesse ponto (a seção 2 fala em reservas e bloqueios "associados a solicitações diferentes"). Percebi ao reler o PDF e perguntei. O bloqueio passou a ter uma solicitação opcional, com testes para os dois casos, e a premissa foi registrada no README.
- **Resposta baseada no código proposto, e não no meu arquivo:** durante a revisão, surgiu a observação de que a propriedade `Produto.Id` poderia ter `set` público, o que permitiria trocar o id depois de o produto estar no repositório. A IA respondeu que o `Id` só tinha `get`, mas isso era o código que ela havia proposto, e não o que estava no meu arquivo, onde um erro de digitação meu tinha deixado um `set`. Abri o arquivo, confirmei o erro e corrigi. A mesma revisão levou a fechar uma lacuna parecida, o construtor do `Produto` passou a validar o id, com testes.
- **Observação que não procedia:** outra observação afirmava que o método `Saida` tinha uma validação duplicada. Conferi no meu arquivo e não havia duplicação, então não alterei nada. Em vez de aceitar ou descartar de imediato, confirmei contra o código.
- **Descrição imprecisa do `lock` no README:** o primeiro rascunho do README descrevia o `lock` do serviço como se protegesse apenas a criação do produto, mas ele envolve a `Entrada` inteira, serializando entradas de produtos diferentes. A imprecisão apareceu na revisão e foi corrigida no README, nas limitações e na visão de engenharia.
- **Instruções em várias partes:** na etapa de concorrência, a mudança foi passada em três partes (o `Produto`, o repositório e o serviço). Na minha cópia, o `Produto.cs` ficou sem o método `ObterSituacao`, e a compilação falhou. Adicionei o método e conferi que os `lock` estavam em todas as operações. A lição foi conferir cada arquivo contra a instrução completa antes de compilar.
- **Teste de concorrência é probabilístico:** a própria IA alertou que um teste desse tipo pode passar por sorte. Por isso o README registra essa limitação, e a suíte foi executada várias vezes durante o trabalho.


