namespace AssociationRegistry.Test.Admin.Api.Queries.VerenigingWithoutGeotagsQuery;

using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.Test.Common.Framework;
using AutoFixture;
using Common.AutoFixture;
using Events;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class VerenigingenWithoutGeotagsQueryTests
{
    [Fact]
    public async ValueTask Given()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var documentStore = await TestDocumentStoreFactory.CreateAsync("VerenigingenWithoutGeotagsQueryTests");
        var session = documentStore.LightweightSession();

        var vzers = StartStreamFor<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>(fixture, session);

        var kbos =  StartStreamFor<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>(fixture, session);

        var fvs = StartStreamFor<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid>(fixture, session);

        var verenigingenWithGeotags = StartStreamFor<GeotagsWerdenBepaald>(fixture, session);

        await session.SaveChangesAsync();

        var sut = new VerenigingenWithoutGeotagsQuery(session, new NullLogger<VerenigingenWithoutGeotagsQuery>());

        var vCodes = await sut.ExecuteAsync(cancellationToken: CancellationToken.None);

        vCodes.Should().BeEquivalentTo(vzers.Select(x => x.VCode).Concat(kbos.Select(x => x.VCode))
                                            .Concat(fvs.Select(x => x.VCode)));

        vCodes.Should().NotContain(verenigingenWithGeotags.Select(x => x.VCode));
    }

    private static IEnumerable<TEvent> StartStreamFor<TEvent>(Fixture fixture, IDocumentSession session)
    where TEvent : IEvent
    {
        var events = fixture.CreateMany<TEvent>();

        foreach (var @event in events)
        {
            switch (@event)
            {
                case VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd vzerGeregistreerd:
                    session.Events.StartStream(vzerGeregistreerd.VCode, @event);
                    break;

                case VerenigingMetRechtspersoonlijkheidWerdGeregistreerd metRechtspersoonlijkheid:
                    session.Events.StartStream(metRechtspersoonlijkheid.VCode, @event);
                    break;

                case FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid gemigreerdeVzer:
                    session.Events.StartStream(gemigreerdeVzer.VCode, fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with{VCode = gemigreerdeVzer.VCode}, @event);
                    break;

                case GeotagsWerdenBepaald geotags:
                    session.Events.StartStream(geotags.VCode, fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with{VCode = geotags.VCode}, @event);
                    break;
            }
        }

        return events;
    }
}
