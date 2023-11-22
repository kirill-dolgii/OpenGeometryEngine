using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Gets the vertices of the bounding box cuboid
        /// where Corners[0] is MinCorner and Corners[7] is MaxCorner.
        /// Vertices are in counterclockwise direction.
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
        private Box(Point center) : this(center, center)
        {
            IsEmpty = true;
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

            Corners = new Point[8];
            Corners[0] = MinCorner;
            Corners[1] = new Point(MaxCorner.X, MinCorner.Y, MinCorner.Z);
            Corners[2] = new Point(MaxCorner.X, MaxCorner.Y, MinCorner.Z);
            Corners[3] = new Point(MinCorner.X, MaxCorner.Y, MinCorner.Z);

            // Back face
            Corners[4] = new Point(MinCorner.X, MinCorner.Y, MaxCorner.Z);
            Corners[5] = new Point(MaxCorner.X, MinCorner.Y, MaxCorner.Z);
            Corners[6] = MaxCorner;
            Corners[7] = new Point(MinCorner.X, MaxCorner.Y, MaxCorner.Z);
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

        /// <summary>
        /// Creates a new instance of the <see cref="Box"/> struct from a collection of points.
        /// </summary>
        /// <param name="points">The collection of points to create the bounding box from.</param>
        /// <returns>A new instance of the <see cref="Box"/> struct representing the bounding box.</returns>
        public static Box Create(ICollection<Point> points)
        {
            if (points == null || points.Count == 0)
                throw new ArgumentException("Points collection cannot be null or empty.");

            // Initialize with the first point
            Point minCorner = points.First();
            Point maxCorner = points.First();

            foreach (var point in points)
            {
                minCorner = new Point(
                    Math.Min(minCorner.X, point.X),
                    Math.Min(minCorner.Y, point.Y),
                    Math.Min(minCorner.Z, point.Z)
                );

                maxCorner = new Point(
                    Math.Max(maxCorner.X, point.X),
                    Math.Max(maxCorner.Y, point.Y),
                    Math.Max(maxCorner.Z, point.Z)
                );
            }

            return new Box(minCorner, maxCorner);
        }

        /// <summary>
        /// Combines two bounding boxes into a new bounding box that encompasses both.
        /// </summary>
        /// <param name="box1">The first bounding box.</param>
        /// <param name="box2">The second bounding box.</param>
        /// <returns>A new bounding box that contains both input bounding boxes.</returns>
        public static Box operator &(Box box1, Box box2)
        {
            if (box1.IsEmpty)
                return box2;
            if (box2.IsEmpty)
                return box1;

            var combinedMinCorner = new Point(
                Math.Min(box1.MinCorner.X, box2.MinCorner.X),
                Math.Min(box1.MinCorner.Y, box2.MinCorner.Y),
                Math.Min(box1.MinCorner.Z, box2.MinCorner.Z)
            );

            var combinedMaxCorner = new Point(
                Math.Max(box1.MaxCorner.X, box2.MaxCorner.X),
                Math.Max(box1.MaxCorner.Y, box2.MaxCorner.Y),
                Math.Max(box1.MaxCorner.Z, box2.MaxCorner.Z)
            );

            return new Box(combinedMinCorner, combinedMaxCorner);
        }

        public static Box Unite(IEnumerable<Box> boxes)
        {
            if (boxes == null || !boxes.Any())
                return new Box();

            Box resultBox = boxes.First();

            foreach (var box in boxes.Skip(1))
            {
                resultBox = resultBox & box;
            }

            return resultBox;
        }
    }
}
