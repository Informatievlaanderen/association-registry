namespace AssociationRegistry.Admin.Api.Verenigingen.VCodes;

using System.Collections.Generic;
using Marten;
using Weasel.Core;
using Weasel.Core.Migrations;
using Weasel.Postgresql;

public class VCodeSequence : FeatureSchemaBase
{
    private readonly int _startFrom;
    private readonly string _schema;

    public VCodeSequence(StoreOptions options, int startFrom = 1) : base(nameof(VCodeSequence), options.Advanced.Migrator)
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
