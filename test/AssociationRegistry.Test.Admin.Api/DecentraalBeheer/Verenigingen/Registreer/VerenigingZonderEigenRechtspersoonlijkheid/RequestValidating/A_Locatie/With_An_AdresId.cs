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
public class With_An_AdresId : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_locatie_0_Adres_When_Present_But_Invalid()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].AdresId = new AdresId();

        var result = validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_Adres_When_Null()
    {
        var validator = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequestValidator(new ClockStub(DateOnly.MaxValue));
        var request = new Fixture().CustomizeAdminApi().Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();
        request.Locaties[0].AdresId = null;

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
