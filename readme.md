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
- insert the file 'data.json' from the solutionfolder 'minio'

## Conventions
### Json --- responses
- strings are empty or have a value (never NULL)
- collections are empty or have values (never NULL)
- objects can be NULL
