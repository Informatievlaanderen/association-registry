namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.RequestValidating.Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;
using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;
using AssociationRegistry.Test.Admin.Api.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class Is_Null : ValidatorTest
{
    [Fact]
    public void Has_validation_error__vertegenwoordiger_is_verplicht()
    {
        var validator = new WijzigVertegenwoordigerValidator();

        var request = new WijzigVertegenwoordigerRequest
        {
            Vertegenwoordiger = null!,
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Vertegenwoordiger)
              .WithErrorMessage("'Vertegenwoordiger' is verplicht.");
    }
}
