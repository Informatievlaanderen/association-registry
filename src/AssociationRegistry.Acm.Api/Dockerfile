FROM mcr.microsoft.com/dotnet/runtime-deps:9.0.7

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 11002/tcp
ENV ASPNETCORE_URLS http://*:11002

ENTRYPOINT ["./init.sh"]
