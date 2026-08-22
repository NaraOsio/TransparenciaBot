FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TransparenciaBot.csproj", "./"]
RUN dotnet restore "TransparenciaBot.csproj"

COPY . .
RUN dotnet publish "TransparenciaBot.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "TransparenciaBot.dll"]
