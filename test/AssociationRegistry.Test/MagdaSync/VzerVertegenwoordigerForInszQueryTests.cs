namespace AssociationRegistry.Test.MagdaSync;

using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using FluentAssertions;
using KboMutations.SyncLambda.MagdaSync.SyncKsz;
using KboMutations.SyncLambda.MagdaSync.SyncKsz.Queries;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Xunit;

public class VzerVertegenwoordigerForInszQueryTests
{
    [Fact]
    public async Task With_multiple_documents_then_distinct_by_vcode_and_vertegenwoordigerId()
    {
        // Arrange
        var fixture = new Fixture().CustomizeDomain();
        var vCode1 = fixture.Create<VCode>();
        var vCode2 = fixture.Create<VCode>();
        var insz = fixture.Create<Insz>();

        var expected = new[]
        {
            Doc(vCode1, 1, fixture, insz),
            Doc(vCode1, 2, fixture, insz),
            Doc(vCode2, 1, fixture, insz),
        };

        var input = expected
            .Append(Doc(vCode1, 1, fixture, insz)) // duplicate (same VCode + VertegenwoordigerId)
            .ToArray();

        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();

        persoonsgegevensRepoMock
            .Setup(x => x.Get(Insz.Create(insz), It.IsAny<CancellationToken>()))
            .ReturnsAsync(input);

        var filterVzerOnlyQueryMock = new Mock<IFilterVzerOnlyQuery>();

        filterVzerOnlyQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<FilterVzerOnlyQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([vCode1, vCode2]);

        var sut = new VzerVertegenwoordigerForInszQuery(
            persoonsgegevensRepoMock.Object,
            filterVzerOnlyQueryMock.Object,
            NullLogger<VzerVertegenwoordigerForInszQuery>.Instance
        );

        // Act
        var result = await sut.ExecuteAsync(insz, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected, opts => opts.WithStrictOrdering());
    }

    private VertegenwoordigerPersoonsgegevens Doc(VCode vCode, int vertegenwoordigerId, Fixture? fixture, Insz? insz) =>
        fixture.Create<VertegenwoordigerPersoonsgegevens>() with
        {
            VCode = vCode,
            Insz = insz,
            VertegenwoordigerId = vertegenwoordigerId,
        };
}
