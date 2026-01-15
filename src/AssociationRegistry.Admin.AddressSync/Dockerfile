

FROM mcr.microsoft.com/dotnet/sdk:9.0.302 AS base
WORKDIR /

COPY paket.* .
COPY *.sln .
COPY .config .

RUN dotnet tool restore

ARG BUILD_PROJECT=AssociationRegistry.Admin.AddressSync
ARG ELASTIC_HOST
ARG CI_BUILD_NUMBER
ARG GIT_HASH

COPY . .

RUN dotnet restore
RUN dotnet dotnet-script .github/build-scripts/SetSolutionInfo.csx


FROM base AS src-build

# First do a regular (framework-dependent) build so assemblies are available for codegen
RUN dotnet build -c Release --no-restore -f net9.0 src/${BUILD_PROJECT}/${BUILD_PROJECT}.csproj

# Now run codegen with the built assemblies
WORKDIR /src/${BUILD_PROJECT}
RUN dotnet run -c Release --no-build -- codegen write
WORKDIR /

# Now do the self-contained build for deployment
RUN dotnet build -c Release --no-restore --runtime "linux-x64" --self-contained -f net9.0 src/${BUILD_PROJECT}/${BUILD_PROJECT}.csproj
RUN dotnet publish -c Release -o /app/publish --no-build --no-restore --runtime "linux-x64" --self-contained -f net9.0 src/${BUILD_PROJECT}/${BUILD_PROJECT}.csproj

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS final

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

WORKDIR /app

COPY --from=src-build /app/publish .

ENTRYPOINT ["/app/AssociationRegistry.Admin.AddressSync"]
