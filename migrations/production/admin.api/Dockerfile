FROM erikbra/grate:1.8.0

ARG CI_BUILD_NUMBER
ARG GIT_HASH

ENV GRATE_VERSION=${CI_BUILD_NUMBER}
ENV GRATE_TRANSACTION=Transaction
ENV GRATE_SILENT=false

COPY scripts/ /db/

# Create entrypoint script that includes the version parameter
RUN echo '#!/bin/sh' > /entrypoint.sh && \
    echo 'exec /app/grate --connstring="Host=$APP_CONNSTRING_HOST;Port=5432;Database=$APP_CONNSTRING_DB;Username=$APP_CONNSTRING_USERNAME;Password=$APP_CONNSTRING_PASSWORD" --version="$GRATE_VERSION" --databasetype=PostgreSQL --schema=grate_admin_api --files=/db "$@"' >> /entrypoint.sh && \
    chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
