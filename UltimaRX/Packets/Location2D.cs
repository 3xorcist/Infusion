﻿using System;

namespace UltimaRX.Packets
{
    public struct Location2D
    {
        public Location2D(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public ushort X { get; }
        public ushort Y { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is Location2D))
            {
                var otherLocation = (Location3D)obj;
                return Equals(otherLocation);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode()*397) ^ Y.GetHashCode();
            }
        }

        public bool Equals(Location2D other) => X == other.X && Y == other.Y;

        public static bool operator ==(Location2D location1, Location2D location2) => location1.Equals(location2);

        public static bool operator !=(Location2D location1, Location2D location2) => !location1.Equals(location2);

        public static explicit operator Location3D(Location2D location) =>
            new Location3D(location.X, location.Y, 0);

        public override string ToString() => $"{X}, {Y}";

        public Location2D LocationInDirection(Direction direction)
        {
            Vector directionVector;

            if (direction == Direction.East)
                directionVector = Vector.EastVector;
            else if (direction == Direction.North)
                directionVector = Vector.NorthVector;
            else if (direction == Direction.Northeast)
                directionVector = Vector.NortheastVector;
            else if (direction == Direction.Northwest)
                directionVector = Vector.NorthwestVector;
            else if (direction == Direction.Southeast)
                directionVector = Vector.SoutheastVector;
            else if (direction == Direction.Southwest)
                directionVector = Vector.SouthwestVector;
            else if (direction == Direction.South)
                directionVector = Vector.SouthVector;
            else if (direction == Direction.West)
                directionVector = Vector.WestVector;
            else
                throw new ArgumentOutOfRangeException(nameof(direction), $"Unknown direction {direction}");

            return new Location2D((ushort)(X + directionVector.X), (ushort)(Y + directionVector.Y));
        }

        public static Vector operator -(Location2D location1, Location2D location2) =>
            new Vector((short)(location1.X - location2.X), (short)(location1.Y - location2.Y),0);
    }
}