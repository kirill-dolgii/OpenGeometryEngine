using System;

namespace OpenGeometryEngine.Structures
{
    /// <summary>
    /// Represents an axis-aligned spatial bounding box.
    /// </summary>
    public readonly struct Box
    {
        /// <summary>
        /// Gets the center point of the bounding box.
        /// </summary>
        public readonly Point Center;

        /// <summary>
        /// Gets the corners of the bounding box 
        /// where Corners[0] is MinCorner and Corners[1] is MaxCorner;
        /// </summary>
        public readonly Point[] Corners;

        /// <summary>
        /// Gets the maximum corner of the bounding box.
        /// </summary>
        public readonly Point MaxCorner;

        /// <summary>
        /// Gets the minimum corner of the bounding box.
        /// </summary>
        public readonly Point MinCorner;

        /// <summary>
        /// Gets the size of the bounding box.
        /// </summary>
        public readonly Vector Size;

        /// <summary>
        /// Gets a value indicating whether the bounding box is empty 
        /// i.e. a box that contains only a single Center point.
        /// </summary>
        public readonly bool IsEmpty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> struct representing 
        /// an empty box centered at a specific point.
        /// </summary>
        /// <param name="center">The center point of the bounding box.</param>
        private Box(Point center)
        {
            IsEmpty = true;
            Center = center;
            MaxCorner = center;
            MinCorner = center;
            Corners = new[] { MinCorner, MaxCorner };
            Size = new Vector();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> struct with specified minimum and maximum corners.
        /// </summary>
        /// <param name="minCorner">The minimum corner of the bounding box.</param>
        /// <param name="maxCorner">The maximum corner of the bounding box.</param>
        private Box(Point minCorner, Point maxCorner)
        {
            IsEmpty = false;
            Center = (minCorner + maxCorner.Vector) / 2;
            MinCorner = minCorner;
            MaxCorner = maxCorner;
            Size = MaxCorner - MinCorner;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Box"/> struct from two corner points.
        /// </summary>
        /// <param name="corner1">The first corner point.</param>
        /// <param name="corner2">The second corner point.</param>
        /// <returns>A new instance of the <see cref="Box"/> struct representing the bounding box.</returns>
        public static Box Create(Point corner1, Point corner2)
        {
            if (corner1 == corner2) return new Box(corner1);
            var maxX = Math.Max(corner1.X, corner2.X);
            var maxY = Math.Max(corner1.Y, corner2.Y);
            var maxZ = Math.Max(corner1.Z, corner2.Z);

            var minX = Math.Min(corner1.X, corner2.X);
            var minY = Math.Min(corner1.Y, corner2.Y);
            var minZ = Math.Min(corner1.Z, corner2.Z);

            return new Box(new Point(minX, minY, minZ), new Point(maxX, maxY, maxZ));
        }
    }
}
