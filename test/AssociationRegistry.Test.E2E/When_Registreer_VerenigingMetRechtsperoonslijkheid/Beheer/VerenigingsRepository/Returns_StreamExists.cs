namespace AssociationRegistry.Test.E2E.When_Registreer_VerenigingMetRechtsperoonslijkheid.Beheer.VerenigingsRepository;

using DecentraalBeheer.Vereniging;
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

    public Returns_StreamExists(
        FullBlownApiSetup apiSetup,
        RegistreerVerenigingMetRechtsperoonlijkheidContext testContext
    )
    {
        _apiSetup = apiSetup;
        _testContext = testContext;
    }

    [Fact]
    public async ValueTask ByVCode()
    {
        using var scope = _apiSetup.AdminApiHost.Services.CreateScope();
        var queryService = scope.ServiceProvider.GetRequiredService<IVerenigingStateQueryService>();

        var streamExists = await queryService.Exists(_testContext.VCode);

        streamExists
            .Should()
            .BeTrue("because the vereniging was registered and the events were saved to the event store");
    }

    [Fact]
    public async ValueTask ByKboNumber()
    {
        using var scope = _apiSetup.AdminApiHost.Services.CreateScope();
        var queryService = scope.ServiceProvider.GetRequiredService<IVerenigingStateQueryService>();

        var streamExists = await queryService.Exists(KboNummer.Create(_testContext.CommandRequest.KboNummer));

        streamExists
            .Should()
            .BeTrue("because the vereniging was registered and the events were saved to the event store");
    }
}
