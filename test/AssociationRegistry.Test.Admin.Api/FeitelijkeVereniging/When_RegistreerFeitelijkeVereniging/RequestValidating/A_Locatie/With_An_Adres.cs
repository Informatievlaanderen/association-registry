namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Adres : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_locatie_0_Adres_When_Present_But_Invalid()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = new ToeTeVoegenAdres();

        var result = validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_0_Adres_When_Null()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var request = new Fixture().CustomizeAll().Create<RegistreerFeitelijkeVerenigingRequest>();
        request.Locaties[0].Adres = null;

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
