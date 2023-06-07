﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Contactgegeven.RequestValidating.ContactGegeven.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.VoegContactGegevenToe;
using Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Not_Valid : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegevenType_mag_niet_leeg_zijn()
    {
        var validator = new VoegContactgegevenToeValidator();
        var result = validator.TestValidate(
            new VoegContactgegevenToeRequest
            {
                Contactgegeven = new ToeTeVoegenContactgegeven
                {
                    Type = "iemeel",
                },
            });

        result.ShouldHaveValidationErrorFor(nameof(VoegContactgegevenToeRequest.Contactgegeven) + "." + nameof(ToeTeVoegenContactgegeven.Type))
            .WithErrorMessage($"De waarde 'iemeel' is geen gekend contactgegeven type.")
            .Only();
    }
}
