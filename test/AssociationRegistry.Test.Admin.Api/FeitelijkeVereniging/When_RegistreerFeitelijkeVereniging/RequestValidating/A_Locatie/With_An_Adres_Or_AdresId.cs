namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Adres_Or_AdresId : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_locatie_0_With_Only_Adres()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = new ToeTeVoegenAdres();
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]");
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_With_Only_AdresId()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]");
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_With_Both()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = new ToeTeVoegenAdres();
        request.Locaties[0].AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]");
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_With_Neither()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]")
            .WithErrorMessage("'Locatie' moet of een adres of een adresId bevatten.");
    }
}
