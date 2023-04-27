// <auto-generated/>
#pragma warning disable
using AssociationRegistry.Vereniging;
using Marten.Internal;
using Marten.Internal.Storage;
using Marten.Schema;
using Marten.Schema.Arguments;
using Npgsql;
using System;
using System.Collections.Generic;
using Weasel.Core;
using Weasel.Postgresql;

namespace Marten.Generated.DocumentStorage
{
    // START: UpsertVerenigingOperation537441598
    public class UpsertVerenigingOperation537441598 : Marten.Internal.Operations.StorageOperation<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly AssociationRegistry.Vereniging.Vereniging _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public UpsertVerenigingOperation537441598(AssociationRegistry.Vereniging.Vereniging document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
        {
            _document = document;
            _id = id;
            _versions = versions;
            _mapping = mapping;
        }


        public const string COMMAND_TEXT = "select public.mt_upsert_vereniging(?, ?, ?, ?)";


        public override void Postprocess(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions)
        {
            storeVersion();
        }


        public override System.Threading.Tasks.Task PostprocessAsync(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions, System.Threading.CancellationToken token)
        {
            storeVersion();
            // Nothing
            return System.Threading.Tasks.Task.CompletedTask;
        }


        public override Marten.Internal.Operations.OperationRole Role()
        {
            return Marten.Internal.Operations.OperationRole.Upsert;
        }


        public override string CommandText()
        {
            return COMMAND_TEXT;
        }


        public override NpgsqlTypes.NpgsqlDbType DbType()
        {
            return NpgsqlTypes.NpgsqlDbType.Text;
        }


        public override void ConfigureParameters(Npgsql.NpgsqlParameter[] parameters, AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session)
        {
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            parameters[0].Value = session.Serializer.ToJson(_document);
            // .Net Class Type
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;
            parameters[1].Value = _document.GetType().FullName;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;

            if (document.VCode != null)
            {
                parameters[2].Value = document.VCode;
            }

            else
            {
                parameters[2].Value = System.DBNull.Value;
            }

            setVersionParameter(parameters[3]);
        }

    }

    // END: UpsertVerenigingOperation537441598
    
    
    // START: InsertVerenigingOperation537441598
    public class InsertVerenigingOperation537441598 : Marten.Internal.Operations.StorageOperation<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly AssociationRegistry.Vereniging.Vereniging _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public InsertVerenigingOperation537441598(AssociationRegistry.Vereniging.Vereniging document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
        {
            _document = document;
            _id = id;
            _versions = versions;
            _mapping = mapping;
        }


        public const string COMMAND_TEXT = "select public.mt_insert_vereniging(?, ?, ?, ?)";


        public override void Postprocess(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions)
        {
            storeVersion();
        }


        public override System.Threading.Tasks.Task PostprocessAsync(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions, System.Threading.CancellationToken token)
        {
            storeVersion();
            // Nothing
            return System.Threading.Tasks.Task.CompletedTask;
        }


        public override Marten.Internal.Operations.OperationRole Role()
        {
            return Marten.Internal.Operations.OperationRole.Insert;
        }


        public override string CommandText()
        {
            return COMMAND_TEXT;
        }


        public override NpgsqlTypes.NpgsqlDbType DbType()
        {
            return NpgsqlTypes.NpgsqlDbType.Text;
        }


        public override void ConfigureParameters(Npgsql.NpgsqlParameter[] parameters, AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session)
        {
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            parameters[0].Value = session.Serializer.ToJson(_document);
            // .Net Class Type
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;
            parameters[1].Value = _document.GetType().FullName;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;

            if (document.VCode != null)
            {
                parameters[2].Value = document.VCode;
            }

            else
            {
                parameters[2].Value = System.DBNull.Value;
            }

            setVersionParameter(parameters[3]);
        }

    }

    // END: InsertVerenigingOperation537441598
    
    
    // START: UpdateVerenigingOperation537441598
    public class UpdateVerenigingOperation537441598 : Marten.Internal.Operations.StorageOperation<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly AssociationRegistry.Vereniging.Vereniging _document;
        private readonly string _id;
        private readonly System.Collections.Generic.Dictionary<string, System.Guid> _versions;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public UpdateVerenigingOperation537441598(AssociationRegistry.Vereniging.Vereniging document, string id, System.Collections.Generic.Dictionary<string, System.Guid> versions, Marten.Schema.DocumentMapping mapping) : base(document, id, versions, mapping)
        {
            _document = document;
            _id = id;
            _versions = versions;
            _mapping = mapping;
        }


        public const string COMMAND_TEXT = "select public.mt_update_vereniging(?, ?, ?, ?)";


        public override void Postprocess(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions)
        {
            storeVersion();
            postprocessUpdate(reader, exceptions);
        }


        public override async System.Threading.Tasks.Task PostprocessAsync(System.Data.Common.DbDataReader reader, System.Collections.Generic.IList<System.Exception> exceptions, System.Threading.CancellationToken token)
        {
            storeVersion();
            await postprocessUpdateAsync(reader, exceptions, token);
        }


        public override Marten.Internal.Operations.OperationRole Role()
        {
            return Marten.Internal.Operations.OperationRole.Update;
        }


        public override string CommandText()
        {
            return COMMAND_TEXT;
        }


        public override NpgsqlTypes.NpgsqlDbType DbType()
        {
            return NpgsqlTypes.NpgsqlDbType.Text;
        }


        public override void ConfigureParameters(Npgsql.NpgsqlParameter[] parameters, AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session)
        {
            parameters[0].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
            parameters[0].Value = session.Serializer.ToJson(_document);
            // .Net Class Type
            parameters[1].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar;
            parameters[1].Value = _document.GetType().FullName;
            parameters[2].NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Text;

            if (document.VCode != null)
            {
                parameters[2].Value = document.VCode;
            }

            else
            {
                parameters[2].Value = System.DBNull.Value;
            }

            setVersionParameter(parameters[3]);
        }

    }

    // END: UpdateVerenigingOperation537441598
    
    
    // START: QueryOnlyVerenigingSelector537441598
    public class QueryOnlyVerenigingSelector537441598 : Marten.Internal.CodeGeneration.DocumentSelectorWithOnlySerializer, Marten.Linq.Selectors.ISelector<AssociationRegistry.Vereniging.Vereniging>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public QueryOnlyVerenigingSelector537441598(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public AssociationRegistry.Vereniging.Vereniging Resolve(System.Data.Common.DbDataReader reader)
        {

            AssociationRegistry.Vereniging.Vereniging document;
            document = _serializer.FromJson<AssociationRegistry.Vereniging.Vereniging>(reader, 0);
            return document;
        }


        public async System.Threading.Tasks.Task<AssociationRegistry.Vereniging.Vereniging> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {

            AssociationRegistry.Vereniging.Vereniging document;
            document = await _serializer.FromJsonAsync<AssociationRegistry.Vereniging.Vereniging>(reader, 0, token).ConfigureAwait(false);
            return document;
        }

    }

    // END: QueryOnlyVerenigingSelector537441598
    
    
    // START: LightweightVerenigingSelector537441598
    public class LightweightVerenigingSelector537441598 : Marten.Internal.CodeGeneration.DocumentSelectorWithVersions<AssociationRegistry.Vereniging.Vereniging, string>, Marten.Linq.Selectors.ISelector<AssociationRegistry.Vereniging.Vereniging>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public LightweightVerenigingSelector537441598(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public AssociationRegistry.Vereniging.Vereniging Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);

            AssociationRegistry.Vereniging.Vereniging document;
            document = _serializer.FromJson<AssociationRegistry.Vereniging.Vereniging>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            return document;
        }


        public async System.Threading.Tasks.Task<AssociationRegistry.Vereniging.Vereniging> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);

            AssociationRegistry.Vereniging.Vereniging document;
            document = await _serializer.FromJsonAsync<AssociationRegistry.Vereniging.Vereniging>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            return document;
        }

    }

    // END: LightweightVerenigingSelector537441598
    
    
    // START: IdentityMapVerenigingSelector537441598
    public class IdentityMapVerenigingSelector537441598 : Marten.Internal.CodeGeneration.DocumentSelectorWithIdentityMap<AssociationRegistry.Vereniging.Vereniging, string>, Marten.Linq.Selectors.ISelector<AssociationRegistry.Vereniging.Vereniging>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public IdentityMapVerenigingSelector537441598(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public AssociationRegistry.Vereniging.Vereniging Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            AssociationRegistry.Vereniging.Vereniging document;
            document = _serializer.FromJson<AssociationRegistry.Vereniging.Vereniging>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            return document;
        }


        public async System.Threading.Tasks.Task<AssociationRegistry.Vereniging.Vereniging> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            AssociationRegistry.Vereniging.Vereniging document;
            document = await _serializer.FromJsonAsync<AssociationRegistry.Vereniging.Vereniging>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            return document;
        }

    }

    // END: IdentityMapVerenigingSelector537441598
    
    
    // START: DirtyTrackingVerenigingSelector537441598
    public class DirtyTrackingVerenigingSelector537441598 : Marten.Internal.CodeGeneration.DocumentSelectorWithDirtyChecking<AssociationRegistry.Vereniging.Vereniging, string>, Marten.Linq.Selectors.ISelector<AssociationRegistry.Vereniging.Vereniging>
    {
        private readonly Marten.Internal.IMartenSession _session;
        private readonly Marten.Schema.DocumentMapping _mapping;

        public DirtyTrackingVerenigingSelector537441598(Marten.Internal.IMartenSession session, Marten.Schema.DocumentMapping mapping) : base(session, mapping)
        {
            _session = session;
            _mapping = mapping;
        }



        public AssociationRegistry.Vereniging.Vereniging Resolve(System.Data.Common.DbDataReader reader)
        {
            var id = reader.GetFieldValue<string>(0);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            AssociationRegistry.Vereniging.Vereniging document;
            document = _serializer.FromJson<AssociationRegistry.Vereniging.Vereniging>(reader, 1);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            StoreTracker(_session, document);
            return document;
        }


        public async System.Threading.Tasks.Task<AssociationRegistry.Vereniging.Vereniging> ResolveAsync(System.Data.Common.DbDataReader reader, System.Threading.CancellationToken token)
        {
            var id = await reader.GetFieldValueAsync<string>(0, token);
            if (_identityMap.TryGetValue(id, out var existing)) return existing;

            AssociationRegistry.Vereniging.Vereniging document;
            document = await _serializer.FromJsonAsync<AssociationRegistry.Vereniging.Vereniging>(reader, 1, token).ConfigureAwait(false);
            _session.MarkAsDocumentLoaded(id, document);
            _identityMap[id] = document;
            StoreTracker(_session, document);
            return document;
        }

    }

    // END: DirtyTrackingVerenigingSelector537441598
    
    
    // START: QueryOnlyVerenigingDocumentStorage537441598
    public class QueryOnlyVerenigingDocumentStorage537441598 : Marten.Internal.Storage.QueryOnlyDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public QueryOnlyVerenigingDocumentStorage537441598(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(AssociationRegistry.Vereniging.Vereniging document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.VCode)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.VCode;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(AssociationRegistry.Vereniging.Vereniging document)
        {
            return document.VCode;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.QueryOnlyVerenigingSelector537441598(session, _document);
        }


        public override Npgsql.NpgsqlCommand BuildLoadCommand(string id, string tenant)
        {
            return new NpgsqlCommand(_loaderSql).With("id", id);
        }


        public override Npgsql.NpgsqlCommand BuildLoadManyCommand(System.String[] ids, string tenant)
        {
            return new NpgsqlCommand(_loadArraySql).With("ids", ids);
        }

    }

    // END: QueryOnlyVerenigingDocumentStorage537441598
    
    
    // START: LightweightVerenigingDocumentStorage537441598
    public class LightweightVerenigingDocumentStorage537441598 : Marten.Internal.Storage.LightweightDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public LightweightVerenigingDocumentStorage537441598(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(AssociationRegistry.Vereniging.Vereniging document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.VCode)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.VCode;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(AssociationRegistry.Vereniging.Vereniging document)
        {
            return document.VCode;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.LightweightVerenigingSelector537441598(session, _document);
        }


        public override Npgsql.NpgsqlCommand BuildLoadCommand(string id, string tenant)
        {
            return new NpgsqlCommand(_loaderSql).With("id", id);
        }


        public override Npgsql.NpgsqlCommand BuildLoadManyCommand(System.String[] ids, string tenant)
        {
            return new NpgsqlCommand(_loadArraySql).With("ids", ids);
        }

    }

    // END: LightweightVerenigingDocumentStorage537441598
    
    
    // START: IdentityMapVerenigingDocumentStorage537441598
    public class IdentityMapVerenigingDocumentStorage537441598 : Marten.Internal.Storage.IdentityMapDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public IdentityMapVerenigingDocumentStorage537441598(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(AssociationRegistry.Vereniging.Vereniging document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.VCode)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.VCode;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(AssociationRegistry.Vereniging.Vereniging document)
        {
            return document.VCode;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.IdentityMapVerenigingSelector537441598(session, _document);
        }


        public override Npgsql.NpgsqlCommand BuildLoadCommand(string id, string tenant)
        {
            return new NpgsqlCommand(_loaderSql).With("id", id);
        }


        public override Npgsql.NpgsqlCommand BuildLoadManyCommand(System.String[] ids, string tenant)
        {
            return new NpgsqlCommand(_loadArraySql).With("ids", ids);
        }

    }

    // END: IdentityMapVerenigingDocumentStorage537441598
    
    
    // START: DirtyTrackingVerenigingDocumentStorage537441598
    public class DirtyTrackingVerenigingDocumentStorage537441598 : Marten.Internal.Storage.DirtyCheckedDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly Marten.Schema.DocumentMapping _document;

        public DirtyTrackingVerenigingDocumentStorage537441598(Marten.Schema.DocumentMapping document) : base(document)
        {
            _document = document;
        }



        public override string AssignIdentity(AssociationRegistry.Vereniging.Vereniging document, string tenantId, Marten.Storage.IMartenDatabase database)
        {
            if (string.IsNullOrEmpty(document.VCode)) throw new InvalidOperationException("Id/id values cannot be null or empty");
            return document.VCode;
        }


        public override Marten.Internal.Operations.IStorageOperation Update(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpdateVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Insert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.InsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Upsert(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {

            return new Marten.Generated.DocumentStorage.UpsertVerenigingOperation537441598
            (
                document, Identity(document),
                session.Versions.ForType<AssociationRegistry.Vereniging.Vereniging, string>(),
                _document
                
            );
        }


        public override Marten.Internal.Operations.IStorageOperation Overwrite(AssociationRegistry.Vereniging.Vereniging document, Marten.Internal.IMartenSession session, string tenant)
        {
            throw new System.NotSupportedException();
        }


        public override string Identity(AssociationRegistry.Vereniging.Vereniging document)
        {
            return document.VCode;
        }


        public override Marten.Linq.Selectors.ISelector BuildSelector(Marten.Internal.IMartenSession session)
        {
            return new Marten.Generated.DocumentStorage.DirtyTrackingVerenigingSelector537441598(session, _document);
        }


        public override Npgsql.NpgsqlCommand BuildLoadCommand(string id, string tenant)
        {
            return new NpgsqlCommand(_loaderSql).With("id", id);
        }


        public override Npgsql.NpgsqlCommand BuildLoadManyCommand(System.String[] ids, string tenant)
        {
            return new NpgsqlCommand(_loadArraySql).With("ids", ids);
        }

    }

    // END: DirtyTrackingVerenigingDocumentStorage537441598
    
    
    // START: VerenigingBulkLoader537441598
    public class VerenigingBulkLoader537441598 : Marten.Internal.CodeGeneration.BulkLoader<AssociationRegistry.Vereniging.Vereniging, string>
    {
        private readonly Marten.Internal.Storage.IDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string> _storage;

        public VerenigingBulkLoader537441598(Marten.Internal.Storage.IDocumentStorage<AssociationRegistry.Vereniging.Vereniging, string> storage) : base(storage)
        {
            _storage = storage;
        }


        public const string MAIN_LOADER_SQL = "COPY public.mt_doc_vereniging(\"mt_dotnet_type\", \"id\", \"mt_version\", \"data\") FROM STDIN BINARY";

        public const string TEMP_LOADER_SQL = "COPY mt_doc_vereniging_temp(\"mt_dotnet_type\", \"id\", \"mt_version\", \"data\") FROM STDIN BINARY";

        public const string COPY_NEW_DOCUMENTS_SQL = "insert into public.mt_doc_vereniging (\"id\", \"data\", \"mt_version\", \"mt_dotnet_type\", mt_last_modified) (select mt_doc_vereniging_temp.\"id\", mt_doc_vereniging_temp.\"data\", mt_doc_vereniging_temp.\"mt_version\", mt_doc_vereniging_temp.\"mt_dotnet_type\", transaction_timestamp() from mt_doc_vereniging_temp left join public.mt_doc_vereniging on mt_doc_vereniging_temp.id = public.mt_doc_vereniging.id where public.mt_doc_vereniging.id is null)";

        public const string OVERWRITE_SQL = "update public.mt_doc_vereniging target SET data = source.data, mt_version = source.mt_version, mt_dotnet_type = source.mt_dotnet_type, mt_last_modified = transaction_timestamp() FROM mt_doc_vereniging_temp source WHERE source.id = target.id";

        public const string CREATE_TEMP_TABLE_FOR_COPYING_SQL = "create temporary table mt_doc_vereniging_temp as select * from public.mt_doc_vereniging limit 0";


        public override string CreateTempTableForCopying()
        {
            return CREATE_TEMP_TABLE_FOR_COPYING_SQL;
        }


        public override string CopyNewDocumentsFromTempTable()
        {
            return COPY_NEW_DOCUMENTS_SQL;
        }


        public override string OverwriteDuplicatesFromTempTable()
        {
            return OVERWRITE_SQL;
        }


        public override void LoadRow(Npgsql.NpgsqlBinaryImporter writer, AssociationRegistry.Vereniging.Vereniging document, Marten.Storage.Tenant tenant, Marten.ISerializer serializer)
        {
            writer.Write(document.GetType().FullName, NpgsqlTypes.NpgsqlDbType.Varchar);
            writer.Write(document.VCode, NpgsqlTypes.NpgsqlDbType.Text);
            writer.Write(Marten.Schema.Identity.CombGuidIdGeneration.NewGuid(), NpgsqlTypes.NpgsqlDbType.Uuid);
            writer.Write(serializer.ToJson(document), NpgsqlTypes.NpgsqlDbType.Jsonb);
        }


        public override async System.Threading.Tasks.Task LoadRowAsync(Npgsql.NpgsqlBinaryImporter writer, AssociationRegistry.Vereniging.Vereniging document, Marten.Storage.Tenant tenant, Marten.ISerializer serializer, System.Threading.CancellationToken cancellation)
        {
            await writer.WriteAsync(document.GetType().FullName, NpgsqlTypes.NpgsqlDbType.Varchar, cancellation);
            await writer.WriteAsync(document.VCode, NpgsqlTypes.NpgsqlDbType.Text, cancellation);
            await writer.WriteAsync(Marten.Schema.Identity.CombGuidIdGeneration.NewGuid(), NpgsqlTypes.NpgsqlDbType.Uuid, cancellation);
            await writer.WriteAsync(serializer.ToJson(document), NpgsqlTypes.NpgsqlDbType.Jsonb, cancellation);
        }


        public override string MainLoaderSql()
        {
            return MAIN_LOADER_SQL;
        }


        public override string TempLoaderSql()
        {
            return TEMP_LOADER_SQL;
        }

    }

    // END: VerenigingBulkLoader537441598
    
    
    // START: VerenigingProvider537441598
    public class VerenigingProvider537441598 : Marten.Internal.Storage.DocumentProvider<AssociationRegistry.Vereniging.Vereniging>
    {
        private readonly Marten.Schema.DocumentMapping _mapping;

        public VerenigingProvider537441598(Marten.Schema.DocumentMapping mapping) : base(new VerenigingBulkLoader537441598(new QueryOnlyVerenigingDocumentStorage537441598(mapping)), new QueryOnlyVerenigingDocumentStorage537441598(mapping), new LightweightVerenigingDocumentStorage537441598(mapping), new IdentityMapVerenigingDocumentStorage537441598(mapping), new DirtyTrackingVerenigingDocumentStorage537441598(mapping))
        {
            _mapping = mapping;
        }


    }

    // END: VerenigingProvider537441598
    
    
}

