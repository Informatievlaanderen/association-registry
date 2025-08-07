namespace AssociationRegistry.Admin.Schema.Search;

using Elastic.Clients.Elasticsearch.Mapping;
using System.Collections.Generic;

public static class DuplicateDetectionDocumentMapping
{
    public const string DuplicateMunicipalityAnalyzer = "duplicate_municipality_analyzer";
    public const string DuplicateAnalyzer = "duplicate_analyzer";
    public const string DuplicateFullNameAnalyzer = "duplicate_fullname_analyzer";

    public static TypeMapping Get()
        => new TypeMapping
        {
            Properties = new Properties
            {
                ["vCode"] = new KeywordProperty(),
                ["naam"] = new TextProperty
                {
                    Fields = new Properties
                    {
                        ["naam"] = new TextProperty
                        {
                            Analyzer = DuplicateAnalyzer
                        },
                        ["naamexact"] = new TextProperty
                        {
                            Analyzer = DuplicateFullNameAnalyzer
                        }
                    },
                    Analyzer = DuplicateAnalyzer
                },
                ["korteNaam"] = new TextProperty(),
                ["verenigingsTypeCode"] = new TextProperty(),
                ["verenigingssubtypeCode"] = new TextProperty(),
                ["hoofdactiviteitVerenigingsloket"] = new TextProperty(),
                ["isGestopt"] = new BooleanProperty(),
                ["isVerwijderd"] = new BooleanProperty(),
                ["isDubbel"] = new BooleanProperty(),
                ["locaties"] = new NestedProperty
                {
                    IncludeInRoot = true,
                    Properties = GetLocatieMapping()
                }
            }
        };

    private static Properties GetLocatieMapping()
        => new Properties
        {
            ["locatieId"] = new TextProperty(),
            ["naam"] = new TextProperty(),
            ["adresvoorstelling"] = new TextProperty(),
            ["isPrimair"] = new BooleanProperty(),
            ["locatietype"] = new TextProperty(),
            ["postcode"] = new TextProperty(),
            ["gemeente"] = new TextProperty
            {
                Analyzer = DuplicateMunicipalityAnalyzer
            }
        };
}
