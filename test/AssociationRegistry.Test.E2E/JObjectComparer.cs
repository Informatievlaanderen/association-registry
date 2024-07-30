namespace AssociationRegistry.Test.E2E;

using KellermanSoftware.CompareNetObjects;
using KellermanSoftware.CompareNetObjects.TypeComparers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JObjectComparer : BaseTypeComparer
{
    public JObjectComparer(RootComparer rootComparer) : base(rootComparer)
    {
    }

    public override bool IsTypeMatch(Type type1, Type type2)
    {
        return type1 == typeof(JObject);
    }

    public override void CompareType(CompareParms parms)
    {
        if (parms.Object1 == null || parms.Object2 == null)
        {
            if (parms.Object1 != parms.Object2)
            {
                AddDifference(parms);
            }
            return;
        }

        try
        {
            // Convert both objects to JSON strings
            string json1 = parms.Object1 is JObject ? ((JObject)parms.Object1).ToString(Formatting.Indented) : JsonConvert.SerializeObject(parms.Object1, Formatting.Indented);
            string json2 = parms.Object2 is JObject ? ((JObject)parms.Object2).ToString(Formatting.Indented) : JsonConvert.SerializeObject(parms.Object2, Formatting.Indented);

            // Deserialize the JSON strings back to the type of the other object
            object otherObject = parms.Object1 is JObject ? parms.Object2 : parms.Object1;
            var typed1 = JsonConvert.DeserializeObject(json1, otherObject.GetType());
            var typed2 = JsonConvert.DeserializeObject(json2, otherObject.GetType());

            // Create new CompareParms to avoid direct call to RootComparer.Compare
            var newParms = new CompareParms
            {
                Config = parms.Config,
                Result = parms.Result,
                ParentObject1 = parms.ParentObject1,
                ParentObject2 = parms.ParentObject2,
                Object1 = typed1,
                Object2 = typed2,
                BreadCrumb = parms.BreadCrumb
            };

            // Perform comparison on deserialized objects
            RootComparer.Compare(newParms);
        }
        catch (Exception ex)
        {
            parms.Result.Differences.Add(new Difference
            {
                ParentObject1 = parms.ParentObject1,
                ParentObject2 = parms.ParentObject2,
                PropertyName = parms.BreadCrumb,
                Object1Value = parms.Object1?.ToString(),
                Object2Value = parms.Object2?.ToString(),
                MessagePrefix = $"Error converting JSON strings to target type: {ex.Message}"
            });
        }
    }
}
