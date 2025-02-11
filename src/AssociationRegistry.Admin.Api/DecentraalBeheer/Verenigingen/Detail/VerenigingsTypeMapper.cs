namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using ResponseModels;
using Vereniging;

public interface IVerenigingsTypeMapper
{
    VerenigingsType Map(Schema.Detail.VerenigingsType verenigingsType);
}

public class VerenigingsTypeMapper : IVerenigingsTypeMapper
{
    public VerenigingsType Map(Schema.Detail.VerenigingsType verenigingsType)
    {
        // TODO: uncomment when implementing VZER
        // if (Vereniging.Verenigingstype.IsVerenigingZonderEigenRechtspersoonlijkheid(verenigingsType.Code))
        // {
        //     return new VerenigingsType
        //     {
        //         Code = Vereniging.Verenigingstype.FeitelijkeVereniging.Code,
        //         Naam = Vereniging.Verenigingstype.FeitelijkeVereniging.Naam,
        //     };
        // }

        return new VerenigingsType
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam,
        };
    }
}

public class VerenigingsTypeMapperV2 : IVerenigingsTypeMapper
{
    public VerenigingsType Map(Schema.Detail.VerenigingsType verenigingsType)
    {
        if (Verenigingstype.IsVerenigingZonderEigenRechtspersoonlijkheid(verenigingsType.Code))
        {
            return new VerenigingsType
            {
                Code = Verenigingstype.VZER.Code,
                Naam = Verenigingstype.VZER.Naam,
            };
        }

        return new VerenigingsType
        {
            Code = verenigingsType.Code,
            Naam = verenigingsType.Naam,
        };
    }
}
