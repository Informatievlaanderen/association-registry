FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.5

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY / /app
WORKDIR /app

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 11003/tcp
ENV ASPNETCORE_URLS http://*:11003

ENTRYPOINT ["./init.sh"]