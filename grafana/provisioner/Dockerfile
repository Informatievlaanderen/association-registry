FROM alpine:3.20

ARG GCX_VERSION=0.4.1

RUN apk add --no-cache curl tar && \
    curl -sSL "https://github.com/grafana/gcx/releases/download/v${GCX_VERSION}/gcx_${GCX_VERSION}_linux_amd64.tar.gz" \
      | tar -xz -C /usr/local/bin gcx && \
    apk del curl tar && \
    gcx --version

ENTRYPOINT ["/bin/sh", "-c"]

