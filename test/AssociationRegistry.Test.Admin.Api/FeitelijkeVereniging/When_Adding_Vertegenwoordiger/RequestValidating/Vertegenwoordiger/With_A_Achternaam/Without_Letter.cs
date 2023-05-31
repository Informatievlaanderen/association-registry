﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Vertegenwoordiger.RequestValidating.Vertegenwoordiger.With_A_Achternaam;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.VoegVertegenwoordigerToe;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Without_Letter
{
    [Theory]
    [InlineData("@#(! ")]
    [InlineData("… --- …")]
    public void Has_validation_error__Achternaam_moet_minstens_een_letter_bevatten(string achternaam)
    {
        var validator = new VoegVertegenwoordigerToeValidator();
        var request = new VoegVertegenwoordigerToeRequest
        {
            Vertegenwoordiger =
                new ToeTeVoegenVertegenwoordiger
                {
                    Achternaam = achternaam,
                },
        };
        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(VoegVertegenwoordigerToeRequest.Vertegenwoordiger)}.{nameof(ToeTeVoegenVertegenwoordiger.Achternaam)}")
            .WithErrorMessage("'Achternaam' moet minstens een letter bevatten.");
    }
}
