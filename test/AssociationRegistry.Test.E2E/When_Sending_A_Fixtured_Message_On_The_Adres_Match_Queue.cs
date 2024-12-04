namespace AssociationRegistry.Test.E2E;

using Acties.GrarConsumer;
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
        var dlqUrl = await _setup.AmazonSqs.GetQueueUrlAsync(_setup.AdminApiConfiguration.GetGrarOptions().Sqs.AddressMatchQueueName);

        await _setup.AmazonSqs.PurgeQueueAsync(dlqUrl.QueueUrl);

        await _setup.SqsClientWrapper.QueueMessage(_autoFixture.Create<TeAdresMatchenLocatieMessage>());

        var tries = 0;
        var passed = false;

        while (!passed && tries < 5)
        {
            try
            {
                var receiveMessageResponse = await _setup.AmazonSqs.ReceiveMessageAsync(dlqUrl.QueueUrl);

                receiveMessageResponse.Messages.Should().NotBeEmpty();
                passed = true;
                ++tries;
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.Message);

                if (tries == 4)
                {
                    throw;
                }
                await Task.Delay(500);
            }
        }
    }
}
