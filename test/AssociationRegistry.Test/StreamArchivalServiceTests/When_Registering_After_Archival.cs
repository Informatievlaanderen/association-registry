namespace AssociationRegistry.Test.StreamArchivalServiceTests;

using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using JasperFx;
using JasperFx.Events;
using Marten;
using MartenDb.Setup;
using MartenDb.Store;
using MartenDb.VCodeGeneration;
using Vereniging;
using Weasel.Core;
using Xunit;

public class When_Registering_After_Archival
{
    private readonly Fixture _fixture;

    public When_Registering_After_Archival()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Fact]
    public async ValueTask The_Archived_VCode_Is_Not_Reused()
    {
        const string schema = "StreamArchivalReuse";

        await using var store = DocumentStore.For(options =>
        {
            options.Connection(
                "host=127.0.0.1:5432;" + "database=verenigingsregister;" + "password=root;" + "username=root"
            );

            options.Events.StreamIdentity = StreamIdentity.AsString;
            options.DatabaseSchemaName = schema;
            options.Events.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.Events.MetadataConfig.EnableAll();
            options.Serializer(TestDocumentStoreFactory.CreateCustomMartenSerializer());
            options.AddVCodeSequence();
        });

        await store.Advanced.ResetAllData();
        await store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();

        var vCodeService = new SequenceVCodeService(store);

        // 1. Get first vCode from the sequence and register a Vereniging
        var vCodeA = await vCodeService.GetNext();

        await using (var session = store.LightweightSession())
        {
            session.Events.Append(
                vCodeA.ToString(),
                _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>() with
                {
                    VCode = vCodeA,
                }
            );
            await session.SaveChangesAsync();
        }

        // 2. Archive the stream via the service
        await using (var session = store.LightweightSession())
        {
            var sut = new StreamArchivalService(session);
            await sut.ArchiveStream(vCodeA);
        }

        // 3. Ask the sequence for the next vCode -- it must NOT reuse the archived one
        var vCodeB = await vCodeService.GetNext();

        vCodeB.Should().NotBe(vCodeA);
        int.Parse(vCodeB.ToString().Substring(1)).Should().BeGreaterThan(int.Parse(vCodeA.ToString().Substring(1)));
    }
}
