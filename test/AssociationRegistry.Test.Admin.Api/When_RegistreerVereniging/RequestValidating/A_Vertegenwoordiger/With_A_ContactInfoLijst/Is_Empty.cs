namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging.RequestValidating.A_Vertegenwoordiger.With_A_ContactInfoLijst;

using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new RegistreerVerenigingRequestValidator();
        var request = new RegistreerVerenigingRequest
        {
            Naam = "abcd",
            Initiator = "OVO000001",
            Vertegenwoordigers = new []
            {
                new RegistreerVerenigingRequest.Vertegenwoordiger
                {
                    ContactInfoLijst = Array.Empty<RegistreerVerenigingRequest.ContactInfo>(),
                },
            },
        };
        var result = validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(nameof(request.Vertegenwoordigers) + "[0]." + nameof(RegistreerVerenigingRequest.Vertegenwoordiger.ContactInfoLijst));
    }
}
