FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Bookstore.IdentityServer/Bookstore.IdentityServer.csproj", "src/Bookstore.IdentityServer/"]
COPY ["src/Bookstore.Shared/Bookstore.Shared.csproj", "src/Bookstore.Shared/"]
RUN dotnet restore "src/Bookstore.IdentityServer/Bookstore.IdentityServer.csproj"
COPY . .
RUN dotnet publish "src/Bookstore.IdentityServer/Bookstore.IdentityServer.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.IdentityServer.dll"]
