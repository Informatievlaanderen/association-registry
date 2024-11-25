namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie.RequestModels;

using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Valid_Name_Length : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigLocatieRequestValidator();
        var request = new WijzigLocatieRequest { Locatie = new TeWijzigenLocatie() };
        request.Locatie.Naam = new string('A', 128);

        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Locatie.Naam);
    }
}
