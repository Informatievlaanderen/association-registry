﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.
    Contactgegevens;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__contactgegeven_is_verplicht()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new RegistreerFeitelijkeVerenigingRequest();
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(toeRequest => toeRequest.Contactgegevens);
    }
}
