
//TODO: Revisit this.  Thinking about the int vals in a json array to define a level.
//      But if we have Square.SquareType, we don't need to know if a Square started off with a block on it, etc... right?
using System.Collections.Generic;

public enum SquareType { Default = 0, BlockStart = 1, GameBlockStart = 2, PlayerStart = 3, RobotStart = 4, Goal = 5 }
public enum SquareDesignator { Goal };

public abstract class Thing
{
    public int IdOnBoard = -1;

    public static Thing Create(SquareType squareType)
    {
        Thing thing = null;
        switch (squareType)
        {
            case SquareType.BlockStart:
            case SquareType.GameBlockStart:
                thing = new Block(isGameBlock: (squareType == SquareType.GameBlockStart));
                break;
            case SquareType.PlayerStart:
                thing = new Player();
                break;
            case SquareType.RobotStart:
                thing = new Robot();
                break;
        }
        return thing;
    }
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

    private List<SquareDesignator> _squareDesignators = new List<SquareDesignator>();
    public void AddDesignator(SquareDesignator designator)
    {
        //TODO: check if already added
        _squareDesignators.Add(designator);
    }
    public bool HasDesignator(SquareDesignator designator)
    {
        return _squareDesignators.Contains(designator);
    }
    //TODO? RemoveDesignator(), ClearDesignators()

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
