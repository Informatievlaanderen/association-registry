﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid;

using FluentAssertions;
using Framework.Categories;
using Framework.Fixtures;
using System.Net;
using With_Kbo_Nummer_For_Unsupported_Organisaties;
using Xunit;
using Xunit.Categories;

public class MagdaUitzonderingFoutSetup : RegistreerVereniginMetRechtspersoonlijkheidSetup
{
    private const string KboNummerForMagdaUitzonderingFoutInWiremock = "0725459040";

    public MagdaUitzonderingFoutSetup(EventsInDbScenariosFixture fixture) : base(fixture, KboNummerForMagdaUitzonderingFoutInWiremock)
    {
    }
}

[Category("AdminApi")]
[IntegrationTestToRefactor]
public class With_MagdaUitzonderingFout : With_KboNummer_For_Unsupported_Organisatie, IClassFixture<MagdaUitzonderingFoutSetup>
{
    public With_MagdaUitzonderingFout(EventsInDbScenariosFixture fixture, MagdaUitzonderingFoutSetup registreerSetup) : base(
        fixture, registreerSetup)
    {
    }

    [Fact]
    public void Then_it_returns_an_internal_server_error_response()
    {
        RegistreerVereniginMetRechtspersoonlijkheidSetup.Response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}
