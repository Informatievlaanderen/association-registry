namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Adres : ValidatorTest
{
    [Fact]
    public void Has_validation_error_for_locatie_Adres_When_Present_But_Invalid()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Adres = new Adres();

        var result = validator.TestValidate(request);
        result.ShouldHaveAnyValidationError();
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_Adres_When_Null()
    {
        var validator = new VoegLocatieToeValidator();
        var fixture = new Fixture().CustomizeAdminApi();
        var request = fixture.Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId = fixture.Create<AdresId>();
        request.Locatie.Adres = null;

        var result = validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
