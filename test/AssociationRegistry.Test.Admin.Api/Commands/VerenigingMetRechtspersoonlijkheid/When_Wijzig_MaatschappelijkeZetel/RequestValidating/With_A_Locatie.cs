namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.
    RequestValidating;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Locatie : ValidatorTest
{
    [Fact]
    public void Uses_TeWijzigenMaatschappelijkeZetelValidator()
    {
        var validator = new WijzigMaatschappelijkeZetelRequestValidator();
        validator.ShouldHaveChildValidator(expression: request => request.Locatie, typeof(TeWijzigenMaatschappelijkeZetelValidator));
    }
}
