namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_A_Locatie: ValidatorTest
{
    [Fact]
    public void Uses_TeWijzigenLocatieValidator()
    {
        var validator = new WijzigLocatieRequestValidator();
        validator.ShouldHaveChildValidator(request => request.Locatie, typeof(TeWijzigenLocatieValidator));
    }
}
