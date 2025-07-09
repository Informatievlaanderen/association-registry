namespace AssociationRegistry.Test.E2E.When_Aanvaard_Dubbel_Van_Handler_Fails;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Messages;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Wolverine;
using Wolverine.Persistence.Durability;
using Wolverine.Persistence.Durability.DeadLetterManagement;
using Xunit;
using ITestOutputHelper = Xunit.ITestOutputHelper;

[Collection(nameof(AanvaardDubbelHandlerCollection))]
public class Given_Incorrect_VCode_In_The_Message
{
    private readonly FullBlownApiSetup _setup;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Fixture _autoFixture;

    public Given_Incorrect_VCode_In_The_Message(FullBlownApiSetup setup, ITestOutputHelper testOutputHelper)
    {
        _autoFixture = new Fixture().CustomizeAdminApi();

        _setup = setup;
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    /// AanvaardDubbeleVerenigingMessage (onbestaande vCode) -> AanvaardDubbeleVerenigingMessagehandler
    /// AanvaardDubbeleVerenigingMessageHandler -> throws domain exception -> send message VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage
    /// VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage (onbestaande vCode) -> VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageHandler
    /// VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessageHandler -> throws exception -> Message comes on dlq
    /// </summary>
    [Fact]
    public async ValueTask Then_VerwerkWeigeringDubbelDoorAuthentiekeVerenigingHandler_Puts_A_Message_On_The_Dlq()
    {
        var bus = _setup.AdminApiHost.Services.GetRequiredService<IMessageBus>();
        var messageStore = _setup.AdminApiHost.Services.GetRequiredService<IMessageStore>();

        await PurgeDeadLetters(messageStore, typeof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage).FullName);

        var aanvaardDubbeleVerenigingMessage = _autoFixture.Create<AanvaardDubbeleVerenigingMessage>()
            with
            {
                VCode = _autoFixture.Create<VCode>(),
                VCodeDubbeleVereniging = _autoFixture.Create<VCode>(),
            };

        await bus.SendAsync(aanvaardDubbeleVerenigingMessage);

        var messages = await WaitForMessageInDlq<VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage>(messageStore);

        var message = messages.ShouldHaveMessageOfType<DeadLetterEnvelope>();
        message.Envelope.MessageType.Should().Be(typeof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage).FullName);
    }

    /// <summary>
    /// AanvaardDubbeleVerenigingMessage (onbestaande vCode) -> AanvaardDubbeleVerenigingMessagehandler
    /// AanvaardDubbeleVerenigingMessageHandler -> throws uncaught exception -> puts AanvaardDubbeleVerenigingMessage on DLQ
    /// </summary>
    [Fact (Skip = "This breaks other tests.")]
    public async Task Then_The_Dlq_Receives_AanvaardDubbeleVerenigingMessage()
    {
        var bus = _setup.AdminApiHost.Services.GetRequiredService<IMessageBus>();
        var messageStore = _setup.AdminApiHost.Services.GetRequiredService<IMessageStore>();
        var eventstore = _setup.AdminApiHost.Services.GetRequiredService<IEventStore>();

        await PurgeDeadLetters(messageStore, typeof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage).FullName);

        var aanvaardDubbeleVerenigingMessage = await StoreImpossibleEventThatWillCrashTheEventStore(eventstore);

        await bus.SendAsync(aanvaardDubbeleVerenigingMessage);

        var messages = await WaitForMessageInDlq<AanvaardDubbeleVerenigingMessage>(messageStore);

        var message = messages.ShouldHaveMessageOfType<DeadLetterEnvelope>();
        message.Envelope.MessageType.Should().Be(typeof(AanvaardDubbeleVerenigingMessage).FullName);

        var session = _setup.AdminApiHost.Services.GetRequiredService<IDocumentSession>();
        session.Events.ArchiveStream(aanvaardDubbeleVerenigingMessage.VCode);
    }

    private async Task<AanvaardDubbeleVerenigingMessage> StoreImpossibleEventThatWillCrashTheEventStore(IEventStore eventstore)
    {
        var aanvaardDubbeleVerenigingMessage = _autoFixture.Create<AanvaardDubbeleVerenigingMessage>()
            with
            {
                VCode = _autoFixture.Create<VCode>(),
                VCodeDubbeleVereniging = _autoFixture.Create<VCode>(),
            };

        await eventstore.Save(aanvaardDubbeleVerenigingMessage.VCode, EventStore.ExpectedVersion.NewStream, new CommandMetadata("test", new Instant(), Guid.NewGuid(), null),
                              CancellationToken.None, new VerenigingWerdGestopt(new DateOnly()));

        return aanvaardDubbeleVerenigingMessage;
    }

    private async Task<IReadOnlyList<DeadLetterEnvelope>?> WaitForMessageInDlq<TMessage>(IMessageStore messageStore)
    {
        var maxRetries = 120;
        var tries = 0;
        IReadOnlyList<DeadLetterEnvelope> messages = null;
        while (tries < maxRetries)
        {
            tries++;

            var envelopesFound = await messageStore.DeadLetters.QueryDeadLetterEnvelopesAsync(new DeadLetterEnvelopeQueryParameters());
            messages = envelopesFound.DeadLetterEnvelopes.Where(x => x.MessageType == typeof(TMessage).FullName).ToArray();

            var deadLetterEnvelopes = envelopesFound.DeadLetterEnvelopes;
            deadLetterEnvelopes.ToList().ForEach(x => _testOutputHelper.WriteLine(x.MessageType));

            if (messages.Any())
            {
                break;
            }

            _testOutputHelper.WriteLine($"Attempt {tries}");
            await Task.Delay(500);
        }

        return messages;
    }

    private static async Task PurgeDeadLetters(IMessageStore messageStore, string? messageType)
    {
        var deadLetters = await messageStore.DeadLetters.QueryDeadLetterEnvelopesAsync(new DeadLetterEnvelopeQueryParameters()
        {
            MessageType = messageType,
        });

        await messageStore.DeadLetters.DeleteDeadLetterEnvelopesAsync(deadLetters.DeadLetterEnvelopes.Select(x => x.Id).ToArray());
    }
}
