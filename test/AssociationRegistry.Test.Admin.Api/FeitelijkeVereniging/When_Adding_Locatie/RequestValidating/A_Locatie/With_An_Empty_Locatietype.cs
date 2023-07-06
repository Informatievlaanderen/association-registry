﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Locatietype : ValidatorTest
{
    [Fact]
    public void Has_validation_error__locatieType_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Locatietype = string.Empty;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Locatie.Locatietype)
            .WithErrorMessage("'Locatietype' mag niet leeg zijn.");
    }
}
