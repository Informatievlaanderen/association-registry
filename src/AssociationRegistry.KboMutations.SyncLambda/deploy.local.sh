#!/bin/sh
dotnet lambda package --region us-east-1
#mv ./bin/Release/net9.0/AssociationRegistry.KboMutations.MutationFileLambda.zip ../../.localstack/lambda/mutationfile.zip

#dotnet lambda package
#
# Set variables
FUNCTION_NAME="sync-lambda"
REGION="us-east-1"
ROLE_ARN="arn:aws:iam::000000000000:role/dummy-role"
HANDLER="AssociationRegistry.KboMutations.SyncLambda"
ZIP_FILE_PATH="fileb://bin/Release/net9.0/AssociationRegistry.KboMutations.SyncLambda.zip"
EVENT_SOURCE_ARN="arn:aws:sqs:us-east-1:000000000000:verenigingsregister-kbomutations-sync"
BATCH_SIZE=1

# Delete the Lambda function if it exists
if awslocal lambda get-function --function-name $FUNCTION_NAME --region $REGION; then
    echo "Deleting existing Lambda function: $FUNCTION_NAME"
    awslocal lambda delete-function --function-name $FUNCTION_NAME --region $REGION
fi

# Create the Lambda function
echo "Creating Lambda function: $FUNCTION_NAME"
awslocal lambda create-function \
    --function-name $FUNCTION_NAME \
    --runtime provided.al2 \
    --role $ROLE_ARN \
    --environment "Variables={POSTGRESQLOPTIONS__HOST=db,POSTGRESQLOPTIONS__USERNAME=root,POSTGRESQLOPTIONS__PASSWORD=root,POSTGRESQLOPTIONS__DATABASE=verenigingsregister}" \
    --handler $HANDLER \
    --zip-file $ZIP_FILE_PATH \
    --timeout 900 --memory-size 1024 \
    --region $REGION --no-paginate

# Attempt to find and delete the existing event source mapping for the Lambda function
EVENT_SOURCE_MAPPING_UUID=$(awslocal lambda list-event-source-mappings --function-name $FUNCTION_NAME --region $REGION | jq -r '.EventSourceMappings[] | select(.EventSourceArn=="'"$EVENT_SOURCE_ARN"'") | .UUID')
if [ ! -z "$EVENT_SOURCE_MAPPING_UUID" ]; then
    echo "Deleting existing event source mapping: $EVENT_SOURCE_MAPPING_UUID"
    awslocal lambda delete-event-source-mapping --uuid $EVENT_SOURCE_MAPPING_UUID --region $REGION
fi

# Create a new event source mapping for the Lambda function
echo "Creating event source mapping for Lambda function: $FUNCTION_NAME"
awslocal lambda create-event-source-mapping \
    --function-name $FUNCTION_NAME \
    --event-source-arn $EVENT_SOURCE_ARN \
    --region $REGION \
    --batch-size $BATCH_SIZE --no-paginate

echo "Setup complete."
