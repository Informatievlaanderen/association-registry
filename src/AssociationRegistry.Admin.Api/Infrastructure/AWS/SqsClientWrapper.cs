namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Acties.HeradresseerLocaties;
using Amazon.SQS;
using ConfigurationBindings;
using Kbo;
using System.Text.Json;
using System.Threading.Tasks;

public class SqsClientWrapper
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _kboSyncQueueUrl;
    private readonly string _readdressQueueUrl;

    public SqsClientWrapper(IAmazonSQS sqsClient, AppSettings appSettings)
    {
        _sqsClient = sqsClient;
        _kboSyncQueueUrl = appSettings.KboSyncQueueUrl;
        _readdressQueueUrl = appSettings.ReaddressQueueUrl;
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
