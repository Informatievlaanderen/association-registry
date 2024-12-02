namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Amazon.SQS;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using System.Text.Json;

public interface ISqsClientWrapper
{
    Task QueueReaddressMessage(TeHeradresserenLocatiesMessage message);
    Task QueueMessage<TMessage>(TMessage message);
    Task QueueKboNummerToSynchronise(string kboNummer);
}

public class SqsClientWrapper : ISqsClientWrapper
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
        await QueueMessage(message);
    }

    public async Task QueueMessage<TMessage>(TMessage message)
    {
        await _sqsClient.SendMessageAsync(
            _readdressQueueUrl,
            JsonSerializer.Serialize(message));
    }

    public async Task QueueKboNummerToSynchronise(string kboNummer)
    {
        await QueueMessage(new TeSynchroniserenKboNummerMessage(kboNummer));
    }
}
