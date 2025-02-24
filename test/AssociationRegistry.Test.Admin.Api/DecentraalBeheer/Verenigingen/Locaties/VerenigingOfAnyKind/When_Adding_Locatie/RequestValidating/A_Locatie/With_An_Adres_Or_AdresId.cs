namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Adres_Or_AdresId : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error_for_locatie_With_Only_Adres()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Adres = new Adres();
        request.Locatie.AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}");
    }

    [Fact]
    public void Has_no_validation_error_for_locatie_With_Only_AdresId()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Adres = null;
        request.Locatie.AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}");
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_With_Both()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Adres = new Adres();
        request.Locatie.AdresId = new AdresId();

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}")
              .WithErrorMessage(ToeTeVoegenLocatieValidator.MustHaveAdresOrAdresIdMessage);
    }

    [Fact]
    public void Has_validation_error_for_locatie_0_With_Neither()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAdminApi().Create<VoegLocatieToeRequest>();
        request.Locatie.Adres = null;
        request.Locatie.AdresId = null;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}")
              .WithErrorMessage("'Locatie' moet of een adres of een adresId bevatten.");
    }
}
