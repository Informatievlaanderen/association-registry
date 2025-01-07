namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AutoFixture;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Null_Bronwaarde : ValidatorTest
{
    [Fact]
    public void Has_validation_error__bronwaarde_mag_niet_null_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = Fixture.Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId = Fixture.Create<AdresId>();
        request.Locatie.AdresId!.Bronwaarde = null!;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(
                   $"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Bronwaarde)}")
              .WithErrorMessage("'Bronwaarde' is verplicht.");
    }
}
