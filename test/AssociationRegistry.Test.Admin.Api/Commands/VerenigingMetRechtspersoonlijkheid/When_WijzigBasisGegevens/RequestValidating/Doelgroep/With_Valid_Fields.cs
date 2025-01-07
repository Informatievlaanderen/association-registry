namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingMetRechtspersoonlijkheid.When_WijzigBasisGegevens.RequestValidating.Doelgroep;

using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Common;
using AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid.RequestModels;
using FluentValidation.TestHelper;
using Xunit;

public class With_Valid_Fields
{
    [Fact]
    public void Has_no_validation_errors()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        var result = validator.TestValidate(new WijzigBasisgegevensRequest
        {
            Doelgroep = new DoelgroepRequest(),
        });

        result.ShouldNotHaveAnyValidationErrors();
    }
}
