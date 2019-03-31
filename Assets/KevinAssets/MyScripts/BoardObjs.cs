﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Revisit this.  Thinking about the int vals in a json array to define a level.
//      But if we have Square.SquareType, we don't need to know if a Square started off with a block on it, etc... right?
public enum SquareType { Default = 0, Blocky = 1, CharStart = 2, GameBlockStart = 3, Goal = 4 }
//Blocky:starts with a block on it
//Elsewhere? designate facing of CharStart

public abstract class Thing
{
    public int IdOnBoard = -1;
}

public class Block : Thing
{
    public Block(bool isGameBlock = false)
    {
        this.IsGameBlock = isGameBlock;
    }

    public bool IsGameBlock { get; set; }
}

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

public class Square
{
    public Thing ThingOnMe { get; set; }

    public SquareType SquareType { get; set; }
    //TODO: allow more than one type (designator)

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
