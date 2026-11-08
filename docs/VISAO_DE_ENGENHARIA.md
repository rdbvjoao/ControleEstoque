# Visão de engenharia

Os cinco aspectos que considero mais importantes para a qualidade, a eficiência, a confiabilidade e a evolução desta solução, em ordem de prioridade.

## 1. Consistência do estoque

**Por que importa:** em um controle de estoque, o pior defeito é o número errado, porque leva a vender o que não existe ou a perder unidades sem rastro. A regra `total = disponível + reservada + bloqueada` precisa valer depois de qualquer operação, inclusive as que falham.

**Como a solução trata:** as quantidades só são alteradas dentro do `Produto` (`private set`), e toda operação **valida antes de alterar**. Uma operação inválida lança exceção e deixa o estoque exatamente como estava. Até a criação de um produto novo segue isso: se a primeira entrada for inválida, o produto não é criado.

## 2. Concorrência e confiabilidade

**Por que importa:** em um sistema real, várias solicitações chegam ao mesmo tempo. Sem controle, duas reservas poderiam sair de uma única unidade, ou entradas simultâneas poderiam se perder.

**Como a solução trata:** cada produto tem um `lock` próprio, e a validação e a alteração acontecem juntas, como um passo indivisível. Operações em produtos diferentes não se bloqueiam, o que ajuda na eficiência, com uma exceção: a `Entrada` passa por um `lock` global no serviço, que torna “procurar, criar e adicionar” um passo único quando o produto é novo. Os testes de concorrência provaram os dois problemas antes da correção (a coleção de reservas sendo alterada por várias threads ao mesmo tempo e entradas perdidas na criação do produto). O limite conhecido é que isso vale em um único processo, e em produção a garantia viria das transações do banco de dados.

## 3. Testes automatizados como especificação

**Por que importa:** os testes documentam as regras de negócio de forma executável e permitem mudar o código sem medo de quebrar o que já funcionava. Foi assim que a falha da saída com quantidade negativa foi confirmada e que a perda de entradas na criação concorrente apareceu.

**Como a solução trata:** o desenvolvimento foi guiado por testes. Cada operação tem o caminho feliz, casos de limite (como reservar um acima do disponível), entradas inválidas e a conferência de que o estoque não mudou após um erro.

## 4. Baixo acoplamento e capacidade de evolução

**Por que importa:** os requisitos mudam, como a troca da persistência, a exposição por uma API ou novas operações. Uma solução que mistura regras com detalhes técnicos fica cara de mudar.

**Como a solução trata:** as regras ficam no `Produto`, o `EstoqueService` localiza o produto (criando-o  na primeira entrada) e repassa a operação, e o acesso aos dados fica atrás da interface `IProdutoRepository`. O domínio não depende do console nem dos testes. Trocar o repositório em memória por um banco, ou colocar uma API na frente do serviço, não exige alterar as regras.

## 5. Rastreabilidade e observabilidade

**Por que importa:** quando o estoque diverge do físico, é preciso descobrir **o que aconteceu e quando**. Sem histórico das movimentações e sem logs, cada divergência vira uma investigação às cegas.

**Como a solução trata hoje:** este aspecto **não está implementado**, e está registrado como evolução no README. Em produção, eu registraria cada movimentação (entrada, saída, reserva, bloqueio e liberação) com data, quantidade e responsável, além de logs estruturados e métricas, e acrescentaria o motivo aos bloqueios.
