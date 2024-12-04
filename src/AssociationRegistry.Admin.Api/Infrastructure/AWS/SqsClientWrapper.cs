namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Acties.GrarConsumer.HeradresseerLocaties;
using Amazon.SQS;
using Framework;
using Grar.GrarUpdates.Fusies.TeHeradresserenLocaties;
using Hosts.Configuration;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using System.Text.Json;
using Wolverine;

public class SqsClientWrapper : ISqsClientWrapper
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IMessageBus _messageBus;
    private readonly string _kboSyncQueueUrl;
    private readonly string _readdressQueueUrl;

    public SqsClientWrapper(IAmazonSQS sqsClient, AppSettings appSettings, GrarOptions grarOptions, IMessageBus messageBus)
    {
        _sqsClient = sqsClient;
        _messageBus = messageBus;
        _kboSyncQueueUrl = appSettings.KboSyncQueueUrl;
        _readdressQueueUrl = grarOptions.Sqs.GrarSyncQueueUrl;
    }

    public async Task QueueReaddressMessage(HeradresseerLocatiesMessage message)
    {
        await QueueMessage(message);
    }

    public async Task QueueMessage<TMessage>(TMessage message)
    {
        // await _sqsClient.SendMessageAsync(
        //     _readdressQueueUrl,
        //     JsonSerializer.Serialize(message));
        await _messageBus.SendAsync(message);
    }

    public async Task QueueKboNummerToSynchronise(string kboNummer)
    {
        await QueueMessage(new TeSynchroniserenKboNummerMessage(kboNummer));
    }
}
