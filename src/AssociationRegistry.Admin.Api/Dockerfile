


FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS base
WORKDIR /

COPY paket.* .
COPY .paket .
COPY *.sln .
COPY packages .
COPY .config .

RUN dotnet tool restore
RUN dotnet paket restore

ARG TEST_PROJECT=AssociationRegistry.Admin.Api.Tests
ARG BUILD_PROJECT=AssociationRegistry.Admin.Api
ARG ELASTIC_HOST=your-elastic-host.example.com
ARG CI_BUILD_NUMBER
ARG GIT_HASH

COPY . .

RUN dotnet restore

ENV FeatureManagement__DisableAdminPreStartUpTasks=true

FROM base AS tests-build
RUN dotnet build --no-restore --runtime "linux-x64" test/${TEST_PROJECT}

FROM tests-build AS tests-run
WORKDIR /
RUN dotnet test test/${TEST_PROJECT} --logger html

FROM base AS src-build
RUN dotnet build -c Release --no-restore --runtime "linux-x64" --self-contained -f net8.0 src/AssociationRegistry.Admin.Api/AssociationRegistry.Admin.Api.csproj
RUN dotnet run -c Release --project src/AssociationRegistry.Admin.Api -- codegen write
RUN /bin/sh .github/build-scripts/set-solution-info.sh
#RUN openssl s_client -showcerts -servername ${ELASTIC_HOST} -connect ${ELASTIC_HOST}:9243 </dev/null 2>/dev/null | openssl x509 -outform PEM > mycert.crt && \
#    cp mycert.crt dist/AssociationRegistry.Admin.Api/linux/
RUN dotnet publish -c Release -o /app/publish --no-build --no-restore --runtime "linux-x64" --self-contained -f net8.0 src/AssociationRegistry.Admin.Api/AssociationRegistry.Admin.Api.csproj

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final

LABEL maintainer="Digitaal Vlaanderen <digitaal.vlaanderen@vlaanderen.be>"
LABEL registry="association-registry"

WORKDIR /app

COPY --from=src-build /app/publish .

RUN apt-get update && \
    apt-get install -y curl jq && \
    chmod +x ./init.sh

EXPOSE 11004/tcp
ENV ASPNETCORE_URLS=http://*:11004

ENTRYPOINT ["./init.sh"]
