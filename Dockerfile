FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/CafeMenu.Web/CafeMenu.Web.csproj", "src/CafeMenu.Web/"]
COPY ["src/CafeMenu.Application/CafeMenu.Application.csproj", "src/CafeMenu.Application/"]
COPY ["src/CafeMenu.Infrastructure/CafeMenu.Infrastructure.csproj", "src/CafeMenu.Infrastructure/"]
COPY ["src/CafeMenu.Domain/CafeMenu.Domain.csproj", "src/CafeMenu.Domain/"]
RUN dotnet restore "src/CafeMenu.Web/CafeMenu.Web.csproj"
COPY . .
WORKDIR "/src/src/CafeMenu.Web"
RUN dotnet build "CafeMenu.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CafeMenu.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CafeMenu.Web.dll"]

