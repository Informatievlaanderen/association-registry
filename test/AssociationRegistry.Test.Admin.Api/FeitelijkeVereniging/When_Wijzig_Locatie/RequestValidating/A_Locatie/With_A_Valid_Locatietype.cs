namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestValidating.A_Locatie;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using Framework;
using Vereniging;
using AutoFixture;
using FluentValidation.TestHelper;
using Xunit;

public class With_A_Valid_Locatietype: ValidatorTest
{
    [Theory]
    [InlineData(nameof(Locatietype.Correspondentie))]
    [InlineData(nameof(Locatietype.Activiteiten))]
    public void Has_NoValidationError_For_Locatietype(string locatietype)
    {
        var validator = new TeWijzigenLocatieValidator();
        var locatie = new Fixture().CustomizeAdminApi().Create<WijzigLocatieRequest.TeWijzigenLocatie>();
        locatie.Locatietype = locatietype;

        var result = validator.TestValidate(locatie);

        result.ShouldNotHaveValidationErrorFor(teWijzigenLocatie => teWijzigenLocatie.Locatietype);
    }
}


