﻿namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Vertegenwoordiger.With_An_Insz;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid
{
    [Theory]
    [InlineData(InszTestSet.Insz1)]
    [InlineData(InszTestSet.Insz2_WithCharacters)]
    public void Has_no_validation_errors(string insz)
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new RegistreerFeitelijkeVerenigingRequest
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

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Vertegenwoordigers)}[0].{nameof(ToeTeVoegenVertegenwoordiger.Insz)}");
    }
}
