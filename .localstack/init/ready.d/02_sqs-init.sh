#!/bin/sh

# Set variables
REGION="us-east-1"
ACCOUNT_ID="000000000000" # Default account ID used by LocalStack
MAX_RECEIVE_COUNT=3

# DLQ names
DLQ_NAME="verenigingsregister-grarsync-dlq"

# Construct DLQ ARNs
DLQ_ARN="arn:aws:sqs:$REGION:$ACCOUNT_ID:$DLQ_NAME"

# Create DLQs
awslocal sqs create-queue --region $REGION --queue-name $DLQ_NAME

awslocal sqs create-queue --region $REGION --queue-name verenigingsregister-grarsync --attribute file:///etc/localstack/init/ready.d/02_sqs-init.redrive.json
