#!/bin/sh
REGION="${AWS_DEFAULT_REGION:-us-east-1}"

aws --endpoint-url=http://127.0.0.1:4566 s3api create-bucket --bucket verenigingsregister-uwp-data
