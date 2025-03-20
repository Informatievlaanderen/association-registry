#!/bin/sh

docker compose -f docker-compose.yml -f docker-compose.services.yml up --build
