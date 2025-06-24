#!/bin/sh
echo "Creating S3 bucket..."
awslocal s3api create-bucket --bucket verenigingsregister-uwp-data
echo "S3 bucket created successfully!"