# BatchLab

## Visão geral

BatchLab é uma API de processamento assíncrono em lote. O sistema recebe solicitações de processamento (jobs), publica essas tarefas em uma fila, processa em background e permite acompanhar o status via API e UI simples.

O foco é **entrega funcional em 4 dias**, com arquitetura clara, escopo controlado e uso exclusivo de **serviços gerenciados**, compatíveis com GitHub Codespaces.

---

## Stack definitiva

### Backend

* .NET 8 (ASP.NET Core Minimal API)
* AWS SDK for .NET
* Mensageria: AWS SQS (Standard Queue)
* Persistência: AWS DynamoDB

### Frontend

* Angular (standalone, sem Nx)
* Comunicação via HTTP com a API

### Ambiente

* GitHub Codespaces
* Execução local via `dotnet run` e `ng serve`
* Configuração por variáveis de ambiente

### Fora do escopo técnico

* Docker como requisito de desenvolvimento
* Cache (Redis)
* Autenticação/autorização
* Observabilidade avançada
* Infra como código

---

## Artefato final

### O que o sistema faz

1. Usuário cria um job de processamento em lote
2. API registra o job como `Queued`
3. API publica mensagem no SQS
4. Worker consome a mensagem
5. Processamento simulado é executado
6. Status do job é atualizado
7. UI exibe o status

---

## MVP mínimo aceitável

### Backend

* `POST /jobs` – cria um job
* `GET /jobs/{id}` – consulta status
* Publicação no SQS
* Worker consumindo mensagens
* Persistência de status no DynamoDB

### Frontend

* Form simples para criação de job
* Visualização de status de um job

### Critério de sucesso do MVP

* Fluxo ponta a ponta funcional sem intervenção manual

---

## Divisão de responsabilidades

### Daniel

* Arquitetura geral
* Backend .NET
* Integração com SQS
* Worker de processamento
* Persistência no DynamoDB
* Garantir funcionamento da Camada 1

### Gabriel

* UI em Angular
* Integração UI ↔ API
* Estrutura visual e fluxo do usuário
* Apoio pontual no backend

Regra explícita: cada um é dono de sua camada. Mudanças cruzadas só com alinhamento.

---

## Organização em camadas

### Camada 1 – Núcleo obrigatório

* API recebe job
* Mensagem publicada no SQS
* Worker consome
* Log de processamento

### Camada 2 – Valor demonstrável

* Persistência de status
* Consulta de job
* UI mínima

### Camada 3 – Extras (somente se sobrar tempo)

* Melhorias de UX
* Retry simples
* Paginação/listagem de jobs

---

## Plano fechado por dia

### Dia 1 – Prova de viabilidade

**Objetivos**

* Repositório criado
* API sobe no Codespaces
* Fila SQS criada
* Mensagem publicada e consumida

**Critério de dia bem-sucedido**

* Console confirma: API → SQS → Worker

**Ponto de corte**

* Se DynamoDB atrasar, status em memória

---

### Dia 2 – Núcleo funcional

**Objetivos**

* DynamoDB funcionando
* Status persistido
* Endpoints completos

**Critério de dia bem-sucedido**

* Fluxo completo via Postman/curl

**Ponto de corte**

* Se UI atrasar, foco total no backend

---

### Dia 3 – UI e estabilização

**Objetivos**

* UI cria job
* UI consulta status
* Tratamento básico de erros

**Critério de dia bem-sucedido**

* Demo funcional sem explicação técnica

**Ponto de corte**

* UI feia é aceitável; UI quebrada não

---

### Dia 4 – Finalização e entrega

**Objetivos**

* README claro
* Script de execução
* Pequena limpeza de código
* Preparação da demo

**Critério de dia bem-sucedido**

* Alguém clona o repositório e roda no Codespaces

---

## Resultado esperado

* Um MVP funcional e demonstrável
* Arquitetura clara de processamento assíncrono
* Aprendizado real em mensageria e cloud
* Entrega concluída em 4 dias sem overengineering
