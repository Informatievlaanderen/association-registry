#!/bin/sh

# Set variables
REGION="${AWS_DEFAULT_REGION:-us-east-1}"
ACCOUNT_ID="${MINISTACK_ACCOUNT_ID:-123456789012}"
ENDPOINT_URL="${AWS_ENDPOINT_URL:-http://127.0.0.1:4566}"

with_dlq()
{
  QUEUE_NAME="verenigingsregister-$@"
  DLQ_NAME="$QUEUE_NAME-dlq"
  DLQ_ARN="arn:aws:sqs:$REGION:$ACCOUNT_ID:$DLQ_NAME"

  aws --endpoint-url="$ENDPOINT_URL" sqs create-queue --region "$REGION" --queue-name "$QUEUE_NAME"
  aws --endpoint-url="$ENDPOINT_URL" sqs create-queue --region "$REGION" --queue-name "$DLQ_NAME"
  QUEUE_URL=$(aws --endpoint-url="$ENDPOINT_URL" sqs get-queue-url --region "$REGION" --queue-name "$QUEUE_NAME" --query QueueUrl --output text)
  aws --endpoint-url="$ENDPOINT_URL" sqs set-queue-attributes \
    --queue-url "$QUEUE_URL" \
    --attributes "{
      \"RedrivePolicy\": \"{ \\\"maxReceiveCount\\\": \\\"3\\\", \\\"deadLetterTargetArn\\\": \\\"$DLQ_ARN\\\" }\"
    }"
}

with_dlq addressmatch
with_dlq addressmatch-e2e
with_dlq grarsync
with_dlq grarsync-e2e
