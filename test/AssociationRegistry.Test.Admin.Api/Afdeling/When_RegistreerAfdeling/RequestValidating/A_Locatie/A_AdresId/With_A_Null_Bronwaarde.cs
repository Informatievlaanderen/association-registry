﻿namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Null_Bronwaarde : ValidatorTest
{
    [Fact]
    public void Has_validation_error__bronwaarde_mag_niet_null_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].AdresId!.Bronwaarde = null!;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                $"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Bronwaarde)}")
            .WithErrorMessage("'Bronwaarde' is verplicht.");
    }
}
