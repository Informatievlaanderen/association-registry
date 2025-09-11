FROM postgres:15.0

# Copy the optimization script
COPY optimize-template.sql /opt/optimize-template.sql

# Create entrypoint script
RUN echo '#!/bin/bash' > /entrypoint.sh && \
    echo 'set -e' >> /entrypoint.sh && \
    echo 'echo "Optimizing golden_master_template database for template usage..."' >> /entrypoint.sh && \
    echo 'PGPASSWORD="$POSTGRES_PASSWORD" psql -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -U "$POSTGRES_USER" -d postgres -f /opt/optimize-template.sql' >> /entrypoint.sh && \
    echo 'echo "Template optimization completed successfully!"' >> /entrypoint.sh && \
    chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]