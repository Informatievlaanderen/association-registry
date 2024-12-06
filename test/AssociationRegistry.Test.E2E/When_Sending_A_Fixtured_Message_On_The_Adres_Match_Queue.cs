namespace AssociationRegistry.Test.E2E;

using Acties.GrarConsumer;
using Amazon.SQS.Model;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework.ApiSetup;
using Grar.AddressMatch;
using Hosts.Configuration;
using Xunit;
using Xunit.Abstractions;

[Collection(FullBlownApiCollection.Name)]
public class When_Sending_A_Fixtured_Message_On_The_Adres_Match_Queue
{
    private readonly FullBlownApiSetup _setup;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _autoFixture;

    public When_Sending_A_Fixtured_Message_On_The_Adres_Match_Queue(FullBlownApiSetup setup, ITestOutputHelper testOutputHelper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();

        _setup = setup;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task Then_The_Dlq_Recieves_The_Message()
    {
        var dlqUrl = await _setup.AmazonSqs.GetQueueUrlAsync(_setup.AdminApiConfiguration.GetGrarOptions().Sqs.AddressMatchDeadLetterQueueName);

        await _setup.AmazonSqs.PurgeQueueAsync(dlqUrl.QueueUrl);

        await _setup.SqsClientWrapper.QueueMessage(_autoFixture.Create<TeAdresMatchenLocatieMessage>());

        var maxRetries = 5;
        var tries = 0;
        List<Message> messages = null;

        while (tries < maxRetries)
        {
            tries++;

            var receiveMessageResponse = await _setup.AmazonSqs.ReceiveMessageAsync(dlqUrl.QueueUrl);
            messages = receiveMessageResponse.Messages;

            if (messages.Any())
            {
                break;
            }

            _testOutputHelper.WriteLine($"Attempt {tries}");
            await Task.Delay(500);
        }

        messages.Should().NotBeEmpty();
    }
}
