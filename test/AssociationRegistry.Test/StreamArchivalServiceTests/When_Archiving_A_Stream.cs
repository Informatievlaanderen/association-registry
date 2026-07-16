namespace AssociationRegistry.Test.StreamArchivalServiceTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using MartenDb.Store;
using Vereniging;
using Xunit;

public class When_Archiving_A_Stream
{
    private readonly Fixture _fixture;

    public When_Archiving_A_Stream()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public async ValueTask Given_An_Existing_Vereniging_Then_The_Stream_Is_Archived()
    {
        await using var store = await TestDocumentStoreFactory.CreateAsync("StreamArchivalArchived");
        await using var session = store.LightweightSession();
        var vCode = _fixture.Create<VCode>();

        session.Events.Append(
            vCode.ToString(),
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
            {
                VCode = vCode,
            }
        );

        await session.SaveChangesAsync();

        var sut = new StreamArchivalService(session);
        await sut.ArchiveStream(VCode.Create(vCode));

        await using var verifySession = store.LightweightSession();
        var state = await verifySession.Events.FetchStreamStateAsync(vCode.ToString());

        state.Should().NotBeNull();
        state!.IsArchived.Should().BeTrue();
    }

    [Fact]
    public async ValueTask Given_A_NonExisting_Stream_Then_It_Does_Not_Throw()
    {
        await using var store = await TestDocumentStoreFactory.CreateAsync("StreamArchivalNonExisting");
        await using var session = store.LightweightSession();
        var vCode = _fixture.Create<VCode>();

        var sut = new StreamArchivalService(session);
        var act = async () => await sut.ArchiveStream(VCode.Create(vCode));

        await act.Should().NotThrowAsync();
    }
}
