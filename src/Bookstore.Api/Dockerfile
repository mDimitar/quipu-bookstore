FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Bookstore.Api/Bookstore.Api.csproj", "src/Bookstore.Api/"]
COPY ["src/Bookstore.Shared/Bookstore.Shared.csproj", "src/Bookstore.Shared/"]
RUN dotnet restore "src/Bookstore.Api/Bookstore.Api.csproj"
COPY . .
RUN dotnet publish "src/Bookstore.Api/Bookstore.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.Api.dll"]
