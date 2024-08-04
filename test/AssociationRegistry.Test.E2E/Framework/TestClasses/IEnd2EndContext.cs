namespace AssociationRegistry.Test.E2E.Framework.TestClasses;

using Alba;

public interface IEnd2EndContext<TRequest>
{
    string ResultingVCode { get; }
    TRequest Request { get; }

    IAlbaHost AdminApiHost { get; }
}
