namespace AssociationRegistry.Acm.Api.Infrastructure.Metrics;

using Marten;
using Marten.Events;
using Marten.Events.Daemon.Internals;
using Marten.Internal.Operations;
using Marten.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

public class ProjectionStateListener : DocumentSessionListenerBase
{
    private readonly AcmInstrumentation _acmInstrumentation;

    public ProjectionStateListener(AcmInstrumentation acmInstrumentation)
    {
        _acmInstrumentation = acmInstrumentation;
    }

    public override Task AfterCommitAsync(IDocumentSession session, IChangeSet commit, CancellationToken token)
    {
        if (commit is not IUnitOfWork uow) return Task.CompletedTask;
        var operations = uow.OperationsFor<IEvent>();

        foreach (var operation in operations)
        {
            var range = GetEventRange(operation);

            if (range is null) continue;

            if (range.ShardName.ProjectionName ==
                typeof(VerenigingenPerInszProjection).FullName)
                _acmInstrumentation.VerenigingPerInszEventValue = range.SequenceCeiling;
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
