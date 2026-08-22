# TransparenciaBot

Projeto de Trabalho de Conclusão de Curso (TCC) do curso de Análise e Desenvolvimento de Sistemas da Ulbra.

O TransparenciaBot é um chatbot integrado ao WhatsApp para facilitar o acesso do cidadão a informações públicas sobre deputados federais. O sistema recebe uma pergunta, consulta fontes oficiais e envia uma resposta clara, indicando a origem dos dados.

## Tecnologias utilizadas

- C#
- ASP.NET Core .NET 8
- PostgreSQL
- Entity Framework Core
- API de Dados Abertos da Câmara dos Deputados
- WhatsApp Business Cloud API
- ngrok
- Git e GitHub

## Funcionalidades

- Receber mensagens enviadas ao WhatsApp do projeto.
- Registrar mensagens no banco de dados.
- Registrar o estado: `Recebida`, `EmProcessamento`, `Respondida` ou `Falhou`.
- Consultar dados cadastrais de deputados federais.
- Consultar gastos parlamentares importados da fonte oficial da Câmara.
- Retornar respostas em linguagem clara e com indicação de fonte.
- Registrar falhas de processamento.
- Evitar resposta duplicada caso a Meta reenvie o mesmo evento.

## Consultas disponíveis

Envie uma destas mensagens para o WhatsApp do projeto:

```text
AJUDA
dados do deputado Erika Hilton
gastos do deputado Erika Hilton em 2025
gastos do deputado Kim Kataguiri
```

Na consulta de gastos sem ano, o sistema utiliza o último ano disponível na base local.

## Fontes públicas

- API de Dados Abertos da Câmara dos Deputados: dados cadastrais.
- Arquivo anual oficial de cotas parlamentares da Câmara dos Deputados: gastos parlamentares.

## Pré-requisitos

- .NET SDK 8
- PostgreSQL
- Conta configurada na WhatsApp Business Cloud API
- ngrok instalado para testes locais

## Configuração local

1. Crie o arquivo:

```text
appsettings.Development.json
```

2. Use `appsettings.Development.example.json` como modelo.

3. Preencha a senha local do PostgreSQL e as configurações da Meta.

4. Aplique as migrations:

```powershell
dotnet ef database update
```

## Importação de gastos

Para a primeira importação de um ano:

```powershell
dotnet run -- --importar-gastos 2025
```

Para substituir com segurança os gastos já importados de um ano:

```powershell
dotnet run -- --reimportar-gastos 2025
```

A reimportação substitui somente os registros de gastos do ano informado.

## Execução local

No terminal, dentro da pasta do projeto:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
```

A aplicação ficará disponível em:

```text
http://localhost:5000
```

Em outro terminal:

```powershell
ngrok http 5000
```

No painel da Meta, configure a URL do webhook com o final:

```text
/api/whatsapp/webhook
```

Exemplo:

```text
https://sua-url-ngrok.ngrok-free.dev/api/whatsapp/webhook
```

## Segurança

O arquivo `appsettings.Development.json` contém credenciais locais e não deve ser enviado ao GitHub, incluído em arquivos `.zip`, artigo ou apresentação.

O telefone do usuário é registrado apenas como hash, sem guardar o número original.

## Publicação na Render

O projeto pode ser publicado na Render usando o `Dockerfile` incluído no repositório.

1. Crie um PostgreSQL gerenciado na Render.
2. Crie um **Web Service** conectado ao repositório GitHub do projeto. A Render identificará o `Dockerfile`.
3. Cadastre as variáveis de ambiente no painel da Render. Nunca crie ou envie um arquivo `appsettings.Production.json` com dados reais para o GitHub.

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__TransparenciaBotDb=STRING_DE_CONEXAO_DO_POSTGRES
WhatsApp__VerifyToken=SEU_VERIFY_TOKEN
WhatsApp__AccessToken=SEU_ACCESS_TOKEN_DA_META
WhatsApp__PhoneNumberId=SEU_PHONE_NUMBER_ID_DA_META
Aplicacao__AplicarMigrationsAoIniciar=true
```

4. No primeiro deploy, mantenha `Aplicacao__AplicarMigrationsAoIniciar=true` para criar as tabelas no banco novo. Após a confirmação do deploy, altere-a para `false` e faça novo deploy.
5. Use `GET /api/health` como Health Check Path.
6. Após a publicação, configure no painel da Meta a URL pública final seguida de `/api/whatsapp/webhook`.
7. Importe os gastos do ano no banco publicado antes de testar consultas de despesas.

O arquivo `render.yaml` contém apenas nomes de variáveis e não contém segredos.

## Fluxo do sistema

```text
WhatsApp
→ Webhook do TransparenciaBot
→ Registro da mensagem no PostgreSQL
→ Interpretação da consulta
→ Consulta à API da Câmara ou à base local de gastos
→ Resposta clara com fonte
→ Envio da resposta pelo WhatsApp
```
