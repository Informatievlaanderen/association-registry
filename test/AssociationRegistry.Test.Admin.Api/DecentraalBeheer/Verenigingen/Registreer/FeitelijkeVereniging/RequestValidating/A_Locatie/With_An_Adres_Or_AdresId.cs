namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_An_Adres_Or_AdresId : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_locatie_0_With_Only_Adres()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = new Adres();
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]");
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_With_Only_AdresId()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]");
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_With_Both()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = new Adres();
        request.Locaties[0].AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]")
              .WithErrorMessage(ToeTeVoegenLocatieValidator.MustHaveAdresOrAdresIdMessage);
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_With_Neither()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Locaties)}[0]")
              .WithErrorMessage("'Locatie' moet of een adres of een adresId bevatten.");
    }
}
