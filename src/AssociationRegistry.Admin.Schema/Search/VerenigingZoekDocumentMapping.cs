namespace AssociationRegistry.Admin.Schema.Search;

using Elastic.Clients.Elasticsearch.Mapping;
using System.Collections.Generic;

public static class VerenigingZoekDocumentMapping
{
    public const string BeheerZoekenNormalizer = "beheer_zoeken_normalizer";
    public const string BeheerZoekenAnalyzer = "beheer_zoeken_analyzer";

    public static TypeMapping Get() => new TypeMapping
    {
        Properties = new Properties
        {
            ["vCode"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer },
            ["naam"] = new TextProperty
            {
                Analyzer = BeheerZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                }
            },
            ["korteNaam"] = new TextProperty
            {
                Analyzer = BeheerZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                }
            },
            ["roepnaam"] = new TextProperty
            {
                Analyzer = BeheerZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                }
            },
            ["startdatum"] = new DateProperty(),
            ["einddatum"] = new DateProperty(),
            ["status"] = new KeywordProperty(),
            ["isUitgeschrevenUitPubliekeDatastroom"] = new BooleanProperty(),
            ["isVerwijderd"] = new BooleanProperty(),
            ["isDubbel"] = new BooleanProperty(),
            ["jsonLdMetadataType"] = new TextProperty(),
            ["doelgroep"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["minimumleeftijd"] = new IntegerNumberProperty(),
                    ["maximumleeftijd"] = new IntegerNumberProperty(),
                }
            },
            ["verenigingstype"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["code"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer },
                    ["naam"] = new TextProperty
                    {
                        Analyzer = BeheerZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    }
                }
            },
            ["locaties"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["locatieId"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    },
                    ["naam"] = new TextProperty
                    {
                        Analyzer = BeheerZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    },
                    ["adresvoorstelling"] = new TextProperty
                    {
                        Analyzer = BeheerZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    },
                    ["isPrimair"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    },
                    ["postcode"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    },
                    ["gemeente"] = new TextProperty
                    {
                        Analyzer = BeheerZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    },
                    ["locatietype"] = new TextProperty
                    {
                        Analyzer = BeheerZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    }
                }
            },
            ["geotags"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["identificatie"] = new TextProperty(),
                }
            },
            ["lidmaatschappen"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["lidmaatschapId"] = new IntegerNumberProperty(),
                    ["andereVereniging"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    },
                    ["datumVan"] = new TextProperty(),
                    ["datumTot"] = new TextProperty(),
                    ["beschrijving"] = new TextProperty
                    {
                        Analyzer = BeheerZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    },
                    ["identificatie"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = BeheerZoekenNormalizer }
                        }
                    }
                }
            }
        }
    };
}
