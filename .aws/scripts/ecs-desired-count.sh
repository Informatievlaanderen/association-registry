#!/bin/bash

# Function to update the desired count of a service
update_service_count() {
    local cluster=$1
    local service=$2
    local count=$3
    echo "Updating $service to desired count: $count"
    aws ecs update-service --cluster "$cluster" --service "$service" --desired-count "$count"
}

# Function to wait for the desired count to be reached
wait_for_desired_count() {
    local cluster=$1
    local service=$2
    local desired_count=$3

    echo "Waiting for $service to reach desired count: $desired_count"
    while :; do
        current_count=$(aws ecs describe-services --cluster "$cluster" --services "$service" --query "services[0].runningCount" --output text)
        if [[ "$current_count" -eq "$desired_count" ]]; then
            echo "$service has reached the desired count: $desired_count"
            break
        else
            echo "Current count for $service is $current_count. Waiting..."
            sleep 10
        fi
    done
}

# Check for minimum required parameters: at least one service name
if [ "$#" -lt 2 ]; then
    echo "Usage: $0 cluster-name service1 [service2 ...]"
    exit 1
fi

# Disable CLI paging for this script
export AWS_PAGER=""

# First argument is the cluster, the rest are services
CLUSTER_ARN=$1
shift
SERVICES=("$@")

# Associative array to store original desired counts
declare -A original_counts

# Get the original desired counts and downscale services
for service in "${SERVICES[@]}"; do
    original_count=$(aws ecs describe-services --cluster "$CLUSTER_ARN" --services "$service" --query "services[0].desiredCount" --output text)
    original_counts["$service"]=$original_count
    update_service_count "$CLUSTER_ARN" "$service" 0
done

for service in "${SERVICES[@]}"; do
    original_count=$(aws ecs describe-services --cluster "$CLUSTER_ARN" --services "$service" --query "services[0].desiredCount" --output text)
    original_counts["$service"]=$original_count
    wait_for_desired_count "$CLUSTER_ARN" "$service" 0
done

# Main script operations
# Place any operations you need to perform while the services are downscaled here

# Restore services to their original desired counts
for service in "${SERVICES[@]}"; do
    original_count=${original_counts["$service"]}
    update_service_count "$CLUSTER_ARN" "$service" "$original_count"
done

for service in "${SERVICES[@]}"; do
    original_count=${original_counts["$service"]}
    wait_for_desired_count "$CLUSTER_ARN" "$service" "$original_count"
done

echo "All services have been restored to their original scaling."
