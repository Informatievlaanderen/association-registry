namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.
    When_RegistreerFeitelijkeVereniging.ResponseHandling;

using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DuplicateVerenigingDetection;
using FluentAssertions;
using FluentValidation;
using Hosts.Configuration.ConfigurationBindings;
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
    public async Task Then_Verenigingstype_FeitelijkeVereniging_Is_Returned()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var messageBus = MockRegistreerVzerCommandHandling(fixture);
        var registreerFeitelijkeVerenigingRequest = fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        var sut = new RegistreerFeitelijkeVerenigingController(messageBus.Object,
                                                               Mock.Of<IValidator<RegistreerFeitelijkeVerenigingRequest>>(),
                                                               new AppSettings()
                                                               {
                                                                   BaseUrl = "http://localhost:5000",
                                                               });

        var actual = await sut.Post(registreerFeitelijkeVerenigingRequest, Mock.Of<ICommandMetadataProvider>(), new WerkingsgebiedenServiceMock());

        var result = actual as ConflictObjectResult;
        var actualPotentialDuplicatesResponse = result!.Value as PotentialDuplicatesResponse;

        var mogelijkeDuplicateVerenigingen = actualPotentialDuplicatesResponse.MogelijkeDuplicateVerenigingen;

        mogelijkeDuplicateVerenigingen.Should()
                                      .AllSatisfy(x => x.Verenigingstype.Code.Should()
                                                        .Be(Verenigingstype.FeitelijkeVereniging.Code))
                                      .And
                                      .AllSatisfy(x => x.Verenigingstype.Naam.Should()
                                                        .Be(Verenigingstype.FeitelijkeVereniging.Naam));
    }

    [Fact]
    public async Task Then_Verenigingssubtype_isNull()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        var messageBus = MockRegistreerVzerCommandHandling(fixture);
        var registreerFeitelijkeVerenigingRequest = fixture.Create<RegistreerFeitelijkeVerenigingRequest>();

        var sut = new RegistreerFeitelijkeVerenigingController(messageBus.Object,
                                                               Mock.Of<IValidator<RegistreerFeitelijkeVerenigingRequest>>(),
                                                               new AppSettings()
                                                               {
                                                                   BaseUrl = "http://localhost:5000",
                                                               });

        var actual = await sut.Post(registreerFeitelijkeVerenigingRequest, Mock.Of<ICommandMetadataProvider>(), new WerkingsgebiedenServiceMock());

        var result = actual as ConflictObjectResult;
        var actualPotentialDuplicatesResponse = result!.Value as PotentialDuplicatesResponse;

        var mogelijkeDuplicateVerenigingen = actualPotentialDuplicatesResponse.MogelijkeDuplicateVerenigingen;

                mogelijkeDuplicateVerenigingen.Should()
                                              .AllSatisfy(x => x.Verenigingssubtype.Should().BeNull());
    }

    private static Mock<IMessageBus> MockRegistreerVzerCommandHandling(Fixture fixture)
    {
        var messageBus = new Mock<IMessageBus>();
        var duplicatesFound = new PotentialDuplicatesFound(fixture.CreateMany<DuplicaatVereniging>().Select(x => x with
        {
            Verenigingstype = new DuplicaatVereniging.Types.Verenigingstype()
            {
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
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
