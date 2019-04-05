using System;
using System.Collections.Generic;
using System.Linq;

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

    // returns true if successfully added
    public bool AddThingToBoard(SquareType squareType, PosFace posFace)
    {
        if (squareType == SquareType.Goal)
        {
            return AddSquareDesignator(SquareType.Goal, posFace);
        }

        Thing newThing = Thing.Create(squareType);
        return AddThingToBoard(newThing, posFace);
    }

    public bool AddThingToBoard(Thing thing, PosFace posFace)
    {
        if (thing == null)
            return false;
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

    public bool AddSquareDesignator(SquareType squareType, Pos pos)
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
    // --- done Adding Things to the Board ---

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
    //TODO: separate GetPlayers and GetRobots - so can have players go first?
    //     ?make a GetThings<T> where you can specify a type that derives from Thing?

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

public static class BoardTest
{
    public static void SetupForTesting(Board b)
    {
        var pfPlayer = new PosFace(4, 0, Facing.North);
        b.AddThingToBoard(SquareType.PlayerStart, pfPlayer);
        var pfRobot = new PosFace(0, 3, Facing.East);
        b.AddThingToBoard(SquareType.RobotStart, pfRobot);
        pfRobot = new PosFace(8, 4, Facing.West);
        b.AddThingToBoard(SquareType.RobotStart, pfRobot);

        var pfBlock = new PosFace(4, 1);
        b.AddThingToBoard(SquareType.GameBlockStart, pfBlock);
        pfBlock = new PosFace(2, 3);
        b.AddThingToBoard(SquareType.Blocky, pfBlock);
        pfBlock = new PosFace(6, 5);
        b.AddThingToBoard(SquareType.Blocky, pfBlock);
        pfBlock = new PosFace(7, 6);
        b.AddThingToBoard(SquareType.Blocky, pfBlock);

        b.AddSquareDesignator(SquareType.Goal, new Pos(8, 8));
    }

    public static void SetupForTesting2(Board b)
    {
        String[] squares =
        {
            "0501",
            "4010",
            "0200",
            "0300"
        };
        String[] facings =
        {
            "0000",
            "3000",
            "0000",
            "0100"
        };
        int y = BoardCore.MAX_Y;
        for (int i = 0; i < squares.Length; i++)
        {
            string squaresRow = squares[i];
            string facingsRow = facings[i];
            for (int j = 0; j < squaresRow.Length; j++)
            {
                //Convert chars to SquareType & Facing
                int squareTypeInt;
                if (!int.TryParse(squaresRow[j].ToString(), out squareTypeInt))
                    continue;
                SquareType squareType = (SquareType)squareTypeInt;
                if (squareType == SquareType.Default)
                    continue; // nothing on that square

                PosFace posFace = new PosFace(j, y); // facing==Undefined

                //TODO: check length of facingsRow...
                int facingInt;
                if (int.TryParse(facingsRow[j].ToString(), out facingInt))
                    posFace.Facing = (Facing)facingInt;

                b.AddThingToBoard((SquareType)squareTypeInt, posFace);

                //TODO: AddSquareDesignator - for Goal
            }
            y--;
        }

    }
}
