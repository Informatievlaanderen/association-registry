﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.
    CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using Fakes;
using Framework;
using Kbo;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultNet;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_VerenigingVolgensKbo_No_Adres
{
    private readonly RegistreerVerenigingUitKboCommand _command;
    private readonly InMemorySequentialVCodeService _vCodeService;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly VerenigingVolgensKbo _verenigingVolgensKbo;

    public With_VerenigingVolgensKbo_No_Adres()
    {
        _verenigingRepositoryMock = new VerenigingRepositoryMock();
        _vCodeService = new InMemorySequentialVCodeService();

        var fixture = new Fixture().CustomizeAdminApi();

        var commandMetadata = fixture.Create<CommandMetadata>();
        _verenigingVolgensKbo = fixture.Create<VerenigingVolgensKbo>();

        _verenigingVolgensKbo.Adres = new AdresVolgensKbo
        {
            Straatnaam = null,
            Huisnummer = null,
            Busnummer = null,
            Postcode = null,
            Gemeente = null,
            Land = null,
        };

        _command = new RegistreerVerenigingUitKboCommand(KboNummer: _verenigingVolgensKbo.KboNummer);

        var commandHandler = new RegistreerVerenigingUitKboCommandHandler();

        commandHandler
           .Handle(new CommandEnvelope<RegistreerVerenigingUitKboCommand>(_command, commandMetadata),
                   _verenigingRepositoryMock,
                   _vCodeService,
                   new MagdaGeefVerenigingNumberFoundServiceMock(
                       _verenigingVolgensKbo
                   ),
                   new MagdaRegistreerInschrijvingServiceMock(Result.Success()),
                   NullLogger<RegistreerVerenigingUitKboCommandHandler>.Instance,
                   CancellationToken.None)
           .GetAwaiter()
           .GetResult();
    }

    [Fact]
    public void Then_it_saves_the_events()
    {
        _verenigingRepositoryMock.ShouldHaveSaved(
            new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
                _vCodeService.GetLast(),
                _command.KboNummer,
                _verenigingVolgensKbo.Type.Code,
                _verenigingVolgensKbo.Naam!,
                _verenigingVolgensKbo.KorteNaam!,
                _verenigingVolgensKbo.Startdatum),
            new VerenigingWerdIngeschrevenOpWijzigingenUitKbo(_command.KboNummer)
        );
    }
}
