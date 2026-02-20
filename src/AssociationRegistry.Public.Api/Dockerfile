FROM mcr.microsoft.com/dotnet/runtime-deps:9.0.13

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

COPY / /app
WORKDIR /app

# Copy the certificate file into the container
COPY mycert.crt /usr/local/share/ca-certificates/mycert.crt

# Update the CA certificates store
RUN update-ca-certificates

RUN apt-get update && \
    apt-get install curl jq -y && \
    chmod +x ./init.sh

EXPOSE 11003/tcp
ENV ASPNETCORE_URLS http://*:11003

ENTRYPOINT ["./init.sh"]
