FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.8

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY / /app
WORKDIR /app

ENTRYPOINT ["/app/AssociationRegistry.Admin.AddressSync"]
