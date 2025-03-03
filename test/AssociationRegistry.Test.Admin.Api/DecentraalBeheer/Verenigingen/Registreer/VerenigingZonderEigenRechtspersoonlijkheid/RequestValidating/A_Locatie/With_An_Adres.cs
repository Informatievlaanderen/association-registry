namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;
using ValidatorTest = Framework.ValidatorTest;

[UnitTest]
public class With_An_Adres : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_locatie_0_Adres_When_Present_But_Invalid()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].Adres = new Adres();

        var result = validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_When_Adres_Null_And_AdresId_NotNull()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = Fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = Fixture.Create<AdresId>();

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_When_Adres_Null_And_AdresId_Null()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = Fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].Adres = null;
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Locaties)}[0]")
              .WithErrorMessage(ToeTeVoegenLocatieValidator.MustHaveAdresOrAdresIdMessage);
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_When_Adres_NotNull_And_AdresId_NotNull()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = Fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].Adres = Fixture.Create<Adres>();
        request.Locaties[0].AdresId = Fixture.Create<AdresId>();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest.Locaties)}[0]")
              .WithErrorMessage(ToeTeVoegenLocatieValidator.MustHaveAdresOrAdresIdMessage);
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_When_Adres_NotNull_And_AdresId_Null()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = Fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].Adres = Fixture.Create<Adres>();
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
