﻿using System;

namespace Infusion
{
    public struct ObjectId
    {
        public ObjectId(uint value) => Value = value;
        public uint Value { get; }

        public static implicit operator ObjectId(uint value) => new ObjectId(value);

        public static implicit operator uint(ObjectId id) => id.Value;
        public static explicit operator int(ObjectId id) => (int)id.Value;
        public static explicit operator ObjectId(int id) => new ObjectId((uint)id);

        public static bool operator ==(ObjectId id1, ObjectId id2) => id1.Equals(id2);

        public static bool operator !=(ObjectId id1, ObjectId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is ObjectId id)
            {
                return Equals(id);
            }

            return false;
        }

        public bool Equals(ObjectId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"0x{Value:X8}";

        public bool IsMobile => Value < 0x40000000;
    }
}
