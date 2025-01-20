# Kbo Sync Lambda

Triggered by SQS items on the kbo sync queue. 

Performs the necessary logic to bring the association up to date with KBO.

## Main dependencies
 
- nuget package `Be.Vlaanderen.Basisregisters.AssociationRegistry`
- nuget package `Be.Vlaanderen.Basisregisters.AssociationRegistry.Magda`
- Kbo sync queue

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using
the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools)
from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.

```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.

```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests

```
    cd "AssociationRegistry.KboMutations.SyncLambda/test/AssociationRegistry.KboMutations.SyncLambda.Tests"
    dotnet test
```

Deploy function to AWS Lambda

```
    cd "AssociationRegistry.KboMutations.SyncLambda/src/AssociationRegistry.KboMutations.SyncLambda"
    dotnet lambda deploy-function
```
