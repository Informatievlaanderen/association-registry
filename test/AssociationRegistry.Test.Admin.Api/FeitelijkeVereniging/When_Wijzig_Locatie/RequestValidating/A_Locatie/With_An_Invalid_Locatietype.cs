namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using Framework;
using Vereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;

public class With_An_Invalid_Locatietype: ValidatorTest
{
    [Theory]
    [InlineData("GeenGeldigLocatieType")]
    public void Has_ValidationError_For_Locatietype(string locatietype)
    {
        var validator = new TeWijzigenLocatieValidator();
        var locatie = new Fixture().CustomizeAll().Create<WijzigLocatieRequest.TeWijzigenLocatie>();
        locatie.Locatietype = locatietype;

        var result = validator.TestValidate(locatie);

        result.ShouldHaveValidationErrorFor(teWijzigenLocatie => teWijzigenLocatie.Locatietype)
            .WithErrorMessage($"'Locatietype' moet een geldige waarde hebben. ({Locatietype.Correspondentie}, {Locatietype.Activiteiten})");
    }
}


