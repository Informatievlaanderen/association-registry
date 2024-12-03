#!/bin/sh

awslocal s3api create-bucket --bucket verenigingsregister-uwp-data

# Set variables
REGION="us-east-1"
ACCOUNT_ID="000000000000" # Default account ID used by LocalStack
MAX_RECEIVE_COUNT=3

# DLQ names
DLQ_NAME="verenigingsregister-addressmatch-dlq"

# Construct DLQ ARNs
DLQ_ARN="arn:aws:sqs:$REGION:$ACCOUNT_ID:$DLQ_NAME"

# Create DLQs
awslocal sqs create-queue --region $REGION --queue-name $DLQ_NAME

awslocal sqs create-queue --region $REGION --queue-name verenigingsregister-addressmatch --attribute file:///etc/localstack/init/ready.d/01_sqs-init.redrive.json
