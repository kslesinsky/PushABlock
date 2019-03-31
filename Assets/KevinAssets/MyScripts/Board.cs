using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IBoard
{
    Square SquareAt(Pos pos);
    IEnumerable<PositionedThing> GetAllPositionedThings();
    PositionedThing GetPositionedThing(int id);
    IEnumerable<Character> GetCharacters();
    IEnumerable<Pos> GetSpecialSquarePositions(SquareType squareType);
    ISurroundings GetSurroundings(Character character);
    bool TryMove(int characterId, Move move, out IEnumerable<PositionedThing> posThingsThatMoved);
}

public class BoardCore
{
    public const int MAX_X = 8;
    public const int MAX_Y = 8;

    protected Square[,] square; // the array of Squares

    protected Dictionary<int, PositionedThing> theThings = new Dictionary<int, PositionedThing>();
    //key is thing.IdOnBoard

    protected Dictionary<SquareType, IEnumerable<Pos>> specialSquarePos = new Dictionary<SquareType, IEnumerable<Pos>>();

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
        if (pos == null)
            return null;
        return SquareAt(pos.X, pos.Y);
    }
    public Square SquareAt(int x, int y) // this signature needed?
    {
        if (x < 0 || y < 0 || x > MAX_X || y > MAX_Y)
            return null;
            //throw new Exception("Trying to GetSquare out of bounds: " + x + "," + y);
        return square[x, y];
    }

    protected List<Pos> GetSpecialSquarePosList(SquareType squareType)
    {
        if (!specialSquarePos.ContainsKey(squareType))
            specialSquarePos[squareType] = new List<Pos>();
        return (List<Pos>)specialSquarePos[squareType];
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
        AddThingToBoard(player, pfPlayer);
        var robot = new Robot();
        var pfRobot = new PosFace(0, 3, Facing.East);
        AddThingToBoard(robot, pfRobot);
        robot = new Robot();
        pfRobot = new PosFace(8, 4, Facing.West);
        AddThingToBoard(robot, pfRobot);

        var block = new Block(isGameBlock: true);
        var pfBlock = new PosFace(4, 1);
        AddThingToBoard(block, pfBlock);
        block = new Block();
        pfBlock = new PosFace(2, 3);
        AddThingToBoard(block, pfBlock);
        block = new Block();
        pfBlock = new PosFace(6, 5);
        AddThingToBoard(block, pfBlock);
        block = new Block();
        pfBlock = new PosFace(7, 6);
        AddThingToBoard(block, pfBlock);

        AddSquareDesignator(new Pos(8, 8), SquareType.Goal);
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

    // returns true if successfully added
    public bool AddSquareDesignator(Pos pos, SquareType squareType)
    {
        var square = SquareAt(pos);
        if (square == null)
            return false;
        //TODO: check if Square already has a SquareType != 0

        square.SquareType = squareType;
        var ssPosList = GetSpecialSquarePosList(squareType);
        ssPosList.Add(pos);
        return true;
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
    public PositionedThing GetPositionedThing(Thing thing)
    {
        return GetPositionedThing(thing.IdOnBoard);
    }

    public IEnumerable<Character> GetCharacters()
    {
        return theThings.Values.Where(x => (x.Thing is Character)).Select(x => x.Thing as Character);
        // ? does the above need ToArray() ? concern about a race condition...
    }

    public IEnumerable<Pos> GetSpecialSquarePositions(SquareType squareType)
    {
        var posList = GetSpecialSquarePosList(squareType);
        return posList;
        // ?should do ToArray? and should copy the Pos's so they can't be altered?
    }

    public ISurroundings GetSurroundings(Character character)
    {
        return null; //TODO: implement!
    }



    // ----- Try Moving -----

    // returns true iff move is successful
    public bool TryMove(int characterId, Move move, out IEnumerable<PositionedThing> posThingsThatMoved)
    {
        posThingsThatMoved = new List<PositionedThing>();

        var characterPosThing = GetPositionedThing(characterId);
        if (!(characterPosThing.Thing is Character))
            return false; // throw new NonCriticalException?

        var character = (Character)characterPosThing.Thing;
        var posFace = characterPosThing.PosFace;

        bool changed = false;
        if (move.ChangesPosition())
        {
            var moveVector = Pos.MoveVectorFor(posFace.Facing, move.MoveType);
            var newPos = ((Pos)posFace) + moveVector;
            var newSquare = SquareAt(newPos);
            if (newSquare == null)
                return false; // move fails

            if (newSquare.ThingOnMe != null) // maybe trying "pushing" it
            {
                //if (move.MoveType != MoveType.Forward)
                //    return false; // can only push forwards

                PositionedThing posThing1;
                bool moved = TryMoveThing(newPos, moveVector, out posThing1);
                if (!moved)
                    return false; // couldn't "push" it; move fails

                ((List<PositionedThing>)posThingsThatMoved).Add(posThing1);
            }
            ClearSquareAt(posFace);
            newSquare.ThingOnMe = character;
            characterPosThing.PosFace.SetPos(newPos);

            changed = true;
        }

        if (move.ChangesFacing())
        {
            characterPosThing.PosFace.Rotate(move.MoveType);
            changed = true;
        }

        if (changed)
            ((List<PositionedThing>)posThingsThatMoved).Add(characterPosThing);
        return true;
    }

    private bool TryMoveThing(Pos pos, Pos moveVector, out PositionedThing posThing) //?optional arg for Square
    {
        posThing = null;
        var square0 = SquareAt(pos);
        var pos1 = pos + moveVector;
        var square1 = SquareAt(pos1);
        //Note: we're ignoring the possibility of having squares in between square0 and square1
        if (square0 == null || square1 == null || !square1.CanMoveOnToMe())
            return false;
        if (square0.ThingOnMe == null)
            return true; // there was nothing to move; call that success

        posThing = GetPositionedThing(square0.ThingOnMe);
        square0.ThingOnMe = null;
        square1.ThingOnMe = posThing.Thing;
        posThing.PosFace.SetPos(pos1);
        return true;
    }
}
