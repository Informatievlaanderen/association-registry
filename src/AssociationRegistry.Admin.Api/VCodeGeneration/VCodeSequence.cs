namespace AssociationRegistry.Admin.Api.VCodeGeneration;

using Marten;
using Weasel.Core;
using Weasel.Core.Migrations;
using Weasel.Postgresql;

public class VCodeSequence : FeatureSchemaBase
{
    private readonly string _schema;
    private readonly int _startFrom;

    public VCodeSequence(StoreOptions options, int startFrom) : base(nameof(VCodeSequence), options.Advanced.Migrator)
    {
        _startFrom = startFrom;
        _schema = options.DatabaseSchemaName;
    }

    protected override IEnumerable<ISchemaObject> schemaObjects()
    {
        // We return a sequence that starts from the value provided in the ctor
        yield return new Sequence(new DbObjectName(_schema, $"mt_{nameof(VCodeSequence).ToLowerInvariant()}"), _startFrom);
    }
}
