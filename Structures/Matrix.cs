using System;

namespace OpenGeometryEngine;

public readonly struct Matrix
{
    private readonly float[,] _data;

    public Matrix() => _data = new float[4, 4];

    public Matrix CreateScale(float sx, float sy, float sz)
    {
        return new Matrix
        {
            _data =
            {
                [0, 0] = sx,
                [1, 1] = sy,
                [2, 2] = sz,
                [3, 3] = 1f
            }
        };
    }

    public Matrix CreateTranslation(float dx, float dy, float dz)
    {
        return new Matrix
        {
            _data =
            {
                [0, 0] = 1f,
                [1, 1] = 1f,
                [2, 2] = 1f,
                [3, 3] = 1f,
                [0, 3] = dx,
                [1, 3] = dy,
                [2, 3] = dz,
            }
        };
    }

    public static Point operator *(Matrix transformationMatrix, Point point)
    {
        var pointAsArray = new[] { point.X, point.Y, point.Z, 1 };
        var newPointAsArray = new double[4];

        for (int i = 0; i < 4; i++)
        {
            var sum = 0d;
            for (int j = 0; j < 4; j++) sum += transformationMatrix._data[i, j] * pointAsArray[j];
            newPointAsArray[i] = sum;
        }
        return new Point(newPointAsArray[0], newPointAsArray[1], newPointAsArray[2]);
    }

    public static Vector operator *(Matrix transformationMatrix, Vector vector)
    {
        var result = transformationMatrix * new Point(vector.X, vector.Y, vector.Z);
        return new Vector(result.X, result.Y, result.Z);
    }
}