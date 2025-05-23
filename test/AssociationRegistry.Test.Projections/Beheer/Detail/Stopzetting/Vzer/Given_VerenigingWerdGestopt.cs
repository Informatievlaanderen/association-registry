﻿namespace AssociationRegistry.Test.Projections.Beheer.Detail.Stopzetting.Vzer;

using AssociationRegistry.Admin.Schema.Constants;
using AssociationRegistry.Formats;
using AssociationRegistry.Test.Projections.Scenario.Stopzetting;

[Collection(nameof(ProjectionContext))]
public class Given_VerenigingWerdGestopt(
    BeheerDetailScenarioFixture<VerenigingWerdGestoptScenario> fixture)
    : BeheerDetailScenarioClassFixture<VerenigingWerdGestoptScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
    {
        fixture.Result.Einddatum.Should().BeEquivalentTo(fixture.Scenario.VerenigingWerdGestopt.Einddatum.FormatAsBelgianDate());
        fixture.Result.Status.Should().BeEquivalentTo(VerenigingStatus.Gestopt);
    }
}
