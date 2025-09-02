FROM erikbra/grate:1.5.4 as migrations

# Env Vars we need set at image runtime in order to control grate
ENV MIGRATIONS_CONNECTIONSTRING=""
ENV VERSION="0.0.0.1"

WORKDIR /app
RUN mkdir scripts
RUN mkdir migration-output
COPY "Migrations/scripts" "./scripts"

RUN addgroup -g 1001 -S nonroot && adduser -u 1001 -S nonroot -G nonroot
RUN chown -R nonroot:nonroot /app

USER nonroot

ENTRYPOINT ./grate \
    --files=./scripts \
    --databasetype postgresql \
    --connectionstring="$MIGRATIONS_CONNECTIONSTRING" \
    --version=$VERSION \
    --silent \
    --outputPath=./migration-output
