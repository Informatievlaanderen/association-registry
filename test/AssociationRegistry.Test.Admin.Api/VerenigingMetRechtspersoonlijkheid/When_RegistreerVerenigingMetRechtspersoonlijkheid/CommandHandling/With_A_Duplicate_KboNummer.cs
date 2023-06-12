namespace AssociationRegistry.Test.Admin.Api.VerenigingMetRechtspersoonlijkheid.When_RegistreerVerenigingMetRechtspersoonlijkheid.CommandHandling;

using Acties.RegistreerVerenigingUitKbo;
using AssociationRegistry.Framework;
using Fakes;
using Framework;
using AutoFixture;
using DuplicateVerenigingDetection;
using EventStore;
using FluentAssertions;
using ResultNet;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class With_A_Duplicate_KboNummer : IAsyncLifetime
{
    private Result _result = null!;
    private readonly RegistreerVerenigingUitKboCommandHandler _commandHandler;
    private readonly CommandEnvelope<RegistreerVerenigingUitKboCommand> _envelope;
    private VerenigingsRepository.VCodeAndNaam _moederVCodeAndNaam;

    public With_A_Duplicate_KboNummer()
    {
        var fixture = new Fixture().CustomizeAll();

        _moederVCodeAndNaam = fixture.Create<VerenigingsRepository.VCodeAndNaam>();
        _commandHandler = new RegistreerVerenigingUitKboCommandHandler(new VerenigingRepositoryMock(moederVCodeAndNaam: _moederVCodeAndNaam), new InMemorySequentialVCodeService());
        _envelope = new CommandEnvelope<RegistreerVerenigingUitKboCommand>(fixture.Create<RegistreerVerenigingUitKboCommand>(), fixture.Create<CommandMetadata>());
    }

    public async Task InitializeAsync()
    {
        _result = await _commandHandler
            .Handle(_envelope, CancellationToken.None);
    }


    [Fact]
    public void Then_The_Result_Is_A_Failure()
    {
        _result.IsFailure().Should().BeTrue();
    }

    [Fact]
    public void Then_The_Result_Contains_The_Duplicate_VCode()
    {
        ((Result<DuplicateKboFound>)_result).Data.VCode.Should().BeEquivalentTo(_moederVCodeAndNaam.VCode);
    }


    public Task DisposeAsync()
        => Task.CompletedTask;
}
