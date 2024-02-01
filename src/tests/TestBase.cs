namespace OpenGeometryEngineTests;

public class TestBase
{
    protected DirectoryInfo AssemblyDirectory => (new FileInfo(typeof(TestBase).Assembly.Location)).Directory;

    protected virtual DirectoryInfo TestDataDirectory => new (Path.Combine(AssemblyDirectory.FullName, "TestData"));
}