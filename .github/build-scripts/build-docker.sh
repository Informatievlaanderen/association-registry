# Set up the docker build command
dockerRegistry="${BUILD_DOCKER_REGISTRY:-dev.local}"
containerName=$1
buildNumber="${CI_BUILD_NUMBER:-0.0.0}"

docker_cmd="docker build . --file .aws/Dockerfile --no-cache --tag $dockerRegistry/$containerName:$buildNumber --build-arg build_number=$buildNumber"

# Run the docker build command, and storeit's result
$docker_cmd 
result=$?

if [ $? -eq 0 ]; then
    echo "Docker build succeeded!"
else
    echo "Docker build failed or timed out!"
fi

exit $result
