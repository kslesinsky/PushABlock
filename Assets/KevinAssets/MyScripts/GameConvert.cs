using UnityEngine;

public class GameConvert
{
    // Compute Vector3 from Pos
    public static Vector3 Vector3From(Pos pos)
    {
        return new Vector3(pos.X - (BoardCore.MAX_X / 2), 0, pos.Y - (BoardCore.MAX_Y / 2));
    }

    // Compute Rotation from Facing
    public static Quaternion QuaternionFrom(Facing facing)
    {
        switch (facing)
        {
            case Facing.North:
                return Quaternion.LookRotation(Vector3.forward);
            case Facing.West:
                return Quaternion.LookRotation(Vector3.left);
            case Facing.East:
                return Quaternion.LookRotation(Vector3.right);
            case Facing.South:
                return Quaternion.LookRotation(Vector3.back);
            default:
                return Quaternion.identity;
        }
    }
}
