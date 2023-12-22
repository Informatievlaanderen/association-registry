#!/bin/bash

# Function to update the desired count of a service
update_service_count() {
    local cluster=$1
    local service=$2
    local count=$3
    SILENT_OUTPUT=`aws ecs update-service --cluster "$cluster" --service "$service" --desired-count "$count"`
}

# Function to wait for the desired count to be reached
wait_for_desired_count() {
    local cluster=$1
    local service=$2
    local desired_count=$3

    while :; do
        current_count=`aws ecs describe-services --cluster "$cluster" --services "$service" --query "services[0].runningCount" --output text`
        if [[ "$current_count" -eq "$desired_count" ]]; then
            break
        else
            sleep 5
        fi
    done
}

# Check for minimum required parameters: at least one service name
# if [ "$#" -lt 1 ]; then
#     echo "Usage: $0 service1 [service2 ...]"
#     exit 1
# fi

# cleanup() {
#     # Restore original desired counts
#     for service in $SERVICES; do
#         original_count=${original_counts["$service"]}
#         update_service_count "$CLUSTER_ARN" "$service" $original_count
#         echo "Restored desired count for service $serviceArn: $original_count"
#     done

#     for service in $SERVICES; do
#         original_count=${original_counts["$service"]}
#         wait_for_desired_count "$CLUSTER_ARN" "$service" $original_count
#         echo "Desired count restored for service $serviceArn: $original_count"
#     done

#     echo ""
#     echo "Done."
# }

# # Set trap to call cleanup function on exit
# trap cleanup EXIT

# # Disable CLI paging for this script
# export AWS_PAGER=""

# # First argument is the cluster, the rest are services
# CLUSTER_ARN=`aws ecs list-clusters --query "clusterArns[0]" --output text`
# echo "Found ECS cluster: $CLUSTER_ARN"

# SERVICES=`aws ecs list-services --cluster $CLUSTER_ARN --query "serviceArns[]"`
# echo "Found ECS services: $SERVICES"
# SERVICES=`aws ecs list-services --cluster $CLUSTER_ARN --query "serviceArns[]" --output text`

# declare -A original_counts

# # Get the original desired counts and downscale services
# for serviceArn in $SERVICES; do
#     original_count=`aws ecs describe-services --cluster "$CLUSTER_ARN" --services "$serviceArn" --query "services[0].desiredCount" --output text`
#     echo "Found desired count for service $serviceArn: $original_count"
#     original_counts["$serviceArn"]=$original_count
#     update_service_count "$CLUSTER_ARN" "$serviceArn" 0
# done

# # Wait for services to be downscaled
# for serviceArn in $SERVICES; do
#     wait_for_desired_count "$CLUSTER_ARN" "$serviceArn" 0
#     echo "Downscaled desired count for service $serviceArn"
# done

# # Clear DynamoDB Locks
# DYNAMODB_TABLES=`aws dynamodb list-tables --query TableNames[]`
# echo "Found DynamoDB tables: $DYNAMODB_TABLES"
# DYNAMODB_TABLES=`aws dynamodb list-tables --query TableNames[] --output text`

# for tableName in $DYNAMODB_TABLES; do
#     if [[ "$tableName" == *"-lock"* ]]; then
#         itemName=`aws dynamodb scan --table-name "$tableName" --query Items[0].resourceId.S --output text`
#         if ! [[ "$itemName" == "None" ]]; then
#             echo "Found $itemName inside $tableName which will be removed"
#             aws dynamodb delete-item --table-name $tableName --key '{"resourceId": {"S": "'$itemName'"}}'
#         fi
#     fi
# done

# # Clear Elasticsearch Indices
# # Elasticsearch endpoint
# ES_ENDPOINT="${ES_HOST}"

# # Authentication credentials from environment variables
# ES_USERNAME="${ES_USERNAME}"
# ES_PASSWORD="${ES_PASSWORD}"

# # Base64 encode the credentials
# AUTH=$(echo -n "$ES_USERNAME:$ES_PASSWORD" | base64)

# # Query Elasticsearch for indices starting with 'verenigingsregister-'
# INDICES=$(curl -s -X GET "$ES_ENDPOINT/_cat/indices/verenigingsregister-*?h=index" -H "Authorization: Basic $AUTH")

# echo "Found ElasticSearch indices: $INDICES"

# # Loop through the indices and delete them
# for index in $INDICES; do
#     echo "Deleting index: $index"
#     RESPONSE=$(curl -s -X DELETE "$ES_ENDPOINT/$index" -H "Authorization: Basic $AUTH")
#     echo "Response: $RESPONSE"
# done

# echo "Deletion process completed."

POSTGRES_USERNAME="root"
POSTGRES_PASSWORD="root"

# Reset database
psql -U $POSTGRES_USERNAME -d postgres -f db-recreate.sql -W $POSTGRES_PASSWORD
psql -U $POSTGRES_USERNAME -d verenigingsregister -f db-privilege-grants.sql -W $POSTGRES_PASSWORD
