# Use .NET 8.0 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files (note corrected spelling of Infrastructure)
COPY ["TransactionService.Api/TransactionService.Api.csproj", "TransactionService.Api/"]
COPY ["TransactionService.Application/TransactionService.Application.csproj", "TransactionService.Application/"]
COPY ["TransactionService.Domain/TransactionService.Domain.csproj", "TransactionService.Domain/"]
COPY ["TransactionService.Infrastructure/TransactionService.Infrastructure.csproj", "TransactionService.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "TransactionService.Api/TransactionService.Api.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/TransactionService.Api"
RUN dotnet build "TransactionService.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TransactionService.Api.csproj" -c Release -o /app/publish

# Runtime stage (using .NET 8.0 runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionService.Api.dll"]