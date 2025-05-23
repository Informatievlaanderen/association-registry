﻿namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.When_Wijzig_MaatschappelijkeZetel.RequestHandling;

using AssociationRegistry.Admin.Api.Infrastructure.ExceptionHandlers;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel;
using AssociationRegistry.Admin.Api.Verenigingen.Locaties.VerenigingMetRechtspersoonlijkheid.WijzigMaatschappelijkeZetel.RequestModels;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fakes;
using Xunit;

public class With_Null_Request
{
    private readonly WijzigMaatschappelijkeZetelController _controller;

    public With_Null_Request()
    {
        var messageBusMock = new MessageBusMock();

        _controller = new WijzigMaatschappelijkeZetelController(messageBusMock, new WijzigMaatschappelijkeZetelRequestValidator(),
                                                                new AppSettings());
    }

    [Fact]
    public async ValueTask Then_it_throws_a_CouldNotParseRequestException()
    {
        await Assert.ThrowsAsync<CouldNotParseRequestException>(
            async () => await _controller.Patch(
                vCode: "V0001001",
                locatieId: 1,
                request: null,
                new CommandMetadataProviderStub { Initiator = "OVO000001" },
                ifMatch: "M/\"1\""));
    }
}
