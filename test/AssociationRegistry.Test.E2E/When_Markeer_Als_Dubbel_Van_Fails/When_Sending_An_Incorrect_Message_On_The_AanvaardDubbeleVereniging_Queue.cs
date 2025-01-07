namespace AssociationRegistry.Test.E2E.When_Markeer_Als_Dubbel_Van_Fails;

using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Framework.ApiSetup;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine;
using Wolverine.Persistence.Durability;
using Xunit;
using Xunit.Abstractions;

[Collection(FullBlownApiCollection.Name)]
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
    public async Task Then_The_Dlq_Receives_The_Message()
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

        var maxRetries = 120;
        var tries = 0;
        IReadOnlyList<DeadLetterEnvelope> messages = null;
        while (tries < maxRetries)
        {
            tries++;

            var envelopesFound = await messageStore.DeadLetters.QueryDeadLetterEnvelopesAsync(new DeadLetterEnvelopeQueryParameters());
            messages = envelopesFound.DeadLetterEnvelopes.Where(x => x.ExceptionType == typeof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage).FullName).ToArray();

            var deadLetterEnvelopes = envelopesFound.DeadLetterEnvelopes;
            deadLetterEnvelopes.ToList().ForEach(x => _testOutputHelper.WriteLine(x.MessageType));

            if (messages.Any())
            {
                break;
            }

            _testOutputHelper.WriteLine($"Attempt {tries}");
            await Task.Delay(500);
        }

        var message = messages.ShouldHaveMessageOfType<DeadLetterEnvelope>();
        message.Envelope.MessageType.Should().Be(typeof(VerwerkWeigeringDubbelDoorAuthentiekeVerenigingMessage).FullName);
    }

    private static async Task PurgeDeadLetters(IMessageStore messageStore, string? messageType)
    {
        var deadLetters = await messageStore.DeadLetters.QueryDeadLetterEnvelopesAsync(new DeadLetterEnvelopeQueryParameters());

        await messageStore.DeadLetters.DeleteDeadLetterEnvelopesAsync(deadLetters.DeadLetterEnvelopes.Select(x => x.Id).ToArray());
    }
}
