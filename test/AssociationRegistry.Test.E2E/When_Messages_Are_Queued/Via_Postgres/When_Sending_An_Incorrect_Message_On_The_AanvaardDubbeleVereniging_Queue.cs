namespace AssociationRegistry.Test.E2E.When_Messages_Are_Queued.Via_Postgres;

using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using AutoFixture;
using FluentAssertions;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vereniging.Exceptions;
using Wolverine;
using Wolverine.Persistence.Durability;
using Xunit;

[Collection(WellKnownCollections.GenericSharedContext)]
public class When_Sending_An_Incorrect_Message_On_The_AanvaardDubbeleVereniging_Queue
{
    private readonly FullBlownApiSetup _setup;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _autoFixture;

    public When_Sending_An_Incorrect_Message_On_The_AanvaardDubbeleVereniging_Queue(FullBlownApiSetup setup, ITestOutputHelper testOutputHelper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();

        _setup = setup;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async ValueTask Then_The_Dlq_Receives_The_Message()
    {
        var bus = _setup.AdminApiHost.Services.GetRequiredService<IMessageBus>();
        var messageStore = _setup.AdminApiHost.Services.GetRequiredService<IMessageStore>();
        await PurgeDeadLetters(messageStore, typeof(VCodeFormaatIsOngeldig).FullName);

        var aanvaardDubbeleVerenigingMessage = _autoFixture.Create<AanvaardDubbeleVerenigingMessage>();

        await bus.SendAsync(aanvaardDubbeleVerenigingMessage);

        var maxRetries = 5;
        var tries = 0;
        IReadOnlyList<DeadLetterEnvelope> messages = null;
        while (tries < maxRetries)
        {
            tries++;

            var envelopesFound = await messageStore.DeadLetters.QueryDeadLetterEnvelopesAsync(new DeadLetterEnvelopeQueryParameters());
            messages = envelopesFound.DeadLetterEnvelopes.Where(x => x.ExceptionType == typeof(VCodeFormaatIsOngeldig).FullName).ToArray();

            var deadLetterEnvelopes = envelopesFound.DeadLetterEnvelopes;
            deadLetterEnvelopes.ToList().ForEach(x => _testOutputHelper.WriteLine(x.ExceptionType));

            if (messages.Any())
            {
                break;
            }

            _testOutputHelper.WriteLine($"Attempt {tries}");
            await Task.Delay(500);
        }

        var message = messages.ShouldHaveMessageOfType<DeadLetterEnvelope>();
        message.Envelope.MessageType.Should().Be(typeof(AanvaardDubbeleVerenigingMessage).FullName);
    }

    private static async Task PurgeDeadLetters(IMessageStore messageStore, string? exceptionTypeToPurge)
    {
        var deadLetters = await messageStore.DeadLetters.QueryDeadLetterEnvelopesAsync(new DeadLetterEnvelopeQueryParameters()
        {
            ExceptionType = exceptionTypeToPurge,
        });

        await messageStore.DeadLetters.DeleteDeadLetterEnvelopesAsync(deadLetters.DeadLetterEnvelopes.Select(x => x.Id).ToArray());
    }
}
