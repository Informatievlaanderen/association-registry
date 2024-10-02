namespace AssociationRegistry.Test.Admin.Api.Projections.V2.Beheer.LocatieZonderAdresMatch.ScenarioClassFixtures;

using Events;
using AutoFixture;

public class AdresHeeftGeenVerschillenMetAdressenregisterScenario : ProjectionScenarioFixture<ProjectionContext>
{
    public FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd { get; }
    public AdresHeeftGeenVerschillenMetAdressenregister AdresHeeftGeenVerschillenMetAdressenregister { get; }

    public AdresHeeftGeenVerschillenMetAdressenregisterScenario(ProjectionContext context) : base(context)
    {
        FeitelijkeVerenigingWerdGeregistreerd = AutoFixture.Create<FeitelijkeVerenigingWerdGeregistreerd>();
        AdresHeeftGeenVerschillenMetAdressenregister = AutoFixture.Create<AdresHeeftGeenVerschillenMetAdressenregister>()
            with
            {
                VCode = FeitelijkeVerenigingWerdGeregistreerd.VCode,
                LocatieId = FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().LocatieId,
            };
    }

    public override async Task Given()
    {
        await using var session = await Context.DocumentSession();

        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd.VCode,
                              FeitelijkeVerenigingWerdGeregistreerd);

        session.Events.Append(FeitelijkeVerenigingWerdGeregistreerd.VCode,
                              AdresHeeftGeenVerschillenMetAdressenregister);

        await session.SaveChangesAsync();
        await Context.WaitForNonStaleProjectionDataAsync();
    }
}
