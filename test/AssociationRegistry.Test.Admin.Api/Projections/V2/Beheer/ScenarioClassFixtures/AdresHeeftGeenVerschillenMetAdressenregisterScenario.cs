namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.ScenarioClassFixtures;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer;
using AutoFixture;

public class AdresHeeftGeenVerschillenMetAdressenregisterScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd { get; }
    public AdresHeeftGeenVerschillenMetAdressenregister AdresHeeftGeenVerschillenMetAdressenregister { get; set; }

    public AdresHeeftGeenVerschillenMetAdressenregisterScenario(ProjectionContext context): base(context)
    {
        VerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();

        AdresHeeftGeenVerschillenMetAdressenregister = AutoFixture.Create<AdresHeeftGeenVerschillenMetAdressenregister>();
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(VerenigingWerdGeregistreerd.VCode,
                              VerenigingWerdGeregistreerd);
        await session.SaveChangesAsync();
        await using var session2 = await Context.DocumentSession();

        session2.Events.Append(VerenigingWerdGeregistreerd.VCode,
                               AdresHeeftGeenVerschillenMetAdressenregister);

        await session2.SaveChangesAsync();

        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
