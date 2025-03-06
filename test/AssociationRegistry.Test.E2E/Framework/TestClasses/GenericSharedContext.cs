namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using ApiSetup;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;

[Collection(WellKnownCollections.GenericSharedContext)]
public class GenericSharedContext
{
    public GenericSharedContext(FullBlownApiSetup apiSetup)
    {
    }
}
