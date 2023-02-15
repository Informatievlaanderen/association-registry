namespace AssociationRegistry.Public.Api.Hoofdactiviteiten;

public record HoofdactiviteitenResponse(HoofdactiviteitenResponse.Hoofdactiviteit[] Hoofdactiviteiten)
{
    public record Hoofdactiviteit(string Code, string Beschrijving);
};
