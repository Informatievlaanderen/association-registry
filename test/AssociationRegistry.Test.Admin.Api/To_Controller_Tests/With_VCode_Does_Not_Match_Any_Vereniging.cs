namespace AssociationRegistry.Test.Admin.Api.To_Controller_Tests;

using System.Net;
using Fixtures;
using Framework;
using VCodes;
using AutoFixture;
using FluentAssertions;
using Xunit;

//TODO rework into repository / eventstore test
public class With_A_NonExisting_VCode_Fixture : AdminApiFixture
{
    public HttpResponseMessage Response = null!;
    private readonly string _vCode;
    private const string NieuweVerenigingsNaam = "De nieuwe vereniging";

    public With_A_NonExisting_VCode_Fixture() : base(
        nameof(With_A_NonExisting_VCode_Fixture))
    {
        var fixture = new Fixture().CustomizeAll();
        _vCode = fixture.Create<VCode>();
    }

    protected override async Task Given()
        => await Task.CompletedTask;

    protected override async Task When()
    {
        var jsonBody = $@"{{""naam"":""{NieuweVerenigingsNaam}""}}";
        Response = await AdminApiClient.PatchVereniging(_vCode, jsonBody);
    }
}

public class With_A_NonExisting_VCode : IClassFixture<With_A_NonExisting_VCode_Fixture>
{
    private readonly With_A_NonExisting_VCode_Fixture _apiFixture;

    public With_A_NonExisting_VCode(With_A_NonExisting_VCode_Fixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public void Then_it_returns_a_bad_request_response()
    {
        _apiFixture.Response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
