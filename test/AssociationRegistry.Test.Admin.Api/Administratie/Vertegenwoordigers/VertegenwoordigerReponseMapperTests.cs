namespace AssociationRegistry.Test.Admin.Api.Administratie.Vertegenwoordigers;

using AssociationRegistry.Admin.Api.WebApi.Administratie.VertegenwoordigersPerVCode;
using AssociationRegistry.Admin.Schema.Vertegenwoordiger;
using Xunit;

public class VertegenwoordigerMappingTests
{
    [Fact]
    public void Map_StatusNull_ReturnsAllVertegenwoordigersFlattened()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(1, VertegenwoordigerKszStatus.NogNietGesynced),
                    new VertegenwoordigerData(2, VertegenwoordigerKszStatus.Bevestigd),
                }
            },
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V002",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(3, VertegenwoordigerKszStatus.Overleden),
                }
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: null);

        Assert.Equal(3, result.Length);

        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 1 && r.Status == VertegenwoordigerKszStatus.NogNietGesynced);
        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 2 && r.Status == VertegenwoordigerKszStatus.Bevestigd);
        Assert.Contains(result, r => r.VCode == "V002" && r.VertegenwoordigerId == 3 && r.Status == VertegenwoordigerKszStatus.Overleden);
    }

    [Fact]
    public void Map_StatusEmpty_ReturnsAllVertegenwoordigersFlattened()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(1, VertegenwoordigerKszStatus.NogNietGesynced),
                    new VertegenwoordigerData(2, VertegenwoordigerKszStatus.NietGekend),
                }
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: "");

        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 1 && r.Status == VertegenwoordigerKszStatus.NogNietGesynced);
        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 2 && r.Status == VertegenwoordigerKszStatus.NietGekend);
    }

    [Fact]
    public void Map_StatusProvided_FiltersToOnlyThatStatus()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(1, VertegenwoordigerKszStatus.NogNietGesynced),
                    new VertegenwoordigerData(2, VertegenwoordigerKszStatus.Bevestigd),
                    new VertegenwoordigerData(3, VertegenwoordigerKszStatus.Bevestigd),
                }
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: VertegenwoordigerKszStatus.Bevestigd);

        Assert.Equal(2, result.Length);
        Assert.All(result, r => Assert.Equal(VertegenwoordigerKszStatus.Bevestigd, r.Status));
        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 2);
        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 3);
        Assert.DoesNotContain(result, r => r.VertegenwoordigerId == 1);
    }

    [Fact]
    public void Map_StatusProvided_NoMatches_ReturnsEmpty()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(1, VertegenwoordigerKszStatus.NogNietGesynced),
                    new VertegenwoordigerData(2, VertegenwoordigerKszStatus.Bevestigd),
                }
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: VertegenwoordigerKszStatus.Overleden);

        Assert.Empty(result);
    }

    [Fact]
    public void Map_EmptyVertegenwoordigersData_ReturnsEmpty()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = Array.Empty<VertegenwoordigerData>()
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: null);

        Assert.Empty(result);
    }

    [Fact]
    public void Map_NullVertegenwoordigersData_TreatedAsEmpty()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = null!
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: null);

        Assert.Empty(result);
    }

    [Fact]
    public void Map_NullDocuments_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => VertegenwoordigerResponseMapper.Map(null!, status: null));
    }

    [Fact]
    public void Map_StatusProvided_FiltersAcrossMultipleDocuments()
    {
        var docs = new[]
        {
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V001",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(1, VertegenwoordigerKszStatus.Bevestigd),
                    new VertegenwoordigerData(2, VertegenwoordigerKszStatus.NogNietGesynced),
                }
            },
            new VertegenwoordigersPerVCodeDocument
            {
                VCode = "V002",
                VertegenwoordigersData = new[]
                {
                    new VertegenwoordigerData(3, VertegenwoordigerKszStatus.Bevestigd),
                    new VertegenwoordigerData(4, VertegenwoordigerKszStatus.NietGekend),
                }
            }
        };

        var result = VertegenwoordigerResponseMapper.Map(docs, status: VertegenwoordigerKszStatus.Bevestigd);

        Assert.Equal(2, result.Length);
        Assert.Contains(result, r => r.VCode == "V001" && r.VertegenwoordigerId == 1 && r.Status == VertegenwoordigerKszStatus.Bevestigd);
        Assert.Contains(result, r => r.VCode == "V002" && r.VertegenwoordigerId == 3 && r.Status == VertegenwoordigerKszStatus.Bevestigd);
    }
}
