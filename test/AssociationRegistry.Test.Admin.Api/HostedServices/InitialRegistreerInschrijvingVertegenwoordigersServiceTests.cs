namespace AssociationRegistry.Test.Admin.Api.HostedServices;

using AssociationRegistry.Admin.Api.HostedServices.InitialRegistreerInschrijvingVertegenwoordigers;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Grar.NutsLau;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.InschrijvingenVertegenwoordigers;
using Common.AutoFixture;
using Common.Framework;
using Common.StubsMocksFakes.Faktories;
using FluentAssertions;
using JasperFx.Core;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class InitialRegistreerInschrijvingVertegenwoordigersServiceTests
{
    private readonly Fixture _fixture;
    private readonly DocumentStore _store;

    public InitialRegistreerInschrijvingVertegenwoordigersServiceTests()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _store = TestDocumentStoreFactory.CreateAsync("InitialRegistreerInschrijvingVertegenwoordigersServiceTests").GetAwaiter()
                                         .GetResult();
    }

    [Fact]
    public async Task Given_It_Throws_An_Exception_In_The_End_Then_No_Initialisation_Record_Is_Saved()
    {
        var vCodes = _fixture.CreateMany<VCode>().ToArray();

        var query = new Mock<INietKboVerenigingenVCodesQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(vCodes);

        var martenOutbox = new Mock<IMartenOutbox>();

        martenOutbox.Setup(x => x.SendAsync(It.IsAny<CommandEnvelope<SchrijfVertegenwoordigersInMessage>>(),
                                            It.IsAny<DeliveryOptions>()))
                    .ThrowsAsync(new Exception());

        var services = new ServiceCollection();
        services.AddScoped(_ => martenOutbox.Object);
        services.AddScoped(_ => query.Object);
        var serviceProvider = services.BuildServiceProvider();

        var sut = new InitialRegistreerInschrijvingVertegenwoordigersService(
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            store: _store,
            new InitialiseerRegistreerInschrijvingOptions
                { MigratieId = nameof(Given_It_Throws_An_Exception_In_The_End_Then_No_Initialisation_Record_Is_Saved) },
            new NullLogger<InitialRegistreerInschrijvingVertegenwoordigersService>());

        try
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(7.Seconds());
            await sut.StartAsync(cancellationTokenSource.Token);
            await sut.ExecuteTask;
        }
        catch (TaskCanceledException)
        {
            // this is fine
        }

        var session = _store.LightweightSession();
        var migrationRecord = await session.Query<InitialisatieInschrijvingenDocument>().SingleOrDefaultAsync();

        migrationRecord.Should().BeNull();
    }

    [Fact]
    public async Task Given_It_Succeeds_A_Initialisation_Record_Is_Saved()
    {
        var vCodes = _fixture.CreateMany<VCode>().ToArray();

        var query = new Mock<INietKboVerenigingenVCodesQuery>();

        query.Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(vCodes);

        var services = new ServiceCollection();
        services.AddScoped(_ => Mock.Of<IMartenOutbox>());
        services.AddScoped(_ => query.Object);
        var serviceProvider = services.BuildServiceProvider();

        var sut = new InitialRegistreerInschrijvingVertegenwoordigersService(
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            store: _store,
            new InitialiseerRegistreerInschrijvingOptions { MigratieId = nameof(Given_It_Succeeds_A_Initialisation_Record_Is_Saved) },
            new NullLogger<InitialRegistreerInschrijvingVertegenwoordigersService>());

        await sut.StartAsync(CancellationToken.None);
        await sut.ExecuteTask;

        var session = _store.LightweightSession();
        var migrationRecord = await session.Query<InitialisatieInschrijvingenDocument>().SingleOrDefaultAsync();

        migrationRecord.Should().NotBeNull();
        migrationRecord.Id.Should().Be(nameof(Given_It_Succeeds_A_Initialisation_Record_Is_Saved));
    }
}
