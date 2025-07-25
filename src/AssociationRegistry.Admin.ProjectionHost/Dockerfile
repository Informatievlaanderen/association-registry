


FROM mcr.microsoft.com/dotnet/sdk:9.0.302 AS base
WORKDIR /

COPY *.sln .
COPY .config .

RUN dotnet tool restore

ARG BUILD_PROJECT=AssociationRegistry.Admin.ProjectionHost
ARG ELASTIC_HOST
ARG CI_BUILD_NUMBER
ARG GIT_HASH

COPY . .

RUN dotnet restore
RUN dotnet dotnet-script .github/build-scripts/SetSolutionInfo.csx

FROM base AS src-build

RUN dotnet run -c Release --project src/${BUILD_PROJECT} -- codegen write
RUN dotnet build -c Release --no-restore --runtime "linux-x64" --self-contained -f net9.0 src/${BUILD_PROJECT}/${BUILD_PROJECT}.csproj

RUN /bin/sh .github/build-scripts/set-solution-info.sh

RUN dotnet publish -c Release -o /app/publish --no-build --no-restore --runtime "linux-x64" --self-contained -f net9.0 src/${BUILD_PROJECT}/${BUILD_PROJECT}.csproj

RUN if [ -n "$ELASTIC_HOST" ]; then \
      echo "ELASTIC_HOST is set to $ELASTIC_HOST, retrieving certificate..."; \
      openssl s_client -showcerts -servername "$ELASTIC_HOST" -connect "$ELASTIC_HOST":9243 </dev/null 2>/dev/null | openssl x509 -outform PEM > mycert.crt && \
      cp mycert.crt /app/publish/; \
    else \
      echo "ELASTIC_HOST not set, skipping certificate retrieval"; \
    fi

FROM mcr.microsoft.com/dotnet/runtime-deps:9.0 AS final

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

WORKDIR /app

COPY --from=src-build /app/publish .

RUN apt-get update && \
    apt-get install -y curl jq && \
    chmod +x ./init.sh


EXPOSE 11006/tcp
ENV ASPNETCORE_URLS http://*:11006

ENTRYPOINT ["./init.sh"]
