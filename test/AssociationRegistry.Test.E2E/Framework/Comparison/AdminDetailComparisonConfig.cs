namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using System;
using System.Collections.Generic;

public class AdminDetailComparisonConfig : End2EndComparisonConfig
{
    public static readonly AdminDetailComparisonConfig Instance = new();

    public AdminDetailComparisonConfig()
    {
        IgnoreCollectionOrder = true;

        CollectionMatchingSpec = new Dictionary<Type, IEnumerable<string>>
        {
            { typeof(Vertegenwoordiger), [nameof(Vertegenwoordiger.id)] },
            { typeof(Sleutel), [nameof(Sleutel.id)] },
            { typeof(Werkingsgebied), [nameof(Werkingsgebied.Code)] },
        };

        // Ignore specific properties
        IgnoreProperty<Locatie>(x => x.Adres);
        IgnoreProperty<Locatie>(x => x.AdresId);
        IgnoreProperty<Locatie>(x => x.Adresvoorstelling);
        IgnoreProperty<Locatie>(x => x.VerwijstNaar);
    }
}
