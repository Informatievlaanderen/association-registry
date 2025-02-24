namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.When_WijzigBasisGegevens.RequestValidating.Doelgroep;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging.RequestModels;
using FluentValidation.TestHelper;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class A_Doelgroep
{
    [Fact]
    public void Uses_DoelgroepValidator()
    {
        var validator = new WijzigBasisgegevensRequestValidator();

        validator.ShouldHaveChildValidator(expression: request => request.Doelgroep, typeof(DoelgroepRequestValidator));
    }
}
