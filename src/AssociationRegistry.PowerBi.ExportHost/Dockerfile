FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.5

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY / /app
WORKDIR /app

ENTRYPOINT ["/app/AssociationRegistry.PowerBi.ExportHost"]
