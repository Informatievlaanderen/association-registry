# Association Registry [![Main](https://github.com/Informatievlaanderen/association-registry/actions/workflows/main.yml/badge.svg)](https://github.com/Informatievlaanderen/association-registry/actions/workflows/main.yml)

## Goal

> Authentic association registry containing 'feitelijke verenigingen' in Flanders.

Please see our [contributing guidelines](CONTRIBUTING.md) before contributing.

## Required tools

- dotnet sdk (see `global.json` for exact version)
- docker compose

## Setup

Working with docker containers :

### Start docker containers

```~~~~
docker compose up -d
```

### Stop docker containers

```~~~~
docker compose down -v
```

## Conventions

### Json --- responses

- strings are empty or have a value (never NULL)
- collections are empty or have values (never NULL)
- objects can be NULL

