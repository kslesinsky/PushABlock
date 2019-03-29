using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBoard
{
    IEnumerable<PositionedThing> GetAllPositionedThings();
    PositionedThing GetPositionedThing(int id);
    IEnumerable<Character> GetCharacters();
    //PosFace GetSpecialSquarePos(SquareType squareType);
    ISurroundings GetSurroundings(Character character);
    bool TryMove(int characterId, Move move, out PositionedThing posThing);
}

public class BoardCore
{
    public const int MAX_X = 8;
    public const int MAX_Y = 8;

    protected Square[,] square; // the array of Squares

    protected Dictionary<int, PositionedThing> theThings = new Dictionary<int, PositionedThing>();
    //key is thing.IdOnBoard

    public BoardCore()
    {
        square = new Square[MAX_X + 1, MAX_Y + 1];
        for (int y = 0; y <= MAX_Y; y++)
        {
            for (int x = 0; x <= MAX_X; x++)
            {
                square[x, y] = new Square();
            }
        }
    }

    public Square SquareAt(Pos pos)
    {
        return SquareAt(pos.X, pos.Y);
    }
    public Square SquareAt(int x, int y) // this signature needed?
    {
        if (x < 0 || y < 0 || x > MAX_X || y > MAX_Y)
            return null;
            //throw new Exception("Trying to GetSquare out of bounds: " + x + "," + y);
        return square[x, y];
    }
}
public class Board : BoardCore, IBoard
{
    int nextThingId = 0;
    int NewEmptyThing()
    {
        int thingId = nextThingId++;
        theThings[thingId] = new PositionedThing();
        return thingId;
    } // Take the return value, remember the Id, and assign Thing & PosFace to theThings[thingId]

    public void SetupForTesting()
    {
        var player = new Player();
        var pfPlayer = new PosFace(4, 0, Facing.North);
        var robot = new Robot();
        var pfRobot = new PosFace(0, 2, Facing.East);

        AddThingToBoard(player, pfPlayer);
        AddThingToBoard(robot, pfRobot);
    }

    // returns true if successfully added
    public bool AddThingToBoard(Thing thing, PosFace posFace)
    {
        //check if thing.IdOnBoard >= 0 ?
        var square = SquareAt(posFace);
        //check for null?
        if (square.ThingOnMe == null)
        {
            int id = NewEmptyThing();
            thing.IdOnBoard = id;
            theThings[id].Thing = thing;
            theThings[id].PosFace = posFace;
            square.ThingOnMe = thing;
            return true;
        }
        //else throw new NonCriticalException
        return false; // didn't add
    }

    public void ClearSquareAt(Pos pos)
    {
        var square = SquareAt(pos);
        if (square != null)
            square.ThingOnMe = null;
    }

    public IEnumerable<PositionedThing> GetAllPositionedThings()
    {
        return theThings.Values;
    }
    public PositionedThing GetPositionedThing(int id)
    {
        if (!theThings.ContainsKey(id))
            return null;
        return theThings[id];
    }

    public IEnumerable<Character> GetCharacters()
    {
        return theThings.Values.Where(x => (x.Thing is Character)).Select(x => x.Thing as Character);
        // ? does the above need ToArray() ? concern about a race condition...
    }

    public ISurroundings GetSurroundings(Character character)
    {
        return null; //TODO: implement!
    }

    // returns true iff move is successful
    public bool TryMove(int characterId, Move move, out PositionedThing posThing)
    {
        posThing = GetPositionedThing(characterId);
        if (!(posThing.Thing is Character))
            return false; // throw new NonCriticalException?

        var character = (Character)posThing.Thing;
        var posFace = posThing.PosFace;

        //bool changed = false;
        if (move.ChangesPosition())
        {
            var moveVector = Pos.MoveVectorFor(posFace.Facing, move.MoveType);
            var newPos = ((Pos)posFace) + moveVector;
            var newSquare = SquareAt(newPos);
            if (newSquare == null || !newSquare.CanMoveOnToMe())
                return false; // move fails

            ClearSquareAt(posFace);
            newSquare.ThingOnMe = character;
            posThing.PosFace.SetPos(newPos);

            //changed = true;
        }

        if (move.ChangesFacing())
        {
            posThing.PosFace.Rotate(move.MoveType);

            //changed = true;
        }

        //if (changed) ...trigger MovedRotatedThing / MovedRotatedCharacter

        return true;
    }
}
