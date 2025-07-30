namespace AssociationRegistry.Events;

using JasperFx.Events;
using Marten;
using Marten.Services.Json.Transformations;
using System.Data.Common;

public class TombstoneUpcaster: IEventUpcaster
{
    public object FromDbDataReader(ISerializer serializer, DbDataReader dbDataReader, int index)
        => new Tombstone();

    public async ValueTask<object> FromDbDataReaderAsync(ISerializer serializer, DbDataReader dbDataReader, int index, CancellationToken ct)
        => new Tombstone();
    public string EventTypeName => Tombstone.Name;
    public Type EventType  => typeof(Tombstone);
}
