FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY tools/SeedTestData/SeedTestData.csproj tools/SeedTestData/
COPY src/AssociationRegistry/AssociationRegistry.csproj src/AssociationRegistry/
COPY src/AssociationRegistry.MartenDb/AssociationRegistry.MartenDb.csproj src/AssociationRegistry.MartenDb/
COPY src/AssociationRegistry.Admin.Schema/AssociationRegistry.Admin.Schema.csproj src/AssociationRegistry.Admin.Schema/
COPY src/AssociationRegistry.Contracts/AssociationRegistry.Contracts.csproj src/AssociationRegistry.Contracts/
COPY src/AssociationRegistry.OpenTelemetry/AssociationRegistry.OpenTelemetry.csproj src/AssociationRegistry.OpenTelemetry/
COPY src/AssociationRegistry.Hosts/AssociationRegistry.Hosts.csproj src/AssociationRegistry.Hosts/
COPY src/integrations/AssociationRegistry.Integrations.Grar/AssociationRegistry.Integrations.Grar.csproj src/integrations/AssociationRegistry.Integrations.Grar/
COPY Directory.Build.props Directory.Packages.props ./

# Restore dependencies
RUN dotnet restore tools/SeedTestData/SeedTestData.csproj

# Copy everything else
COPY tools/SeedTestData/ tools/SeedTestData/
COPY src/ src/

# Build
RUN dotnet publish tools/SeedTestData/SeedTestData.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SeedTestData.dll"]
