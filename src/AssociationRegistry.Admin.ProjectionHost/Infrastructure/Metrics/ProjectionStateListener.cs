namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Metrics;

using Marten;
using Marten.Events;
using Marten.Events.Daemon;
using Marten.Events.Daemon.Internals;
using Marten.Internal.Operations;
using Marten.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Projections.Detail;
using Projections.Historiek;
using Projections.Locaties;
using System.Reflection;

public class ProjectionStateListener : DocumentSessionListenerBase
{
    private readonly AdminInstrumentation _adminInstrumentation;

    public ProjectionStateListener(AdminInstrumentation adminInstrumentation)
    {
        _adminInstrumentation = adminInstrumentation;
    }

    public override Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        if (commit is not IUnitOfWork uow) return Task.CompletedTask;
        var operations = uow.OperationsFor<IEvent>();

        foreach (var operation in operations)
        {
            var range = GetEventRange(operation);

            if (range is null) continue;

            if (range.ShardName.ProjectionName == "BeheerVerenigingZoekenDocument")
                _adminInstrumentation.VerenigingZoekenEventValue = range.SequenceCeiling;

            if (range.ShardName.ProjectionName == typeof(BeheerVerenigingDetailProjection).FullName)
                _adminInstrumentation.VerenigingDetailEventValue = range.SequenceCeiling;

            if (range.ShardName.ProjectionName == typeof(LocatieLookupProjection).FullName)
                _adminInstrumentation.LocatieLookupEventValue = range.SequenceCeiling;

            if (range.ShardName.ProjectionName == typeof(BeheerVerenigingHistoriekProjection).FullName)
                _adminInstrumentation.VerenigingHistoriekEventValue = range.SequenceCeiling;
        }

        return Task.CompletedTask;
    }

    private static EventRange? GetEventRange(IStorageOperation opperation)
    {
        return opperation.GetType().Name switch
        {
            "InsertProjectionProgress" => opperation.GetType().GetField(name: "_progress", BindingFlags.NonPublic | BindingFlags.Instance)
                                                    .GetValue(opperation) as EventRange,
            "UpdateProjectionProgress" => opperation.GetType().GetProperty("Range").GetValue(opperation) as EventRange,
            _ => null,
        };
    }
}

public class AllFieldsContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var props = type
                   .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                   .Select(p => base.CreateProperty(p, memberSerialization))
                   .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                              .Select(f => base.CreateProperty(f, memberSerialization)))
                   .ToList();

        props.ForEach(p =>
        {
            p.Writable = true;
            p.Readable = true;
        });

        return props;
    }
}
