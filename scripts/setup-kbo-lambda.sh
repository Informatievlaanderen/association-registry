#!/bin/sh

# Set variables
REGION="us-east-1"
ACCOUNT_ID="000000000000"
LAMBDA_NAME="kbo-sync-lambda"
LAMBDA_ZIP="/artifacts/kbo-sync-lambda.zip"
QUEUE_NAME="verenigingsregister-kbo-sync"

echo "Deploying pre-built KBO Sync Lambda to LocalStack..."

# Check if zip file exists
if [ ! -f "$LAMBDA_ZIP" ]; then
    echo "ERROR: Lambda zip file not found: $LAMBDA_ZIP"
    echo "Available files in /artifacts:"
    ls -la /artifacts/
    exit 1
fi

echo "Found Lambda package: $LAMBDA_ZIP ($(du -h $LAMBDA_ZIP | cut -f1))"

# Create IAM role for Lambda
echo "Creating IAM role..."
awslocal iam create-role \
  --role-name kbo-lambda-execution-role \
  --assume-role-policy-document '{
    "Version": "2012-10-17",
    "Statement": [
      {
        "Effect": "Allow",
        "Principal": {"Service": "lambda.amazonaws.com"},
        "Action": "sts:AssumeRole"
      }
    ]
  }' 2>/dev/null || echo "Role may already exist"

# Attach policies
echo "Attaching policies..."
awslocal iam attach-role-policy \
  --role-name kbo-lambda-execution-role \
  --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole 2>/dev/null || true

awslocal iam attach-role-policy \
  --role-name kbo-lambda-execution-role \
  --policy-arn arn:aws:iam::aws:policy/AmazonSSMReadOnlyAccess 2>/dev/null || true

# Give IAM a moment to propagate
sleep 2

echo "Creating Lambda function..."

# Delete function if it exists
awslocal lambda delete-function --function-name $LAMBDA_NAME 2>/dev/null || true

# Create the Lambda function
if awslocal lambda create-function \
  --function-name $LAMBDA_NAME \
  --runtime dotnet8 \
  --role arn:aws:iam::$ACCOUNT_ID:role/kbo-lambda-execution-role \
  --handler AssociationRegistry.KboMutations.SyncLambda::AssociationRegistry.KboMutations.SyncLambda.Function::FunctionHandler \
  --zip-file fileb://$LAMBDA_ZIP \
  --timeout 300 \
  --memory-size 1024 \
  --environment Variables='{
    "ASPNETCORE_ENVIRONMENT":"Development",
    "AWS_ENDPOINT_URL":"http://localstack:4566",
    "AWS_REGION":"us-east-1"
  }'; then
    echo "✅ Lambda function created successfully!"
else
    echo "❌ Failed to create Lambda function"
    exit 1
fi

# Verify function exists
echo "Verifying Lambda function..."
if ! awslocal lambda get-function --function-name $LAMBDA_NAME >/dev/null 2>&1; then
    echo "❌ Lambda function verification failed"
    exit 1
fi

echo "Setting up SQS trigger..."

QUEUE_ARN="arn:aws:sqs:$REGION:$ACCOUNT_ID:$QUEUE_NAME"

# Delete existing event source mapping if it exists
awslocal lambda list-event-source-mappings --function-name $LAMBDA_NAME --query 'EventSourceMappings[].UUID' --output text | \
xargs -r -I {} awslocal lambda delete-event-source-mapping --uuid {} 2>/dev/null || true

# Create event source mapping
if awslocal lambda create-event-source-mapping \
  --function-name $LAMBDA_NAME \
  --event-source-arn $QUEUE_ARN \
  --batch-size 10 \
  --maximum-batching-window-in-seconds 5 \
  --starting-position LATEST; then
    echo "✅ Event source mapping created successfully!"
else
    echo "❌ Failed to create event source mapping"
    echo "Function name: $LAMBDA_NAME"
    echo "Queue ARN: $QUEUE_ARN"
    exit 1
fi

echo "🎉 KBO Sync Lambda deployment complete!"
echo "Lambda ARN: arn:aws:lambda:$REGION:$ACCOUNT_ID:function:$LAMBDA_NAME"
echo "Triggered by queue: $QUEUE_NAME"
echo "Queue URL: http://sqs.us-east-1.localhost.localstack.cloud:4566/000000000000/$QUEUE_NAME"