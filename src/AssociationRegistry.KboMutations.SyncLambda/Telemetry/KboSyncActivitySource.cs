namespace AssociationRegistry.KboMutations.SyncLambda.Telemetry;

using System.Diagnostics;

// OpenTelemetry Semantic Convention constants
// Based on https://opentelemetry.io/docs/specs/semconv/
public static class SemanticConventions
{
    // FAAS (Function as a Service) - https://opentelemetry.io/docs/specs/semconv/faas/
    public const string FaasName = "faas.name";
    public const string FaasTrigger = "faas.trigger";
    public const string FaasColdstart = "faas.coldstart";

    // Cloud - https://opentelemetry.io/docs/specs/semconv/resource/cloud/
    public const string CloudProvider = "cloud.provider";
    public const string CloudPlatform = "cloud.platform";

    // Custom application tags
    // Note: High-cardinality identifiers (KBO nummer, file names) should NOT be tagged
    // to avoid exploding the number of unique time series in the telemetry backend
    public const string MessageType = "message.type";

    // Low-cardinality boolean flags are safe to tag
    public const string Overleden = "overleden";

    // Trigger type values - https://opentelemetry.io/docs/specs/semconv/faas/faas-spans/#faas-trigger
    public static class TriggerTypes
    {
        public const string Timer = "timer";
        public const string Pubsub = "pubsub";
        public const string Http = "http";
        public const string Datasource = "datasource";
        public const string Other = "other";
    }

    // Cloud provider values
    public static class CloudProviders
    {
        public const string Aws = "aws";
    }

    // Cloud platform values
    public static class CloudPlatforms
    {
        public const string AwsLambda = "aws_lambda";
    }

    // Message type values
    public static class MessageTypes
    {
        public const string SyncKbo = "SyncKbo";
        public const string SyncKsz = "SyncKsz";
    }
}

// Lambda names
public static class LambdaNames
{
    public const string KboSync = "kbo_sync";
}

public static class KboSyncActivitySource
{
    public static readonly ActivitySource Source = new("KboSync", "1.0.0");
}
