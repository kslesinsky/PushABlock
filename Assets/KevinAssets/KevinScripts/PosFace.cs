using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Facing { Undefined = 0, North = 1, West = 2, East = 3, South = 4 }

// A Position (also used for Vectors)
public class Pos
{
    public int X;
    public int Y;
    public Pos(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public Pos(Pos pos)
    {
        this.X = pos.X;
        this.Y = pos.Y;
    }

    public void SetPos(Pos pos)
    {
        this.X = pos.X;
        this.Y = pos.Y;
    }

    public static Pos operator +(Pos a, Pos b)
    {
        return new Pos(a.X + b.X, a.Y + b.Y);
    }
    public static Pos operator *(Pos p, int i)
    {
        return new Pos(p.X * i, p.Y * i);
    }

    //TODO: make readonly somewhere? and static?
    // Get the vector in the direction of the specified Facing
    public static Pos ForwardFor(Facing f)
    {
        switch (f)
        {
            case (Facing.North):
                return new Pos(0, 1);
            case (Facing.West):
                return new Pos(-1, 0);
            case (Facing.East):
                return new Pos(1, 0);
            case (Facing.South):
                return new Pos(0, -1);
            default:
                return new Pos(0, 0);
        }
    }
    public static Pos MoveVectorFor(Facing f, MoveType moveType)
    {
        switch (moveType)
        {
            case MoveType.Forward:
                return ForwardFor(f);
            case MoveType.Reverse:
                return ForwardFor(f) * -1;
            default:
                return new Pos(0, 0);
        }
    }
}

// A Position & Facing
public class PosFace : Pos
{
    public Facing Facing;

    public PosFace(int x, int y, Facing facing) : base(x, y)
    {
        this.Facing = facing;
    }
    public PosFace(PosFace posFace) : base(posFace)
    {
        this.Facing = posFace.Facing;
    }

    public void Rotate(MoveType moveType)
    {
        this.Facing = RotatedFacing(this.Facing, moveType);
    }

    public static Facing RotatedFacing(Facing inFacing, MoveType moveType)
    {
        if (moveType == MoveType.RotLeft)
        {
            switch (inFacing)
            {
                case Facing.North:
                    return Facing.West;
                case Facing.East:
                    return Facing.North;
                case Facing.South:
                    return Facing.East;
                case Facing.West:
                    return Facing.South;
                default:
                    return Facing.Undefined;
            }
        }
        else if (moveType == MoveType.RotRight)
        {
            switch (inFacing)
            {
                case Facing.North:
                    return Facing.East;
                case Facing.West:
                    return Facing.North;
                case Facing.South:
                    return Facing.West;
                case Facing.East:
                    return Facing.South;
                default:
                    return Facing.Undefined;
            }
        }
        else
            return inFacing;
    }
}
