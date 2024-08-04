namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class HistoriekDataComparer : BaseTypeComparer
{
    public HistoriekDataComparer(RootComparer rootComparer) : base(rootComparer)
    {
    }

    public override bool IsTypeMatch(Type type1, Type type2)
        => type1 == typeof(JObject) || type2 == typeof(JObject);

    public override void CompareType(CompareParms parms)
    {
        var expectedType = parms.Object1.GetType() == typeof(JObject) ? parms.Object2.GetType() : parms.Object1.GetType();

        var actualValue = parms.Object1.GetType() == typeof(JObject)
            ? JsonConvert.DeserializeObject(((JObject)parms.Object1).ToString(), expectedType)
            : parms.Object1;

        var expectedValue = parms.Object2.GetType() == typeof(JObject)
            ? JsonConvert.DeserializeObject(((JObject)parms.Object2).ToString(), expectedType)
            : parms.Object2;

        if (actualValue == null || expectedValue == null)
            throw new InvalidOperationException("Unable to deserialize the JObject to the expected type.");

        RootComparer.Compare(new CompareParms
        {
            Result = parms.Result,
            Config = parms.Config,
            Object1 = actualValue,
            Object2 = expectedValue,
            ParentObject1 = parms.ParentObject1,
            ParentObject2 = parms.ParentObject2,
            BreadCrumb = parms.BreadCrumb,
        });
    }
}
