namespace AssociationRegistry.Admin.Api.Infrastructure.AWS;

using Amazon.SQS;
using ConfigurationBindings;
using Kbo;
using System.Text.Json;
using System.Threading.Tasks;

public class SqsClientWrapper
{
    private readonly IAmazonSQS _sqsClient;
    private readonly string _kboSyncQueueUrl;

    public SqsClientWrapper(IAmazonSQS sqsClient, AppSettings appSettings)
    {
        _sqsClient = sqsClient;
        _kboSyncQueueUrl = appSettings.KboSyncQueueUrl;
    }

    public async Task QueueKboNummerToSynchronise(string kboNummer)
    {
        await _sqsClient.SendMessageAsync(
            _kboSyncQueueUrl,
            JsonSerializer.Serialize(
                new TeSynchroniserenKboNummerMessage(kboNummer)));
    }
}
