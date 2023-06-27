namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.VoegLocatieToe;
using Framework;
using Vereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Locatietype : ValidatorTest
{
    [Theory]
    [InlineData(nameof(Locatietype.Correspondentie))]
    [InlineData(nameof(Locatietype.Activiteiten))]
    public void Has_no_validation_errors(string locationType)
    {
        var validator = new VoegLocatieToeValidator();
        var request = new Fixture().CustomizeAll().Create<VoegLocatieToeRequest>();
        request.Locatie.Locatietype = locationType;

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor($"{nameof(VoegLocatieToeRequest.Locatie)}.{nameof(ToeTeVoegenLocatie.Adres.Gemeente)}");
    }
}
