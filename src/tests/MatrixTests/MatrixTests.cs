using OpenGeometryEngine;

namespace OpenGeometryEngineTests.MatrixTests;

[TestFixture]
public class MatrixTests
{
    [Test]
    public void Mapping()
    {
        var frameDirX = new Vector(0.687834219796478, 0.351255122458403, 0.635219588035274);
        var frameDirY = new Vector(-0.691074294163565, 0.584586693378039, 0.425058487589251);
        var frameDirZ = new Vector(-0.222036947428355, -0.731353701619171, 0.644842117967364);
        var frameOrigin = new Point(0.12314121, 0.01231231, 0.0031324);

        var frameToMap = new Frame(frameOrigin,
                                   frameDirX,
                                   frameDirY,
                                   frameDirZ);

        var mapping = Matrix.CreateMapping(frameToMap);

        var origin = mapping * Point.Origin;

        var dirX = mapping * Frame.World.DirX;
        var dirY = mapping * Frame.World.DirY;
        var dirZ = mapping * Frame.World.DirZ;
     
        Assert.That(origin, Is.EqualTo(frameOrigin));

        Assert.That(dirX, Is.EqualTo(frameDirX));
        Assert.That(dirY, Is.EqualTo(frameDirY));
        Assert.That(dirZ, Is.EqualTo(frameDirZ));
    }
}
