namespace AssociationRegistry.Test.Admin.Api.Projections.Detail.When_RegistreerFeitelijkeVereniging;

using AssociationRegistry.Framework;
using AutoFixture;
using Framework;
using Marten;
using NodaTime;
using NodaTime.Text;
using Xunit;

[Collection(nameof(ProjectionContext))]
public class Saves_Detail
{
    private readonly ProjectionContext _context;

    public Saves_Detail(ProjectionContext context)
    {
        _context = context;
    }

    [Fact]
    public async Task Then()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        // TODO: move to given
        await using (var session = _context.ProjectionHost.DocumentStore().LightweightSession())
        {
            session.SetHeader(MetadataHeaderNames.Initiator, "metadata.Initiator");
            session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(new Instant()));
            session.CorrelationId = Guid.NewGuid().ToString();

            session.Events.Append(scenario.VCode, scenario.CreateEvents());
            await session.SaveChangesAsync();
        }
    }
}
