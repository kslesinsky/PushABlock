using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Forward = 1, RotLeft = 2, RotRight = 3, Reverse = 4 }

public class Move
{
    public MoveType MoveType { get; set; }

    public Move(MoveType moveType)
    {
        this.MoveType = moveType;
    }

    public bool ChangesPosition()
    {
        return Move.ChangesPosition(this.MoveType);
    }
    public bool ChangesFacing()
    {
        return Move.ChangesFacing(this.MoveType);
    }

    public static bool ChangesPosition(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Forward:
            case MoveType.Reverse:
                return true;
            default:
                return false;
        }
    }
    public static bool ChangesFacing(MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.RotLeft:
            case MoveType.RotRight:
                return true;
            default:
                return false;
        }
    }
}
public class DesiredMove
{
    public Move PrimaryMove { get; set; }
    public Move SecondaryMove { get; set; }
}
