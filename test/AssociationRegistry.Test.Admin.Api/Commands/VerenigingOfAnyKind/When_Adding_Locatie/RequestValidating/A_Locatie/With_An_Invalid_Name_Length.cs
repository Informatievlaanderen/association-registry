﻿namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Invalid_Name_Length : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Naam = new string('A', 129);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Locatie.Naam)
              .WithErrorMessage("Locatienaam mag niet langer dan 128 karakters zijn.");
    }
}
