#!/bin/sh

# Set variables
REGION="us-east-1"
ACCOUNT_ID="000000000000"
LAMBDA_NAME="kbo-sync-lambda"
LAMBDA_ZIP="/artifacts/kbo-sync-lambda.zip"
QUEUE_NAME="verenigingsregister-kbo-sync"
S3_BUCKET="verenigingsregister-uwp-data"
S3_KEY="lambda/kbo-sync-lambda.zip"

echo "Deploying KBO Sync Lambda (with S3 support for large packages)..."

# Check if zip file exists
if [ ! -f "$LAMBDA_ZIP" ]; then
    echo "ERROR: Lambda zip file not found: $LAMBDA_ZIP"
    echo "Available files in /artifacts:"
    ls -la /artifacts/ 2>/dev/null || echo "No /artifacts directory found"
    exit 1
fi

PACKAGE_SIZE=$(stat -c%s "$LAMBDA_ZIP")
PACKAGE_SIZE_MB=$((PACKAGE_SIZE / 1024 / 1024))
echo "Found Lambda package: $LAMBDA_ZIP (${PACKAGE_SIZE_MB}MB)"

# Create IAM role
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

awslocal iam attach-role-policy \
  --role-name kbo-lambda-execution-role \
  --policy-arn arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole 2>/dev/null || true

awslocal iam attach-role-policy \
  --role-name kbo-lambda-execution-role \
  --policy-arn arn:aws:iam::aws:policy/AmazonSSMReadOnlyAccess 2>/dev/null || true

sleep 2

# Delete function if it exists
awslocal lambda delete-function --function-name $LAMBDA_NAME 2>/dev/null || true

echo "Creating Lambda function..."

if [ $PACKAGE_SIZE -gt 52428800 ]; then
    echo "Package is too large for direct upload (${PACKAGE_SIZE_MB}MB > 50MB), using S3..."
    
    # Upload to S3
    echo "Uploading Lambda package to S3..."
    awslocal s3 cp "$LAMBDA_ZIP" "s3://$S3_BUCKET/$S3_KEY"
    
    # Create Lambda from S3
    if awslocal lambda create-function \
      --function-name $LAMBDA_NAME \
      --runtime dotnet8 \
      --role arn:aws:iam::$ACCOUNT_ID:role/kbo-lambda-execution-role \
      --handler AssociationRegistry.KboMutations.SyncLambda::AssociationRegistry.KboMutations.SyncLambda.Function::FunctionHandler \
      --code S3Bucket=$S3_BUCKET,S3Key=$S3_KEY \
      --timeout 300 \
      --memory-size 1024; then
        
        echo "✅ Lambda function created from S3!"
        
        # Add environment variables separately (more reliable)
        echo "Adding environment variables..."
        awslocal lambda update-function-configuration \
          --function-name $LAMBDA_NAME \
          --environment Variables='{ASPNETCORE_ENVIRONMENT=Development,AWS_ENDPOINT_URL=http://localstack:4566,AWS_REGION=us-east-1}' 2>/dev/null || true
    else
        echo "❌ Failed to create Lambda function from S3"
        exit 1
    fi

else
    echo "Package size is acceptable (${PACKAGE_SIZE_MB}MB <= 50MB), using direct upload..."
    
    if awslocal lambda create-function \
      --function-name $LAMBDA_NAME \
      --runtime dotnet8 \
      --role arn:aws:iam::$ACCOUNT_ID:role/kbo-lambda-execution-role \
      --handler AssociationRegistry.KboMutations.SyncLambda::AssociationRegistry.KboMutations.SyncLambda.Function::FunctionHandler \
      --zip-file fileb://$LAMBDA_ZIP \
      --timeout 300 \
      --memory-size 1024; then
        
        echo "✅ Lambda function created with direct upload!"
        
        # Add environment variables separately
        echo "Adding environment variables..."
        awslocal lambda update-function-configuration \
          --function-name $LAMBDA_NAME \
          --environment Variables='{
            ASPNETCORE_ENVIRONMENT=Development,
            AWS_ENDPOINT_URL=http://localstack:4566,
            AWS_REGION=us-east-1,
            ParamNames__MagdaCertificate=ParamNames__MagdaCertificate,
            ParamNames__MagdaCertificatePassword=ParamNames__MagdaCertificatePassword,
            ParamNames__PostgresPassword=ParamNames__PostgresPassword
          }' 2>/dev/null || true
    else
        echo "❌ Failed to create Lambda function with direct upload"
        exit 1
    fi
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
awslocal lambda list-event-source-mappings --function-name $LAMBDA_NAME --query 'EventSourceMappings[].UUID' --output text 2>/dev/null | \
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