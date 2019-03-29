using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquareType { Empty = 0, Blocky = 1, CharStart = 2, GameBlockStart = 3, Goal = 4 }
//Blocky:starts with a block on it
//Elsewhere? designate facing of CharStart

public abstract class Thing
{
    public int IdOnBoard = -1;
}

public class Block : Thing { }

public abstract class Character : Thing
{
    public abstract DesiredMove GetDesiredMove(ISurroundings surroundings);
}
public class Player : Character
{
    public override DesiredMove GetDesiredMove(ISurroundings surroundings)
    {
        return null; // comes from user input
    }
}
public class Robot : Character
{
    public override DesiredMove GetDesiredMove(ISurroundings surroundings)
    {
        return new DesiredMove
        {
            PrimaryMove = new Move(MoveType.Forward),
            SecondaryMove = new Move(MoveType.RotRight)
        };
    }
}

// A wrapper class, used because a Thing doesn't know its position or facing
public class PositionedThing
{
    public Thing Thing { get; set; }
    public PosFace PosFace { get; set; }

    public PositionedThing() { }
    public PositionedThing(Thing thing, PosFace posFace)
    {
        this.Thing = thing;
        this.PosFace = posFace;
    }
}

public interface ISquare // ?not needed?
{
    Thing ThingOnMe { get; }
    bool CanMoveOnToMe();
}
public class Square : ISquare
{
    public Thing ThingOnMe { get; set; }

    public bool CanMoveOnToMe()
    {
        return (ThingOnMe == null); // there can only be one thing on a square
    }

    //public Square() { }
    //public Square(Thing thing)
    //{
    //    ThingOnMe = thing;
    //}
}
