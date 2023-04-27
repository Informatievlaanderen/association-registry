// <auto-generated/>
#pragma warning disable
using Marten;
using Marten.Events;
using System;

namespace Marten.Generated.EventStore
{
    // START: GeneratedEventDocumentStorage
    public class GeneratedEventDocumentStorage : Marten.Events.EventDocumentStorage
    {
        private readonly Marten.StoreOptions _options;

        public GeneratedEventDocumentStorage(Marten.StoreOptions options) : base(options)
        {
            _options = options;
        }



        public override Marten.Internal.Operations.IStorageOperation AppendEvent(Marten.Events.EventGraph events, Marten.Internal.IMartenSession session, Marten.Events.StreamAction stream, Marten.Events.IEvent e)
        {
            return new Marten.Generated.EventStore.AppendEventOperation(stream, e);
        }


        public override Marten.Internal.Operations.IStorageOperation InsertStream(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedInsertStream(stream);
        }


        public override Marten.Linq.QueryHandlers.IQueryHandler<Marten.Events.StreamState> QueryForStream(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedStreamStateQueryHandler(stream.Key);
        }


        public override Marten.Internal.Operations.IStorageOperation UpdateStreamVersion(Marten.Events.StreamAction stream)
        {
            return new Marten.Generated.EventStore.GeneratedStreamVersionOperation(stream);
        }


        public override void ApplyReaderDataToEvent(System.Data.Common.DbDataReader reader, Marten.Events.IEvent e)
        {
            if (!reader.IsDBNull(3))
            {
            var sequence = reader.GetFieldValue<long>(3);
            e.Sequence = sequence;
            }
            if (!reader.IsDBNull(4))
            {
            var id = reader.GetFieldValue<System.Guid>(4);
            e.Id = id;
            }
            var streamKey = reader.GetFieldValue<string>(5);
            e.StreamKey = streamKey;
            if (!reader.IsDBNull(6))
            {
            var version = reader.GetFieldValue<long>(6);
            e.Version = version;
            }
            if (!reader.IsDBNull(7))
            {
            var timestamp = reader.GetFieldValue<System.DateTimeOffset>(7);
            e.Timestamp = timestamp;
            }
            if (!reader.IsDBNull(8))
            {
            var tenantId = reader.GetFieldValue<string>(8);
            e.TenantId = tenantId;
            }
            if (!reader.IsDBNull(9))
            {
            var correlationId = reader.GetFieldValue<string>(9);
            e.CorrelationId = correlationId;
            }
            if (!reader.IsDBNull(10))
            {
            var causationId = reader.GetFieldValue<string>(10);
            e.CausationId = causationId;
            }
            if (!reader.IsDBNull(11))
            {
            var headers = reader.GetFieldValue<System.Collections.Generic.Dictionary<string, object>>(11);
            e.Headers = headers;
            }
            var isArchived = reader.GetFieldValue<bool>(12);
            e.IsArchived = isArchived;
        }


        public override async System.Threading.Tasks.Task ApplyReaderDataToEventAsync(System.Data.Common.DbDataReader reader, Marten.Events.IEvent e, System.Threading.CancellationToken token)
        {
            if (!(await reader.IsDBNullAsync(3, token).ConfigureAwait(false)))
            {
            var sequence = await reader.GetFieldValueAsync<long>(3, token).ConfigureAwait(false);
            e.Sequence = sequence;
            }
            if (!(await reader.IsDBNullAsync(4, token).ConfigureAwait(false)))
            {
            var id = await reader.GetFieldValueAsync<System.Guid>(4, token).ConfigureAwait(false);
            e.Id = id;
            }
            var streamKey = await reader.GetFieldValueAsync<string>(5, token).ConfigureAwait(false);
            e.StreamKey = streamKey;
            if (!(await reader.IsDBNullAsync(6, token).ConfigureAwait(false)))
            {
            var version = await reader.GetFieldValueAsync<long>(6, token).ConfigureAwait(false);
            e.Version = version;
            }
            if (!(await reader.IsDBNullAsync(7, token).ConfigureAwait(false)))
            {
            var timestamp = await reader.GetFieldValueAsync<System.DateTimeOffset>(7, token).ConfigureAwait(false);
            e.Timestamp = timestamp;
            }
            if (!(await reader.IsDBNullAsync(8, token).ConfigureAwait(false)))
            {
            var tenantId = await reader.GetFieldValueAsync<string>(8, token).ConfigureAwait(false);
            e.TenantId = tenantId;
            }
            if (!(await reader.IsDBNullAsync(9, token).ConfigureAwait(false)))
            {
            var correlationId = await reader.GetFieldValueAsync<string>(9, token).ConfigureAwait(false);
            e.CorrelationId = correlationId;
            }
            if (!(await reader.IsDBNullAsync(10, token).ConfigureAwait(false)))
            {
            var causationId = await reader.GetFieldValueAsync<string>(10, token).ConfigureAwait(false);
            e.CausationId = causationId;
            }
            if (!(await reader.IsDBNullAsync(11, token).ConfigureAwait(false)))
            {
            var headers = await reader.GetFieldValueAsync<System.Collections.Generic.Dictionary<string, object>>(11, token).ConfigureAwait(false);
            e.Headers = headers;
            }
            var isArchived = await reader.GetFieldValueAsync<bool>(12, token).ConfigureAwait(false);
            e.IsArchived = isArchived;
        }

    }

    // END: GeneratedEventDocumentStorage
    
    
    // START: AppendEventOperation
    public class AppendEventOperation : Marten.Events.Operations.AppendEventOperationBase
    {
        private readonly Marten.Events.StreamAction _stream;
        private readonly Marten.Events.IEvent _e;

        public AppendEventOperation(Marten.Events.StreamAction stream, Marten.Events.IEvent e) : base(stream, e)
        {
            _stream = stream;
            _e = e;
        }


        public const string SQL = "insert into public.mt_events (data, type, mt_dotnet_type, seq_id, id, stream_id, version, timestamp, tenant_id, correlation_id, causation_id, headers) values (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(SQL);
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            parameters[0].Value = session.Serializer.ToJson(Event.Data);
            parameters[1].Value = Event.EventTypeName != null ? (object)Event.EventTypeName : System.DBNull.Value;
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[2].Value = Event.DotNetTypeName != null ? (object)Event.DotNetTypeName : System.DBNull.Value;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[3].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[3].Value = Event.Sequence;
            parameters[4].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            parameters[4].Value = Event.Id;
            parameters[5].Value = Stream.Key != null ? (object)Stream.Key : System.DBNull.Value;
            parameters[5].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[6].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[6].Value = Event.Version;
            parameters[7].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.TimestampTz;
            parameters[7].Value = Event.Timestamp;
            parameters[8].Value = Stream.TenantId != null ? (object)Stream.TenantId : System.DBNull.Value;
            parameters[8].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[9].Value = Event.CorrelationId != null ? (object)Event.CorrelationId : System.DBNull.Value;
            parameters[9].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[10].Value = Event.CausationId != null ? (object)Event.CausationId : System.DBNull.Value;
            parameters[10].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[11].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            parameters[11].Value = session.Serializer.ToJson(Event.Headers);
        }

    }

    // END: AppendEventOperation
    
    
    // START: GeneratedInsertStream
    public class GeneratedInsertStream : Marten.Events.Operations.InsertStreamBase
    {
        private readonly Marten.Events.StreamAction _stream;

        public GeneratedInsertStream(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }


        public const string SQL = "insert into public.mt_streams (id, type, version, tenant_id) values (?, ?, ?, ?)";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(SQL);
            parameters[0].Value = Stream.Key != null ? (object)Stream.Key : System.DBNull.Value;
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[1].Value = Stream.AggregateTypeName != null ? (object)Stream.AggregateTypeName : System.DBNull.Value;
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[2].Value = Stream.Version;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[3].Value = Stream.TenantId != null ? (object)Stream.TenantId : System.DBNull.Value;
            parameters[3].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
        }

    }

    // END: GeneratedInsertStream
    
    
    // START: GeneratedStreamStateQueryHandler
    public class GeneratedStreamStateQueryHandler : Marten.Events.Querying.StreamStateQueryHandler
    {
        private readonly string _streamId;

        public GeneratedStreamStateQueryHandler(string streamId)
        {
            _streamId = streamId;
        }


        public const string SQL = "select id, version, type, timestamp, created as timestamp, is_archived from public.mt_streams where id = ?";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var npgsqlParameterArray = builder.AppendWithParameters(SQL);
            npgsqlParameterArray[0].Value = _streamId;
            npgsqlParameterArray[0].DbType = System.Data.DbType.String;
        }


        public override Marten.Events.StreamState Resolve(Marten.Internal.IMartenSession session, System.Data.Common.DbDataReader reader)
        {
            var streamState = new Marten.Events.StreamState();
            var key = reader.GetFieldValue<string>(0);
            streamState.Key = key;
            var version = reader.GetFieldValue<long>(1);
            streamState.Version = version;
            SetAggregateType(streamState, reader, session);
            var lastTimestamp = reader.GetFieldValue<System.DateTimeOffset>(3);
            streamState.LastTimestamp = lastTimestamp;
            var created = reader.GetFieldValue<System.DateTimeOffset>(4);
            streamState.Created = created;
            var isArchived = reader.GetFieldValue<bool>(5);
            streamState.IsArchived = isArchived;
            return streamState;
        }


        public override async System.Threading.Tasks.Task<Marten.Events.StreamState> ResolveAsync(Marten.Internal.IMartenSession session, System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var streamState = new Marten.Events.StreamState();
            var key = await reader.GetFieldValueAsync<string>(0, token).ConfigureAwait(false);
            streamState.Key = key;
            var version = await reader.GetFieldValueAsync<long>(1, token).ConfigureAwait(false);
            streamState.Version = version;
            await SetAggregateTypeAsync(streamState, reader, session, token).ConfigureAwait(false);
            var lastTimestamp = await reader.GetFieldValueAsync<System.DateTimeOffset>(3, token).ConfigureAwait(false);
            streamState.LastTimestamp = lastTimestamp;
            var created = await reader.GetFieldValueAsync<System.DateTimeOffset>(4, token).ConfigureAwait(false);
            streamState.Created = created;
            var isArchived = await reader.GetFieldValueAsync<bool>(5, token).ConfigureAwait(false);
            streamState.IsArchived = isArchived;
            return streamState;
        }

    }

    // END: GeneratedStreamStateQueryHandler
    
    
    // START: GeneratedStreamVersionOperation
    public class GeneratedStreamVersionOperation : Marten.Events.Operations.UpdateStreamVersion
    {
        private readonly Marten.Events.StreamAction _stream;

        public GeneratedStreamVersionOperation(Marten.Events.StreamAction stream) : base(stream)
        {
            _stream = stream;
        }


        public const string SQL = "update public.mt_streams set version = ? where id = ? and version = ?";


        public override void ConfigureCommand(Weasel.Postgresql.CommandBuilder builder, Marten.Internal.IMartenSession session)
        {
            var parameters = builder.AppendWithParameters(SQL);
            parameters[0].Value = Stream.Version;
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
            parameters[1].Value = Stream.Key != null ? (object)Stream.Key : System.DBNull.Value;
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;
            parameters[2].Value = Stream.ExpectedVersionOnServer;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Bigint;
        }

    }

    // END: GeneratedStreamVersionOperation
    
    
}

