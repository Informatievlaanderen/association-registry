FROM erikbra/grate:1.8.0

ARG CI_BUILD_NUMBER
ARG GIT_HASH

ENV GRATE_VERSION=${CI_BUILD_NUMBER}
ENV GRATE_TRANSACTION=Transaction
ENV GRATE_SILENT=false

COPY scripts/ /db/

# Create entrypoint script that includes the version parameter
RUN echo '#!/bin/sh' > /entrypoint.sh && \
    echo 'exec /app/grate --connstring="$APP_CONNSTRING" --version="$GRATE_VERSION" --databasetype=PostgreSQL "$@"' >> /entrypoint.sh && \
    chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
