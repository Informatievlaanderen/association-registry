namespace AssociationRegistry.Test.VerenigingsRepositoryTests;

using AssociationRegistry.EventStore;
using AutoFixture;
using Common.AutoFixture;
using Events;
using EventStore;
using FluentAssertions;
using Framework;
using Moq;
using Vereniging;
using Xunit;

public class When_Checking_If_Vereniging_IsVerwijderd
{
    private readonly Fixture _fixture;

    public When_Checking_If_Vereniging_IsVerwijderd()
    {
    _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public async ValueTask Given_A_Vereniging_IsVerwijderd()
    {
        var eventStoreMock = new Mock<IEventStore>();

        var verenigingState = new VerenigingState
        {
            IsVerwijderd = true,
        };

        eventStoreMock.Setup(x => x.Load<VerenigingState>(It.IsAny<string>(), null))
                      .ReturnsAsync(verenigingState);

        var sut = new VerenigingsRepository(eventStoreMock.Object);

        var actual = await sut.IsVerwijderd(VCode.Create(_fixture.Create<VCode>()));

        actual.Should().BeTrue();
    }

    [Fact]
    public async ValueTask Given_A_Vereniging_IsNietVerwijderd()
    {
        var eventStoreMock = new Mock<IEventStore>();

        eventStoreMock.Setup(x => x.Load<VerenigingState>(It.IsAny<string>(), null))
                      .ReturnsAsync(new VerenigingState());

        var sut = new VerenigingsRepository(eventStoreMock.Object);

        var actual = await sut.IsVerwijderd(VCode.Create(_fixture.Create<VCode>()));

        actual.Should().BeFalse();
    }
}
