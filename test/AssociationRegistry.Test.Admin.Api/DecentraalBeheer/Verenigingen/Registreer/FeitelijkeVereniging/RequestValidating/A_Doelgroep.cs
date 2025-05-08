namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequestValidating;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Test.Framework;
using FluentValidation.TestHelper;
using Xunit;

public class A_Doelgroep
{
    [Fact]
    public void Uses_DoelgroepValidator()
    {
        var validator = new RegistreerFeitelijkeVerenigingRequestValidator(new ClockStub(DateOnly.MaxValue));

        validator.ShouldHaveChildValidator(expression: request => request.Doelgroep, typeof(DoelgroepRequestValidator));
    }
}
