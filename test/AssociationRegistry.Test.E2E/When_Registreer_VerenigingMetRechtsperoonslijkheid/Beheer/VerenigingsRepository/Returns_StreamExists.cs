namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Beheer.VerenigingsRepository;

using EventStore;
using FluentAssertions;
using Formats;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using MartenDb.Store;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Vereniging;
using Xunit;

[Collection(nameof(RegistreerVerenigingMetRechtsperoonlijkheidCollection))]
public class Returns_StreamExists
{
    private readonly FullBlownApiSetup _apiSetup;
    private readonly RegistreerVerenigingMetRechtsperoonlijkheidContext _testContext;
    private readonly IEventStore _eventStore;

    public Returns_StreamExists(FullBlownApiSetup apiSetup, RegistreerVerenigingMetRechtsperoonlijkheidContext testContext)
    {
        _apiSetup = apiSetup;
        _testContext = testContext;
        _eventStore = _apiSetup.AdminApiHost.Services.GetRequiredService<IEventStore>();
    }

    [Fact]
    public async ValueTask ByVCode()
    {
        var streamExists = await _eventStore.Exists(_testContext.VCode);

        streamExists.Should().BeTrue("because the vereniging was registered and the events were saved to the event store");
    }

    [Fact]
    public async ValueTask ByKboNumber()
    {
        var streamExists = await _eventStore.Exists(KboNummer.Create(_testContext.CommandRequest.KboNummer));

        streamExists.Should().BeTrue("because the vereniging was registered and the events were saved to the event store");
    }
}
