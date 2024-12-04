#!/bin/sh

# Set variables
REGION="us-east-1"
ACCOUNT_ID="000000000000" # Default account ID used by LocalStack
MAX_RECEIVE_COUNT=3
QUEUE_PART=""

with_dlq()
{
  QUEUE_PART=$@
  QUEUE_NAME="verenigingsregister-$@"
  DLQ_NAME="$QUEUE_NAME-dlq"
  DLQ_ARN="arn:aws:sqs:$REGION:$ACCOUNT_ID:$DLQ_NAME"

  awslocal sqs create-queue --region $REGION --queue-name $QUEUE_NAME
  awslocal sqs create-queue --region $REGION --queue-name $DLQ_NAME
  awslocal sqs set-queue-attributes \
    --queue-url http://sqs.$REGION.localhost.localstack.cloud:4566/000000000000/$QUEUE_NAME \
    --attributes "{
      \"RedrivePolicy\": \"{ \\\"maxReceiveCount\\\": \\\"3\\\", \\\"deadLetterTargetArn\\\": \\\"arn:aws:sqs:$REGION:000000000000:$DLQ_NAME\\\" }\"
    }"
}

with_dlq addressmatch
with_dlq addressmatch-e2e
with_dlq grarsync
with_dlq grarsync-e2e
