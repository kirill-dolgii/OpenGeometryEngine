namespace OpenGeometryEngineTests;

public abstract class PolyLineRegionTestBase : TestBase
{
    protected override DirectoryInfo TestDataDirectory 
        => new (Path.Combine(base.TestDataDirectory.FullName, "PolyLineRegionTestsData"));
}