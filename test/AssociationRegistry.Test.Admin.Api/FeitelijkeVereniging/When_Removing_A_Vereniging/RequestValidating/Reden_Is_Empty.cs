﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Removing_A_Vereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Verwijderen;
using AssociationRegistry.Admin.Api.Verenigingen.Verwijderen.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Reden_Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_reden()
    {
        var validator = new VerwijderVerenigingRequestValidator();
        var result = validator.TestValidate(new VerwijderVerenigingRequest { Reden = string.Empty });

        result.ShouldHaveValidationErrorFor(nameof(VerwijderVerenigingRequest.Reden))
              .WithErrorMessage("'Reden' mag niet leeg zijn.");
    }
}
