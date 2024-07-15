namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Adres_Or_AdresId : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_locatie_With_Only_Adres()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new Fixture().CustomizeAdminApi().Create<WijzigLocatieRequest>();
        request.Locatie.Adres = new Adres();
        request.Locatie.AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(WijzigLocatieRequest.Locatie)}");
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_With_Only_AdresId()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new Fixture().CustomizeAdminApi().Create<WijzigLocatieRequest>();
        request.Locatie.Adres = null;
        request.Locatie.AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(WijzigLocatieRequest.Locatie)}");
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_With_Both()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new Fixture().CustomizeAdminApi().Create<WijzigLocatieRequest>();
        request.Locatie.Adres = new Adres();
        request.Locatie.AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(WijzigLocatieRequest.Locatie)}")
              .WithErrorMessage(ToeTeVoegenLocatieValidator.MustHaveAdresOrAdresIdMessage);
    }

    [Fact]
    public void Has_No_validation_error_for_locatie_0_With_Neither()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new Fixture().CustomizeAdminApi().Create<WijzigLocatieRequest>();
        request.Locatie.Adres = null;
        request.Locatie.AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(WijzigLocatieRequest.Locatie)}");
    }
}
