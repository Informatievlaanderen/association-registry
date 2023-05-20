namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging.RequestValidating.Contactgegevens.Type;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using FluentValidation.TestHelper;
using Framework;
using Vereniging;
using Xunit;

public class Is_Valid : ValidatorTest
{
    [Fact]
    public void Has_no_validation_error()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator();
        var result = validator.TestValidate(
            new RegistreerFeitelijkeVerenigingRequest
            {
                Contactgegevens =
                    new[]
                    {
                        new ToeTeVoegenContactgegeven
                        {
                            Type = ContactgegevenType.Email,
                        },
                    },
            });

        result.ShouldNotHaveValidationErrorFor($"{nameof(RegistreerFeitelijkeVerenigingRequest.Contactgegevens)}[0].{nameof(ToeTeVoegenContactgegeven.Type)}");
    }
}
