namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using Framework;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_An_Empty_Locatietype : ValidatorTest
{
    [Fact]
    public void Has_validation_error__locatieType_mag_niet_leeg_zijn()
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAll().Create<VoegLocatieToeRequest>();
        request.Locatie.Locatietype = string.Empty;

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.Locatietype)}")
            .WithErrorMessage("'Locatietype' mag niet leeg zijn.");
    }
}
