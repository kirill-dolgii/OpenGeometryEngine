namespace OpenGeometryEngineTests;

public abstract class PolygonRegionTestBase : TestBase
{
    protected override DirectoryInfo TestDataDirectory => new (Path.Combine(base.TestDataDirectory.FullName, "PolylineRegionData"));
}