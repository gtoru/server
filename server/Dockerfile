FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /app

COPY server.sln .
COPY src/server.core/server.core.csproj src/server.core/server.core.csproj

RUN dotnet restore

COPY . /app

RUN dotnet build -c Release --no-restore
RUN dotnet publish "src/server.core/server.core.csproj" -c Release --output "./dist"

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as deploy

ENV ASPNETCORE_ENVIRONMENT Development
ENV DB__Host postgre
ENV DB__Port 5432

COPY --from=build /app/dist /app
WORKDIR /app

CMD ["dotnet", "server.core.dll"]
