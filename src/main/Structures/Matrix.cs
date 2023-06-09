using System;
using System.Text;

namespace OpenGeometryEngine;

public readonly struct Matrix
{
    private readonly double[,] _data;

    public Matrix() => _data = new double[4, 4];

    public static Matrix CreateScale(float sx, float sy, float sz)
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

    public static Matrix CreateTranslation(float dx, float dy, float dz)
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

    public static Matrix CreateRotation(Vector direction, double angle)
    {
        // Normalize the axis vector
        Vector unitDir = direction.Normalize();

        // Precompute trigonometric values
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        float oneMinusCos = 1f - cos;

        // Calculate the rotation matrix elements
        double xx = unitDir.X * unitDir.X;
        double xy = unitDir.X * unitDir.Y;
        double xz = unitDir.X * unitDir.Z;
        double yy = unitDir.Y * unitDir.Y;
        double yz = unitDir.Y * unitDir.Z;
        double zz = unitDir.Z * unitDir.Z;
        double lxy = xy * oneMinusCos;
        double lyz = yz * oneMinusCos;
        double lzx = xz * oneMinusCos;
        double xSin = unitDir.X * sin;
        double ySin = unitDir.Y * sin;
        double zSin = unitDir.Z * sin;

        return new Matrix
        {
            _data =
            {
                [0, 0] = cos + xx * oneMinusCos,
                [0, 1] = lxy - zSin,
                [0, 2] = lzx + ySin,
                [0, 3] = 0f,

                [1, 0] = lxy + zSin,
                [1, 1] = cos + yy * oneMinusCos,
                [1, 2] = lyz - xSin,
                [1, 3] = 0f,

                [2, 0] = lzx - ySin,
                [2, 1] = lyz + xSin,
                [2, 2] = cos + zz * oneMinusCos,
                [2, 3] = 0f,

                [3, 0] = 0f,
                [3, 1] = 0f,
                [3, 2] = 0f,
                [3, 3] = 1f
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++) sb.Append($"{_data[i, j]:.000} ");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}