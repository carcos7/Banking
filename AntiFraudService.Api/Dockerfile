# Build stage (using .NET 8.0 SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (order matters for layer caching)
COPY ["AntiFraudService.Api/AntiFraudService.Api.csproj", "AntiFraudService.Api/"]
COPY ["AntiFraudService.Application/AntiFraudService.Application.csproj", "AntiFraudService.Application/"]
COPY ["AntiFraudService.Domain/AntiFraudService.Domain.csproj", "AntiFraudService.Domain/"]
COPY ["AntiFraudService.Infrastructure/AntiFraudService.Infrastructure.csproj", "AntiFraudService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "AntiFraudService.Api/AntiFraudService.Api.csproj"

# Copy remaining source code
COPY . .

# Build and publish
WORKDIR "/src/AntiFraudService.Api"
RUN dotnet publish "AntiFraudService.Api.csproj" -c Release -o /app/publish

# Runtime stage (using .NET 8.0 ASP.NET runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AntiFraudService.Api.dll"]