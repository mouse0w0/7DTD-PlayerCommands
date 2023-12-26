using System;
using UnityEngine;

namespace PlayerCommands;

public struct Location : IEquatable<Location>
{
    public float x;
    public float y;
    public float z;
    public float pitch;
    public float yaw;

    public Vector3 GetPosition() => new(x, y, z);

    public Vector3 GetRotation() => new(pitch, yaw, 0);

    public string ToPositionString() => Utility.ToPositionString(x, z);

    public Location(Entity entity) : this(entity.position, entity.rotation)
    {
    }

    public Location(Vector3 position, Vector3? rotation = null) :
        this(position.x, position.y, position.z, rotation?.x ?? 0, rotation?.y ?? 0)
    {
    }

    public Location(float x, float y, float z, float pitch = 0, float yaw = 0)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.pitch = pitch;
        this.yaw = yaw;
    }

    public bool Equals(Location other)
    {
        return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && pitch.Equals(other.pitch) &&
               yaw.Equals(other.yaw);
    }

    public override bool Equals(object obj)
    {
        return obj is Location other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = x.GetHashCode();
            hashCode = (hashCode * 397) ^ y.GetHashCode();
            hashCode = (hashCode * 397) ^ z.GetHashCode();
            hashCode = (hashCode * 397) ^ pitch.GetHashCode();
            hashCode = (hashCode * 397) ^ yaw.GetHashCode();
            return hashCode;
        }
    }
}