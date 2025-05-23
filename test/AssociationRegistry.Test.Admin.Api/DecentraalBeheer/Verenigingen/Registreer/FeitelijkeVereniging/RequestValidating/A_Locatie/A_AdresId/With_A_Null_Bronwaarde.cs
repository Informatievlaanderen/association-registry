﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.A_Locatie.
    A_AdresId;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using ValidatorTest = Framework.ValidatorTest;

public class With_A_Null_Bronwaarde : ValidatorTest
{
    [Fact]
    public void Has_validation_error__bronwaarde_mag_niet_null_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = Fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = Fixture.Create<AdresId>();
        request.Locaties[0].AdresId!.Bronwaarde = null!;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Bronwaarde)}")
              .WithErrorMessage("'Bronwaarde' is verplicht.");
    }
}
