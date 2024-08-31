namespace AssociationRegistry.ScheduledTaskHost.Infrastructure.AWS;

using Amazon.SQS;
using AssociationRegistry.Grar.HeradresseerLocaties;
using AssociationRegistry.Hosts.Configuration;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Kbo;
using System.Text.Json;

public class SqsClientWrapper
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _kboSyncQueueUrl;
    private readonly string _readdressQueueUrl;

    public SqsClientWrapper(IAmazonSQS sqsClient, AppSettings appSettings, GrarOptions grarOptions)
    {
        _sqsClient = sqsClient;
        _kboSyncQueueUrl = appSettings.KboSyncQueueUrl;
        _readdressQueueUrl = grarOptions.Sqs.GrarSyncQueueUrl;
    }

    public async Task QueueReaddressMessage(TeHeradresserenLocatiesMessage message)
    {
        await _sqsClient.SendMessageAsync(
            _readdressQueueUrl,
            JsonSerializer.Serialize(message));
    }

    public async Task QueueKboNummerToSynchronise(string kboNummer)
    {
        await _sqsClient.SendMessageAsync(
            _kboSyncQueueUrl,
            JsonSerializer.Serialize(
                new TeSynchroniserenKboNummerMessage(kboNummer)));
    }
}
