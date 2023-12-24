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

        Assert.That(dirX, Is.EqualTo(frameDirX.Unit));
        Assert.That(dirY, Is.EqualTo(frameDirY.Unit));
        Assert.That(dirZ, Is.EqualTo(frameDirZ.Unit));

        var inverseMapping = mapping.Inverse();

        var inverseOrigin = inverseMapping * origin;

        var inverseDirX = inverseMapping * dirX;
        var inverseDirY = inverseMapping * dirY;
        var inverseDirZ = inverseMapping * dirZ;

        Assert.That(inverseOrigin, Is.EqualTo(Point.Origin));

        Assert.That(inverseDirX, Is.EqualTo(Frame.World.DirX));
        Assert.That(inverseDirY, Is.EqualTo(Frame.World.DirY));
        Assert.That(inverseDirZ, Is.EqualTo(Frame.World.DirZ));
    }

    [Test]
    public void INVERSE_ROTATE_AND_TRANSLATE()
    {
        double angle = 30 / 180.0 * Math.PI;
        var rot = Matrix.CreateRotation(Vector.UnitZ.Unit, angle);
        var transVec = new Vector(.1, .2, .3);
        var trans = Matrix.CreateTranslation(transVec.X, transVec.Y, transVec.Z);

        var resulting = trans * rot;
        var inverse = resulting.Inverse();

        var p = new Point(1, 0, 0);
        var newP = resulting * p;
        var inversedNewP = inverse * newP;

        Assert.That(inversedNewP, Is.EqualTo(p));
    }
}
