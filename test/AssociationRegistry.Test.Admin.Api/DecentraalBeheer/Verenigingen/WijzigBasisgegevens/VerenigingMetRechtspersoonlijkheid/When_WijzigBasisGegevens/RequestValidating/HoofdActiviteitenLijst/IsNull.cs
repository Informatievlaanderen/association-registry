﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.
    HoofdActiviteitenLijst;

using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class IsNull : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_hoofdactiviteitenLijst()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var request = new WijzigBasisgegevensRequest
        {
            HoofdactiviteitenVerenigingsloket = null,
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(vereniging => vereniging.HoofdactiviteitenVerenigingsloket);
    }
}
