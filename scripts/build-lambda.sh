#!/bin/bash

echo "🔨 Installing required packages..."
apt-get update && apt-get install -y zip unzip

echo "🔨 Installing Lambda tools..."
dotnet tool install -g Amazon.Lambda.Tools && dotnet tool restore
export PATH="$PATH:/root/.dotnet/tools"

echo "🔨 Building Lambda package..."
cd src/AssociationRegistry.KboMutations.SyncLambda

# Try standard Release build first
echo "Building Lambda package..."
if dotnet lambda package --configuration Release --output-package /artifacts/kbo-sync-lambda.zip; then
    dotnet clean --configuration Release
    dotnet restore
    dotnet build
    echo "✅ Lambda build successful!"
else
    echo "❌ Lambda build failed"
    exit 1
fi

# Check package size
PACKAGE_SIZE=$(stat -c%s /artifacts/kbo-sync-lambda.zip)
PACKAGE_SIZE_MB=$((PACKAGE_SIZE / 1024 / 1024))
echo "Package size: $PACKAGE_SIZE bytes (${PACKAGE_SIZE_MB}MB)"

if [ $PACKAGE_SIZE -gt 52428800 ]; then
    echo "⚠️ Package is larger than 50MB limit - will use S3 deployment"
else
    echo "✅ Package size is acceptable for direct deployment!"
fi

ls -la /artifacts/
dotnet clean
echo "Build complete!"