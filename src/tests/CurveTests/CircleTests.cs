using OpenGeometryEngine;
using OpenGeometryEngine.Classes.Curves;

[TestFixture]
public class CircleTests
{

    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void TEST_EVALUATE()
    {
        var circle = new Circle(Frame.World, 1);
        var zeroEval = circle.Evaluate(0);
        var doublePiEval = circle.Evaluate(Math.PI * 2);
        var angle90Eval = circle.Evaluate(Math.PI / 2);
        Assert.That(zeroEval.Param, Is.EqualTo(0));
        Assert.That(zeroEval.Point, Is.EqualTo(new Point(1, 0, 0)));
        Assert.That(doublePiEval.Param, Is.EqualTo(Math.PI * 2));
        Assert.That(doublePiEval.Point, Is.EqualTo(new Point(1, 0, 0)));        
        Assert.That(angle90Eval.Param, Is.EqualTo(Math.PI / 2));
        Assert.IsTrue(angle90Eval.Point == new Point(0, 1, 0));
    }

}