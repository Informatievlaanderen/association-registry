namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Locatie.RequestValidating;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.FeitelijkeVereniging.WijzigLocatie;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class With_A_Locatie : ValidatorTest
{
    [Fact]
    public void Uses_TeWijzigenLocatieValidator()
    {
        var validator = new WijzigLocatieRequestValidator();
        validator.ShouldHaveChildValidator(expression: request => request.Locatie, typeof(TeWijzigenLocatieValidator));
    }
}
