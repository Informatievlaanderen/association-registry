namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie.RequestValidating.A_Locatie.A_AdresId;

using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using AdresId = AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common.AdresId;

public class With_A_Valid_Broncode : ValidatorTest
{
    [Theory]
    [InlineData("AR")]
    public void Has_no_validation_errors(string adresBroncode)
    {
        var validator = new VoegLocatieToeValidator();
        var request = Fixture.Create<VoegLocatieToeRequest>();
        request.Locatie.AdresId = Fixture.Create<AdresId>();
        request.Locatie.AdresId!.Broncode = Adresbron.Parse(adresBroncode);
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(
            $"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.AdresId)}.{nameof(ToeTeVoegenLocatie.AdresId.Broncode)}");
    }
}
