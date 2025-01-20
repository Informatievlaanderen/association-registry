namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Broncode : ValidatorTest
{
    [Fact]
    public void Has_validation_error__broncode_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = Fixture.Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId = Fixture.Create<AdresId>();
        request.Locatie.AdresId!.Broncode = string.Empty;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Broncode)}")
              .WithErrorMessage("'Broncode' mag niet leeg zijn.");
    }
}
