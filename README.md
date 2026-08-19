# TransparenciaBot

Este projeto está sendo desenvolvido como Trabalho de Conclusão de Curso (TCC) do curso de Análise e Desenvolvimento de Sistemas da Ulbra.

O TransparenciaBot é um chatbot integrado ao WhatsApp. A proposta é facilitar o acesso do cidadão a informações públicas sobre deputados federais, principalmente dados de identificação e gastos parlamentares.

A ideia é simples: a pessoa faz uma pergunta pelo WhatsApp, o sistema consulta uma fonte oficial da Câmara dos Deputados e devolve uma resposta clara, indicando a origem da informação.

## Tecnologias utilizadas

- C#
- ASP.NET Core .NET 8
- PostgreSQL
- Entity Framework Core
- API de Dados Abertos da Câmara dos Deputados
- WhatsApp Business Cloud API
- Git e GitHub

## O que já foi desenvolvido

- API criada em ASP.NET Core.
- Banco de dados PostgreSQL configurado.
- Tabelas para usuários, mensagens, falhas de processamento e gastos.
- Registro de mensagens com telefone protegido por hash.
- Consulta de deputado por nome na API da Câmara.
- Importação do arquivo anual oficial de cotas parlamentares da Câmara.
- Consulta de gastos armazenados no banco de dados.
- Retorno da quantidade de gastos, total gasto e maiores despesas.
- Estrutura inicial do webhook do WhatsApp.
- Preparação de uma resposta em linguagem clara para o cidadão.

## Como funciona atualmente

```text
Consulta → API do TransparenciaBot → Banco de dados →
dados oficiais da Câmara → resposta da API