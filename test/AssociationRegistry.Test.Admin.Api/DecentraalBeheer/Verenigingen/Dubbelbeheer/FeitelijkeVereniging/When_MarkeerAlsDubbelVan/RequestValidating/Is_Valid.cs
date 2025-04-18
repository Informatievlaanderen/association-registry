﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan;
using AssociationRegistry.Admin.Api.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.MarkeerAlsDubbelVan.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new MarkeerAlsDubbelVanValidator();

        var request = new MarkeerAlsDubbelVanRequest
        {
            IsDubbelVan = "V0001001",
        };

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
