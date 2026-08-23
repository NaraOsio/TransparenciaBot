TransparenciaBot

O TransparenciaBot é um projeto de Trabalho de Conclusão de Curso (TCC) do curso de Análise e Desenvolvimento de Sistemas da Ulbra.

O projeto foi criado para facilitar o acesso do cidadão a informações públicas sobre deputados federais. Por meio do WhatsApp, a pessoa pode pesquisar um deputado e receber dados oficiais de forma rápida e simples.

Objetivo

Muitas informações públicas já estão disponíveis na internet, mas nem sempre são fáceis de localizar ou compreender.

O TransparenciaBot aproxima esses dados do cidadão. Em vez de procurar em diferentes páginas, basta enviar uma mensagem pelo WhatsApp para consultar informações sobre deputados federais e seus gastos parlamentares.

Como funciona

O cidadão envia uma mensagem pelo WhatsApp.

O bot identifica a consulta, busca os dados nas fontes oficiais da Câmara dos Deputados e envia uma resposta clara pelo próprio WhatsApp.

O sistema consulta:

 dados do deputado, como nome, partido e estado;
 quantidade de gastos;
 valor total gasto;
 maiores despesas parlamentares.

Exemplos de consultas

O cidadão pode enviar apenas o nome do deputado:

text
Maria do Rosário

Também pode informar a sigla do partido:

text
Maria do Rosário PT

Nesse caso, o bot apresenta um resumo com os dados do deputado e os gastos do último ano disponível.

Para consultar apenas os dados do deputado:

text
dados do deputado Erika Hilton

Para consultar os gastos detalhados:

text
gastos do deputado Erika Hilton em 2025

text
gastos do deputado Kim Kataguiri

Para visualizar exemplos de uso:
text
AJUDA

Informações apresentadas

Ao pesquisar um deputado, o cidadão recebe:

nome;
partido;
UF, que representa o estado do deputado;
quantidade de gastos;
total gasto no ano;
orientação para consultar as maiores despesas.

O bot consulta somente deputados federais. Deputados estaduais, senadores, prefeitos e vereadores não fazem parte da base utilizada pelo projeto.

Fontes dos dados

O TransparenciaBot utiliza fontes oficiais da Câmara dos Deputados:

API de Dados Abertos da Câmara dos Deputados, para dados cadastrais dos parlamentares;
arquivo anual oficial de cotas parlamentares, para consulta dos gastos.

O sistema não julga se uma despesa é certa ou errada. Seu objetivo é facilitar o acesso e a compreensão de informações públicas.

Tecnologias utilizadas

C#
ASP.NET Core .NET 8
PostgreSQL
Entity Framework Core
WhatsApp Business Cloud API
API de Dados Abertos da Câmara dos Deputados
Render
Git e GitHub

Segurança

Senhas, tokens da Meta e dados de conexão com o banco não são enviados ao GitHub. Essas informações ficam protegidas nas variáveis de ambiente da plataforma de hospedagem.

O telefone do usuário é registrado apenas como hash, sem guardar o número original no banco de dados.

Fluxo do sistema

text
Cidadão envia uma mensagem no WhatsApp
        ↓
O webhook recebe a mensagem
        ↓
O sistema identifica o tipo de consulta
        ↓
Consulta a API da Câmara ou a base de gastos
        ↓
Envia uma resposta clara pelo WhatsApp


Situação atual

O TransparenciaBot está publicado e funcionando.

Foram realizados testes de:

consulta por nome;
consulta com sigla de partido;
dados de deputados;
gastos detalhados;
mensagens fora do escopo;
conexão com banco de dados;
envio de mensagens pelo WhatsApp.

Atualmente, o sistema possui os gastos parlamentares de 2025 disponíveis para consulta.
