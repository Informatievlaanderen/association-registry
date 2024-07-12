namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using FluentValidation.TestHelper;
using Test.Framework;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_An_Empty_Bronwaarde : ValidatorTest
{
    [Fact]
    public void Has_validation_error__bronwaarde_mag_niet_leeg_zijn()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = Fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = Fixture.Create<AdresId>();
        request.Locaties[0].AdresId!.Bronwaarde = string.Empty;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0].{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Bronwaarde)}")
              .WithErrorMessage("'Bronwaarde' mag niet leeg zijn.");
    }
}
