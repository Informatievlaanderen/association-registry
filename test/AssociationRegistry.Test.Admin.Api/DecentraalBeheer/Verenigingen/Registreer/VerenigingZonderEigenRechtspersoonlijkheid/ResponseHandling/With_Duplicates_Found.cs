namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.ResponseHandling;

using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using Common.Framework;
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultNet;
using Vereniging;
using Wolverine;
using Xunit;
using Verenigingstype = Vereniging.Verenigingstype;

public class With_Duplicates_Found
{
    [Fact]
    public async ValueTask Then_Verenigingstype_FeitelijkeVereniging_Is_Returned()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var messageBus = SetupRegistreerVZERCommandHandling(fixture);
        var registreerVerenigingZonderEigenRechtspersoonlijkheidRequest = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();

        var sut = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidController(messageBus.Object,
                                                                                     Mock.Of<IValidator<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>>(),
                                                                                     new AppSettings()
                                                                                     {
                                                                                         BaseUrl = "http://localhost:5000",
                                                                                     });

        var actual = await sut.Post(registreerVerenigingZonderEigenRechtspersoonlijkheidRequest, Mock.Of<ICommandMetadataProvider>(), new WerkingsgebiedenServiceMock());

        var result = actual as ConflictObjectResult;
        var actualPotentialDuplicatesResponse = result!.Value as PotentialDuplicatesResponse;

        actualPotentialDuplicatesResponse.MogelijkeDuplicateVerenigingen.Should()
                                         .AllSatisfy(x => x.Verenigingstype.Code.Should()
                                                           .Be(Verenigingstype.VZER.Code))
                                         .And
                                         .AllSatisfy(x => x.Verenigingstype.Naam.Should()
                                                           .Be(Verenigingstype.VZER.Naam));
    }

    [Fact]
    public async ValueTask Then_Verenigingssubtype_isNietBepaald()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var messageBus = SetupRegistreerVZERCommandHandling(fixture);
        var registreerVerenigingZonderEigenRechtspersoonlijkheidRequest = fixture.Create<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>();

        var sut = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidController(messageBus.Object,
                                                                                     Mock.Of<IValidator<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>>(),
                                                                                     new AppSettings()
                                                                                     {
                                                                                         BaseUrl = "http://localhost:5000",
                                                                                     });

        var actual = await sut.Post(registreerVerenigingZonderEigenRechtspersoonlijkheidRequest, Mock.Of<ICommandMetadataProvider>(), new WerkingsgebiedenServiceMock());

        var result = actual as ConflictObjectResult;
        var actualPotentialDuplicatesResponse = result!.Value as PotentialDuplicatesResponse;

        var mogelijkeDuplicateVerenigingen = actualPotentialDuplicatesResponse.MogelijkeDuplicateVerenigingen;

        mogelijkeDuplicateVerenigingen.Should()
                                      .AllSatisfy(x => x.Verenigingssubtype.Code.Should()
                                                        .Be(VerenigingssubtypeCode.NietBepaald.Code))
                                      .And
                                      .AllSatisfy(x => x.Verenigingssubtype.Naam.Should()
                                                        .Be(VerenigingssubtypeCode.NietBepaald.Naam));
    }

    private static Mock<IMessageBus> SetupRegistreerVZERCommandHandling(Fixture fixture)
    {
        var messageBus = new Mock<IMessageBus>();
        var duplicatesFound = new PotentialDuplicatesFound(fixture.CreateMany<DuplicaatVereniging>().Select(x => x with
        {
            Verenigingstype = new DuplicaatVereniging.Types.Verenigingstype()
            {
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
            },
            Verenigingssubtype = new DuplicaatVereniging.Types.Verenigingssubtype()
            {
                Code = VerenigingssubtypeCode.NietBepaald.Code,
                Naam = VerenigingssubtypeCode.NietBepaald.Naam,
            },
        }));

        messageBus.Setup(x => x.InvokeAsync<Result>(
                             It.IsAny<CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>>(),
                             It.IsAny<CancellationToken>(),
                             It.IsAny<TimeSpan?>()))
                  .ReturnsAsync(Result.Success(duplicatesFound));

        return messageBus;
    }
}
