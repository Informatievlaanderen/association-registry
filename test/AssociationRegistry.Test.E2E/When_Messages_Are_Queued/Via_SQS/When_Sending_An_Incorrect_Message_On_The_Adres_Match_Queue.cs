namespace AssociationRegistry.Test.E2E.When_Messages_Are_Queued.Via_SQS;

using Amazon.SQS.Model;
using Hosts.Configuration;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using AutoFixture;
using FluentAssertions;
using Messages;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(MessageQueueCollection))]
public class When_Sending_An_Incorrect_Message_On_The_Adres_Match_Queue
{
    private readonly FullBlownApiSetup _setup;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _autoFixture;

    public When_Sending_An_Incorrect_Message_On_The_Adres_Match_Queue(FullBlownApiSetup setup, ITestOutputHelper testOutputHelper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();

        _setup = setup;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async ValueTask Then_The_Dlq_Recieves_The_Message()
    {
        var dlqUrl = await _setup.AmazonSqs.GetQueueUrlAsync(_setup.AdminApiConfiguration.GetGrarOptions().Sqs.AddressMatchDeadLetterQueueName);

        await _setup.AmazonSqs.PurgeQueueAsync(dlqUrl.QueueUrl);

        await _setup.SqsClientWrapper.QueueMessage(_autoFixture.Create<TeAdresMatchenLocatieMessage>());

        var maxRetries = 50;
        var tries = 0;
        List<Message> messages = null;

        while (tries < maxRetries)
        {
            tries++;

            var receiveMessageResponse = await _setup.AmazonSqs.ReceiveMessageAsync(dlqUrl.QueueUrl);
            messages = receiveMessageResponse?.Messages;

            if (messages?.Any() ?? false)
            {
                break;
            }

            _testOutputHelper.WriteLine($"Attempt {tries}");
            await Task.Delay(500);
        }

        messages.Should().NotBeEmpty();
    }
}
