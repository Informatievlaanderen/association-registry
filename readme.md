# Association Registry

## Goal

> Authentic association registry containing 'feitelijke verenigingen' in Flanders.

Please see our [contributing guidelines](CONTRIBUTING.md) before contributing.

## Required tools
- dotnet sdk (see `global.json` for exact version)
- docker compose

## Setup
### Start docker containers
```~~~~
docker compose up
```
#### Minio
- use a webbrowser to navigate to [http://localhost:9011](http://localhost:9011)
- login into minio with the correct credentials
- create a bucket with the name 'verenigingen'
#### Postgress - ([more info](https://towardsdatascience.com/how-to-run-postgresql-and-pgadmin-using-docker-3a6a8ae918b5))
- use a webbrowser to navigate to [pgadmin](http://localhost:6050)
- login with:
  - username: admin@admin.com
  - password: root
- click "Add new server"
- configure pgadmin using following settings:
  - General
    - Name (name of the server - does not need to be the same as the DB)
  - Connection
    - Hostname/address (your local IP address - NOT "localhost" !)
    - Port: 5432
    - Username: root
    - Password: root
- You should now be able to connect to db "verenigingsregister"
## Conventions
### Json --- responses
- strings are empty or have a value (never NULL)
- collections are empty or have values (never NULL)
- objects can be NULL
