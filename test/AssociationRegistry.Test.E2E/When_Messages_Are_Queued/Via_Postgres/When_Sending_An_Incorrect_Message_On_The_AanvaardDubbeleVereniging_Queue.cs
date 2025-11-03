namespace AssociationRegistry.Test.E2E.When_Messages_Are_Queued.Via_Postgres;

using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.Persistence.Durability;
using Wolverine.Persistence.Durability.DeadLetterManagement;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(MessageQueueCollection))]
public class When_Sending_An_Incorrect_Message_On_The_AanvaardDubbeleVereniging_Queue
{
    private readonly FullBlownApiSetup _setup;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _autoFixture;

    public When_Sending_An_Incorrect_Message_On_The_AanvaardDubbeleVereniging_Queue(
        FullBlownApiSetup setup,
        ITestOutputHelper testOutputHelper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();

        _setup = setup;
        _testOutputHelper = testOutputHelper;
    }

    [Fact(Skip = "this test fails too much, todo fix")]
    public async ValueTask Then_The_Dlq_Receives_The_Message()
    {
        using var scope = _setup.AdminApiHost.Services.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMessageBus>();
        var messageStore = scope.ServiceProvider.GetRequiredService<IMessageStore>();
        await PurgeDeadLetters(messageStore, typeof(VCodeFormaatIsOngeldig).FullName);

        var aanvaardDubbeleVerenigingMessage = _autoFixture.Create<AanvaardDubbeleVerenigingMessage>();
        _testOutputHelper.WriteLine(string.Join(",", bus.PreviewSubscriptions(aanvaardDubbeleVerenigingMessage)));

        await bus.SendAsync(aanvaardDubbeleVerenigingMessage);

        var maxRetries = 100;
        var tries = 0;
        IReadOnlyList<DeadLetterEnvelope> messages = null;

        while (tries < maxRetries)
        {
            tries++;

            var envelopesFound = await messageStore.DeadLetters.QueryAsync(new DeadLetterEnvelopeQuery
            {
                ExceptionType = typeof(VCodeFormaatIsOngeldig).FullName
            }, CancellationToken.None);

            var all = envelopesFound.Envelopes;
            foreach (var x in all) _testOutputHelper.WriteLine(x.ExceptionType);

            messages = all.Where(x => x.ExceptionType == typeof(VCodeFormaatIsOngeldig).FullName).ToArray();

            if (messages.Any()) break;

            _testOutputHelper.WriteLine($"Attempt {tries}");
            await Task.Delay(500);
        }

        var message = messages.ShouldHaveMessageOfType<DeadLetterEnvelope>();
        message.Envelope.MessageType.Should().Be(typeof(AanvaardDubbeleVerenigingMessage).FullName);
    }

    private static async Task PurgeDeadLetters(IMessageStore messageStore, string? exceptionTypeToPurge)
    {
        var results = await messageStore.DeadLetters.QueryAsync(new DeadLetterEnvelopeQuery
        {
            ExceptionType = exceptionTypeToPurge
        }, CancellationToken.None);

        var ids = results.Envelopes.Select(x => x.Id).ToArray();

        foreach (var id in ids)
            await messageStore.DeadLetters.DeadLetterEnvelopeByIdAsync(id);
    }
}
