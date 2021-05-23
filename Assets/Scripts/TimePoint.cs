using UnityEngine;

[System.Serializable]
public class TimePoint
{
    public Transform ObjTransform;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    public Vector3 Velocity;
    public bool IsActive = false;
    
    public TimePoint(
        Vector3 Position,
        Quaternion Rotation, 
        Vector3 Scale,
        Vector3 Velocity)
    {
        this.Position = Position;
        this.Rotation = Rotation;
        this.Scale = Scale;
        this.Velocity = Velocity;
    }

    public TimePoint(
        Vector3 Position,
        Quaternion Rotation,
        Vector3 Scale,
        Vector3 Velocity, 
        bool IsActive
        )
    {
        this.Position = Position;
        this.Rotation = Rotation;
        this.Scale = Scale;
        this.Velocity = Velocity;
        this.IsActive = IsActive;
    }


}
