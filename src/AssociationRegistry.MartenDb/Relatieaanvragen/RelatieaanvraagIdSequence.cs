namespace AssociationRegistry.MartenDb.Relatieaanvragen;

using Marten;
using Weasel.Core;
using Weasel.Core.Migrations;
using Weasel.Postgresql;

/// <summary>The sequence behind the readable aanvraagId (RA0000001), like the VCodeSequence for vCodes.</summary>
public class RelatieaanvraagIdSequence : FeatureSchemaBase
{
    public const int StartFrom = 1;

    private readonly string _schema;

    public RelatieaanvraagIdSequence(StoreOptions options)
        : base(nameof(RelatieaanvraagIdSequence), options.Advanced.Migrator)
    {
        _schema = options.DatabaseSchemaName;
    }

    protected override IEnumerable<ISchemaObject> schemaObjects()
    {
        yield return new Sequence(
            new DbObjectName(_schema, $"mt_{nameof(RelatieaanvraagIdSequence).ToLowerInvariant()}"),
            StartFrom
        );
    }
}
