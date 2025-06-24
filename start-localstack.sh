#!/bin/bash

echo "🚀 Starting LocalStack with KBO Lambda..."
echo "This will:"
echo "  1. Build your .NET Lambda"
echo "  2. Start LocalStack"
echo "  3. Set up all infrastructure"
echo "  4. Deploy Lambda with SQS trigger"
echo ""

# Clean up any existing setup
docker-compose down -v

# Start everything
docker-compose up --build

echo ""
echo "🎉 Setup complete!"
echo ""
echo "Test your Lambda with:"
echo "  ./scripts/test-kbo-lambda.sh"
echo ""
echo "LocalStack is running on: http://localhost:4566"