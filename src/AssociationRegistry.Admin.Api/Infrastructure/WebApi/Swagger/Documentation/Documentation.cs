namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Documentation;

public static class Documentation
{
    public static string GetHeadContent()
    {
        var lessSpaceBetweenSections = @"
<script src=""https://cdn.redoc.ly/redoc/latest/bundles/redoc.standalone.js""> </script>
<script>
window.addEventListener('load', () => {
Redoc.init(
        '/docs/v1/docs.json',
    {
        'theme': {
            'spacing': {
                'sectionVertical': '0'
            },
        }
    },
    document.getElementById('redoc-container')
)
});
</script>";

        return $@"
{lessSpaceBetweenSections}
";
    }
}
