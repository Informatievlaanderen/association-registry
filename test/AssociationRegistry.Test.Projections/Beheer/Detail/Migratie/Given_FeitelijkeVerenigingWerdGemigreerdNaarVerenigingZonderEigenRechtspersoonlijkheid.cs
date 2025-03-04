﻿namespace AssociationRegistry.Test.Projections.Beheer.Detail.Migratie;

using Admin.Schema;
using Scenario.Migratie;
using Vereniging;


[Collection(nameof(ProjectionContext))]
public class Given_FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(BeheerDetailScenarioFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario> fixture)
    : BeheerDetailScenarioClassFixture<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheidScenario>
{
    [Fact]
    public void Metadata_Is_Updated()
        => fixture.Result
                  .Metadata.Version.Should().Be(2);

    [Fact]
    public void Document_Is_Updated()
        => fixture.Result
                  .Verenigingstype.Should().Be(new VerenigingsType()
                   {
                       Code = Verenigingstype.VZER.Code,
                       Naam = Verenigingstype.VZER.Naam,
                   });
}
