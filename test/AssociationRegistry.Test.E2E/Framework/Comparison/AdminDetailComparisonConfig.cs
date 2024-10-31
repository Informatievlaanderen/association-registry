namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using Admin.Api.Verenigingen.Detail.ResponseModels;
using Bogus;
using Events;
using FluentAssertions.Equivalency;

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
            { typeof(Contactgegeven), [nameof(Contactgegeven.ContactgegevenId)] },
        };

        // Ignore specific properties
        IgnoreProperty<Locatie>(x => x.Adres);
        IgnoreProperty<Locatie>(x => x.AdresId);
        IgnoreProperty<Locatie>(x => x.Adresvoorstelling);
        IgnoreProperty<Locatie>(x => x.VerwijstNaar);

        IgnoreProperty<VertegenwoordigerContactgegevens>(x => x.id);
    }
}
