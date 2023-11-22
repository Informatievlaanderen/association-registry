﻿namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Vertegenwoordiger.With_An_Insz;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling.RequestModels;
using Fakes;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_Invalid_Characters
{
    [Theory]
    [InlineData("ABC.DEF")]
    [InlineData("25/03/71 123 57")]
    public void Has_validation_error__insz_heeft_incorect_formaat(string insz)
    {
        var validator = new RegistreerAfdelingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new RegistreerAfdelingRequest
        {
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = insz,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(request.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Insz)}")
            .WithErrorMessage("Insz heeft incorrect formaat (00.00.00-000.00 of 00000000000)");
    }
}
