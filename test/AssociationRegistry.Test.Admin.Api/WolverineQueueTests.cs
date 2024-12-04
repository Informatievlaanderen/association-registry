namespace AssociationRegistry.Test.Admin.Api;

using Acties.GrarConsumer;
using Acties.GrarConsumer.HeradresseerLocaties;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework.Fixtures;
using Hosts.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class WolverineQueueTests
{
    private readonly AdminApiFixture _fixture;
    private readonly ITestOutputHelper _helper;
    private readonly Fixture _autoFixture;

    public WolverineQueueTests(EventsInDbScenariosFixture fixture, ITestOutputHelper helper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();
        _fixture = fixture;
        _helper = helper;
    }

    [Fact]
    public async Task XXX()
    {
        await _fixture.SqsClientWrapper.QueueMessage(_autoFixture.Create<OverkoepelendeGrarConsumerMessage>());

        var dlqUrl = await _fixture.AmazonSqs.GetQueueUrlAsync(_fixture.Configuration.GetGrarOptions().Sqs.GrarSyncDeadLetterQueueName);

        var passed = false;

        while (!passed)
        {
            try
            {
                var receiveMessageResponse = await _fixture.AmazonSqs.ReceiveMessageAsync(dlqUrl.QueueUrl);

                receiveMessageResponse.Messages.Should().NotBeEmpty();
                passed = true;
            }
            catch (Exception e)
            {
                _helper.WriteLine(e.Message);
                await Task.Delay(500);
            }
        }
    }
}
