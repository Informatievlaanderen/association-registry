namespace AssociationRegistry.Grar.Clients;

public record GrarOptions
{
    public KafkaOptions Kafka { get; init; }
    public SqsOptions Sqs { get; init; }
    public WolverineOptions Wolverine { get; init; }
    public HttpClientOptions HttpClient { get; init; }

    public readonly record struct KafkaOptions(string BootstrapServer, string Username, string Password, string TopicName, string GroupId, int Offset, bool Enabled, string SlackWebhook);
    public readonly record struct SqsOptions(string GrarSyncQueueUrl, string GrarSyncQueueName, string GrarSyncDeadLetterQueueName, bool GrarSyncQueueListenerEnabled, string AddressMatchQueueName, string AddressMatchDeadLetterQueueName, bool UseLocalStack);
    public readonly record struct WolverineOptions(string TransportServiceUrl, bool AutoProvision, bool OptimizeArtifactWorkflow);
    public readonly record struct HttpClientOptions(string BaseUrl, string ApiKey, int Timeout, int[] BackoffInMs);
};
