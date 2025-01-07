namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.RequestValidating.Vertegenwoordiger;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger.RequestModels;
using FluentValidation.TestHelper;
using Framework;
using Xunit;

public class Is_Empty : ValidatorTest
{
    [Fact]
    public void Has_validation_error__vertegenwoordiger_is_verplicht()
    {
        var validator = new WijzigVertegenwoordigerValidator();

        var request = new WijzigVertegenwoordigerRequest
        {
            Vertegenwoordiger = new TeWijzigenVertegenwoordiger(),
        };

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(toeRequest => toeRequest.Vertegenwoordiger)
              .WithErrorMessage("'Vertegenwoordiger' moet ingevuld zijn.");
    }
}
