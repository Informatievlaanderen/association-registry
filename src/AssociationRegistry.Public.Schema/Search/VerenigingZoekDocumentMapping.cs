namespace AssociationRegistry.Public.Schema.Search;

using Elastic.Clients.Elasticsearch.Mapping;

public static class VerenigingZoekDocumentMapping
{
    public const string PubliekZoekenNormalizer = "publiek_zoeken_normalizer";
    public const string PubliekZoekenAnalyzer = "publiek_zoeken_analyzer";

    public static TypeMapping Get() => new TypeMapping
    {
        Properties = new Properties
        {
            ["vCode"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer },
            ["naam"] = new TextProperty
            {
                Analyzer = PubliekZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                }
            },
            ["jsonLdMetadataType"] = new TextProperty
            {
                Analyzer = PubliekZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                }
            },
            ["roepnaam"] = new TextProperty
            {
                Analyzer = PubliekZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                }
            },
            ["korteNaam"] = new TextProperty
            {
                Analyzer = PubliekZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                }
            },
            ["korteBeschrijving"] = new TextProperty
            {
                Analyzer = PubliekZoekenAnalyzer,
                Fields = new Properties
                {
                    ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                }
            },
            ["status"] = new KeywordProperty(),
            ["isUitgeschrevenUitPubliekeDatastroom"] = new BooleanProperty(),
            ["isVerwijderd"] = new BooleanProperty(),
            ["verenigingstype"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["code"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer },
                    ["naam"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    }
                }
            },
            ["doelgroep"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["jsonLdMetadata"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["id"] = new TextProperty(),
                            ["type"] = new TextProperty()
                        }
                    },
                    ["minimumleeftijd"] = new IntegerNumberProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    },
                    ["maximumleeftijd"] = new IntegerNumberProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    }
                }
            },
            ["locaties"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["jsonLdMetadata"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["id"] = new TextProperty(),
                            ["type"] = new TextProperty()
                        }
                    },
                    ["locatieId"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    },
                    ["naam"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["adresvoorstelling"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["isPrimair"] = new BooleanProperty
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
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["locatietype"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    }
                }
            },
            ["hoofdactiviteitenVerenigingsloket"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["code"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    },
                    ["jsonLdMetadata"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["id"] = new TextProperty(),
                            ["type"] = new TextProperty()
                        }
                    },
                    ["naam"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    }
                }
            },
            ["werkingsgebieden"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["code"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty()
                        }
                    },
                    ["jsonLdMetadata"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["id"] = new TextProperty(),
                            ["type"] = new TextProperty()
                        }
                    },
                    ["naam"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    }
                }
            },
            ["lidmaatschappen"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["jsonLdMetadata"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["id"] = new TextProperty(),
                            ["type"] = new TextProperty()
                        }
                    },
                    ["andereVereniging"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["datumVan"] = new TextProperty(),
                    ["datumTot"] = new TextProperty(),
                    ["identificatie"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["beschrijving"] = new TextProperty
                    {
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    }
                }
            },
            ["sleutels"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["bron"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer },
                    ["jsonLdMetadata"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["id"] = new TextProperty(),
                            ["type"] = new TextProperty()
                        }
                    },
                    ["waarde"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["codeerSysteem"] = new TextProperty
                    {
                        Analyzer = PubliekZoekenAnalyzer,
                        Fields = new Properties
                        {
                            ["keyword"] = new KeywordProperty { Normalizer = PubliekZoekenNormalizer }
                        }
                    },
                    ["gestructureerdeIdentificator"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["jsonLdMetadata"] = new NestedProperty
                            {
                                IncludeInRoot = true,
                                Properties = new Properties
                                {
                                    ["id"] = new TextProperty(),
                                    ["type"] = new TextProperty()
                                }
                            },
                            ["nummer"] = new TextProperty
                            {
                                Fields = new Properties
                                {
                                    ["keyword"] = new KeywordProperty()
                                }
                            }
                        }
                    }
                }
            },
            ["geotags"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["identificatie"] = new TextProperty ()
                }
            },
            ["relaties"] = new NestedProperty
            {
                IncludeInRoot = true,
                Properties = new Properties
                {
                    ["relatietype"] = new KeywordProperty(),
                    ["andereVereniging"] = new NestedProperty
                    {
                        IncludeInRoot = true,
                        Properties = new Properties
                        {
                            ["kboNummer"] = new KeywordProperty(),
                            ["vCode"] = new KeywordProperty(),
                            ["naam"] = new TextProperty()
                        }
                    }
                }
            }
        }
    };
}
