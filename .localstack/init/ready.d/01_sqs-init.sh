#!/bin/sh

# Set variables
REGION="us-east-1"
ACCOUNT_ID="000000000000" # Default account ID used by LocalStack
MAX_RECEIVE_COUNT=3
QUEUE_PART=""

with_dlq()
{
  QUEUE_NAME="verenigingsregister-$@"
  DLQ_NAME="$QUEUE_NAME-dlq"
  DLQ_ARN="arn:aws:sqs:$REGION:$ACCOUNT_ID:$DLQ_NAME"

  aws --endpoint-url=http://127.0.0.1:4566 sqs create-queue --region $REGION --queue-name $QUEUE_NAME
  aws --endpoint-url=http://127.0.0.1:4566 sqs create-queue --region $REGION --queue-name $DLQ_NAME
  QUEUE_URL=$(aws --endpoint-url=http://127.0.0.1:4566 sqs get-queue-url --region $REGION --queue-name $QUEUE_NAME --query QueueUrl --output text)
  aws --endpoint-url=http://127.0.0.1:4566 sqs set-queue-attributes \
    --queue-url "$QUEUE_URL" \
    --attributes "{
      \"RedrivePolicy\": \"{ \\\"maxReceiveCount\\\": \\\"3\\\", \\\"deadLetterTargetArn\\\": \\\"$DLQ_ARN\\\" }\"
    }"
}

with_dlq addressmatch
with_dlq addressmatch-e2e
with_dlq grarsync
with_dlq grarsync-e2e
