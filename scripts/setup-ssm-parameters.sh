#!/bin/sh

REGION="us-east-1"

echo "Setting up SSM parameters in LocalStack with local development values..."

# Helper function to create SSM parameters using JSON input to avoid CLI parsing issues
create_param() {
    local name="$1"
    local value="$2"
    local type="${3:-String}"
    
    # Skip if value is empty or just whitespace
    if [ -z "$value" ] || [ "$value" = " " ]; then
        echo "Skipping empty parameter: $name"
        return 0
    fi
    
    echo "Creating parameter: $name"
    
    # Use a temporary file to avoid CLI parsing issues with special characters
    cat > /tmp/param.json << EOF
{
    "Name": "$name",
    "Value": "$value",
    "Type": "$type",
    "Overwrite": true
}
EOF
    
    awslocal ssm put-parameter --cli-input-json file:///tmp/param.json --region "$REGION" 2>/dev/null || echo "Failed to create $name"
    rm -f /tmp/param.json
}

# Magda Options - properly escaped
# Create dummy certificate parameters for local testing  
create_param "magda-cert" "dummy-cert-for-local-testing" "SecureString"
create_param "magda-cert-password" "dummy-password" "SecureString"
create_param "postgres-password" "root" "SecureString"

echo "SSM parameters setup complete!"

# Verify parameters
echo "Verifying parameters..."
awslocal ssm describe-parameters --region "$REGION" --query 'Parameters[].Name' --output table