namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_An_Insz;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Valid
{
    [Theory]
    [InlineData(InszTestSet.Insz1)]
    [InlineData(InszTestSet.Insz2_WithCharacters)]
    public void Has_no_validation_errors(string insz)
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Vertegenwoordigers = new []
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger()
                {
                    Insz = insz,
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
