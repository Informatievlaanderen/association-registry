#!/bin/sh
set -e

REGION="${AWS_DEFAULT_REGION:-us-east-1}"
ENDPOINT_URL="${AWS_ENDPOINT_URL:-http://127.0.0.1:4566}"

if ! aws --endpoint-url="$ENDPOINT_URL" s3api head-bucket --bucket verenigingsregister-uwp-data >/dev/null 2>&1; then
  aws --endpoint-url="$ENDPOINT_URL" s3api create-bucket --region "$REGION" --bucket verenigingsregister-uwp-data
fi
