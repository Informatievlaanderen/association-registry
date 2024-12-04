namespace AssociationRegistry.Test.E2E;

using Acties.GrarConsumer;
using Acties.GrarConsumer.HeradresseerLocaties;
using AutoFixture;
using Common.AutoFixture;
using E2E;
using E2E.Framework.ApiSetup;
using E2E.Framework.TestClasses;
using E2E.Scenarios.Requests;
using E2E.When_Verwijder_Lidmaatschap;
using Events;
using FluentAssertions;
using Hosts.Configuration;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Public.Api.Verenigingen.Search.ResponseModels;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

[Collection(FullBlownApiCollection.Name)]
public class Returns_SearchVerenigingenResponse
{
    private readonly FullBlownApiSetup _setup;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _autoFixture;

    public Returns_SearchVerenigingenResponse(FullBlownApiSetup setup, ITestOutputHelper testOutputHelper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();

        _setup = setup;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task XXX()
    {
        var dlqUrl = await _setup.AmazonSqs.GetQueueUrlAsync(_setup.AdminApiConfiguration.GetGrarOptions().Sqs.GrarSyncDeadLetterQueueName);

        await _setup.AmazonSqs.PurgeQueueAsync(dlqUrl.QueueUrl);

        await _setup.SqsClientWrapper.QueueMessage(_autoFixture.Create<OverkoepelendeGrarConsumerMessage>());


        var passed = false;

        while (!passed)
        {
            try
            {
                var receiveMessageResponse = await _setup.AmazonSqs.ReceiveMessageAsync(dlqUrl.QueueUrl);

                receiveMessageResponse.Messages.Should().NotBeEmpty();
                passed = true;
            }
            catch (Exception e)
            {
                _testOutputHelper.WriteLine(e.Message);
                await Task.Delay(500);
            }
        }
    }
}
