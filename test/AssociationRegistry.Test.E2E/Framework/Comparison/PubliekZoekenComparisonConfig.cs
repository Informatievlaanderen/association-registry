namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using Public.Api.Verenigingen.Search.ResponseModels;

public class PubliekZoekenComparisonConfig : End2EndComparisonConfig
{
    public static readonly PubliekZoekenComparisonConfig Instance = new();

    public PubliekZoekenComparisonConfig()
    {
        IgnoreCollectionOrder = true;

        CollectionMatchingSpec = new Dictionary<Type, IEnumerable<string>>
        {
            { typeof(Sleutel), [nameof(Sleutel.id)] },
            { typeof(Werkingsgebied), [nameof(Werkingsgebied.Code)] },
        };

        // Ignore specific properties
        IgnoreProperty<Locatie>(x => x.Postcode);
        IgnoreProperty<Locatie>(x => x.Gemeente);
        IgnoreProperty<Locatie>(x => x.Adresvoorstelling);
    }
}
