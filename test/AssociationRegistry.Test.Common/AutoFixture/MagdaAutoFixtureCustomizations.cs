namespace AssociationRegistry.Test.Common.AutoFixture;

using global::AutoFixture;
using Magda.Constants;
using Magda.Models;
using Magda.Models.RegistreerInschrijving;
using Magda.Onderneming.GeefOnderneming;
using AntwoordenType = Magda.Repertorium.RegistreerInschrijving.AntwoordenType;
using ContextType = Magda.Repertorium.RegistreerInschrijving.ContextType;
using RepliekType = Magda.Repertorium.RegistreerInschrijving.RepliekType;

public static class MagdaAutoFixtureCustomizations
{
    public static void CustomizeMagdaResponses(this IFixture fixture)
    {
        fixture.DoNotAutoGenerateAntwoordUitzonderingen();
        fixture.OnlyAllowSupportedRechtsvorm();
        fixture.OnlyAllowActiveStatus();
        fixture.OnlyAllowOnderneming();
        fixture.OnlyAllowRechtspersoon();
        fixture.OnlyAllowNamenWithValidTaalCodes();
        fixture.DoNotAutoGenerateRepliekUitzonderingen();

        fixture.Customize<Onderneming2_0Type>(
            composer => composer
               .With(propertyPicker: o => o.Rechtsvormen, fixture.CreateMany<RechtsvormExtentieType>(1).ToArray()));

        fixture.Customize<NamenOndernemingType>(
            composer => composer
                       .With(propertyPicker: o => o.MaatschappelijkeNamen,
                             fixture.CreateMany<NaamOndernemingType>(4).DistinctBy(n => n.Taalcode).ToArray())
                       .With(propertyPicker: o => o.AfgekorteNamen,
                             fixture.CreateMany<NaamOndernemingType>(4).DistinctBy(n => n.Taalcode).ToArray())
        );
    }

    private static void OnlyAllowNamenWithValidTaalCodes(this IFixture fixture)
    {
        fixture.Customize<NaamOndernemingType>(
            composer => composer
                       .FromFactory<int>(
                            i => new NaamOndernemingType
                            {
                                Naam = fixture.Create<string>(),
                                DatumBegin = null,
                                DatumEinde = null,
                                Taalcode = new[]
                                {
                                    TaalCodes.Nederlands,
                                    TaalCodes.Frans,
                                    TaalCodes.Duits,
                                    TaalCodes.Engels,
                                }[i % 4],
                            }).OmitAutoProperties());
    }

    private static void OnlyAllowRechtspersoon(this IFixture fixture)
    {
        fixture.Customize<SoortOndernemingType>(
            composer => composer.FromFactory(
                () => new SoortOndernemingType
                {
                    Code = new CodeSoortOndernemingType
                    {
                        Value = SoortOndernemingCodes.Rechtspersoon,
                    },
                }).OmitAutoProperties());
    }

    private static void OnlyAllowOnderneming(this IFixture fixture)
    {
        fixture.Customize<OndernemingOfVestigingType>(
            composer => composer.FromFactory(
                () => new OndernemingOfVestigingType
                {
                    Code = new CodeOndernemingOfVestigingType
                    {
                        Value = OndernemingOfVestigingCodes.Onderneming,
                    },
                }).OmitAutoProperties());
    }

    private static void OnlyAllowActiveStatus(this IFixture fixture)
    {
        fixture.Customize<StatusKBOType>(
            composer => composer.FromFactory(
                () => new StatusKBOType
                {
                    Code = new CodeStatusKBOType
                    {
                        Value = StatusKBOCodes.Actief,
                    },
                }).OmitAutoProperties());
    }

    private static void OnlyAllowSupportedRechtsvorm(this IFixture fixture)
    {
        fixture.Customize<RechtsvormExtentieType>(
            composer => composer.FromFactory<int>(
                i => new RechtsvormExtentieType
                {
                    Code = new CodeRechtsvormType
                    {
                        Value = RechtsvormCodes.VZW,
                    },
                    DatumBegin = null,
                    DatumEinde = null,
                }).OmitAutoProperties());
    }

    private static void DoNotAutoGenerateAntwoordUitzonderingen(this IFixture fixture)
    {
        fixture.Customize<AntwoordType>(
            composer => composer.FromFactory(
                () => new AntwoordType
                {
                    Inhoud = fixture.Create<AntwoordInhoudType>(),
                    Referte = fixture.Create<string>(),
                    Uitzonderingen = Array.Empty<UitzonderingType>(),
                }
            ).OmitAutoProperties());
    }

    private static void DoNotAutoGenerateRepliekUitzonderingen(this IFixture fixture)
    {
        fixture.Customize<RepliekType>(
            composer => composer.FromFactory(
                () => new RepliekType()
                {
                    Uitzonderingen = Array.Empty<Magda.Repertorium.RegistreerInschrijving.UitzonderingType>(),
                    Antwoorden = fixture.Create<AntwoordenType>(),
                    Context = fixture.Create<ContextType>(),
                }
            ).OmitAutoProperties());
    }
}
