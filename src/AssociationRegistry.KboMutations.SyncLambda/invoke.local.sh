#!/bin/sh

awslocal lambda invoke output.json \
  --cli-binary-format raw-in-base64-out \
  --payload file://input.json \
  --function-name sync-lambda \
  --region us-east-1
  
cat output.json

rm output.json