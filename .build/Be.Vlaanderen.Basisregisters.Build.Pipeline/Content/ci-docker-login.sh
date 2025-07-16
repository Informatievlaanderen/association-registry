#!/usr/bin/env bash

eval $(aws ecr get-login --no-include-email --region eu-west-1 | sed 's|https://||')