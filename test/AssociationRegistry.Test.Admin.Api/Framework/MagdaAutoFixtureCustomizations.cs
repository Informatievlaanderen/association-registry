namespace AssociationRegistry.Test.Admin.Api.Framework;

using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using AutoFixture;

public static class MagdaAutoFixtureCustomizations
{
    public static void CustomizeMagdaResponses(this IFixture fixture)
    {
        fixture.DoNotAutoGenerateUitzonderingen();
        fixture.OnlyAllowSupportedRechtsvorm();
        fixture.OnlyAllowActiveStatus();
        fixture.OnlyAllowOnderneming();
        fixture.OnlyAllowRechtspersoon();

        fixture.Customize<Onderneming2_0Type>(
            composer => composer.With(o => o.Rechtsvormen, fixture.CreateMany<RechtsvormExtentieType>(1).ToArray));
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
                        Value = Rechtsvorm.All[i % Rechtsvorm.All.Length].CodeVolgensMagda,
                    },
                    DatumBegin = null,
                    DatumEinde = null,
                }).OmitAutoProperties());
    }

    private static void DoNotAutoGenerateUitzonderingen(this IFixture fixture)
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
}
