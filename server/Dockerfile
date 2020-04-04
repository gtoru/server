FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /app

COPY server.sln .
COPY src/server.core/server.core.csproj src/server.core/server.core.csproj
COPY tests/Domain.Tests/Domain.Tests.csproj tests/Domain.Tests/Domain.Tests.csproj
COPY tests/Infrastructure.Tests/Infrastructure.Tests.csproj tests/Infrastructure.Tests/Infrastructure.Tests.csproj
COPY tests/Application.Tests/Application.Tests.csproj tests/Application.Tests/Application.Tests.csproj

RUN dotnet restore

COPY . /app

RUN dotnet build -c Release --no-restore
RUN dotnet publish "src/server.core/server.core.csproj" -c Release --output "./dist"

FROM build as test

COPY --from=build /app .
CMD ["dotnet", "test", "-c", "Release", "--no-restore"]

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as deploy

COPY --from=build /app/dist /app
CMD ["dotnet", "/app/server.core.dll"]
