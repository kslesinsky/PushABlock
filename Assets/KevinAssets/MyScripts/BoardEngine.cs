using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEventArgs : EventArgs
{
    public int ThingId { get; set; }
    public PosFace NewPosFace { get; set; }
    public String Message { get; set; }
}

public class BoardEngine
{
    public event EventHandler<BoardEventArgs> ThingMoved;
    protected virtual void OnThingMoved(PositionedThing posThing)
    {
        var handler = ThingMoved;
        if (handler != null)
        {
            var args = new BoardEventArgs // make a copy of the values
            {
                ThingId = posThing.Thing.IdOnBoard,
                NewPosFace = new PosFace(posThing.PosFace)
            };
            handler(null, args);
            //handler(this, args); // don't need to pass this ?
        }
    }

    public event EventHandler<BoardEventArgs> DebugMessageEvent;
    protected virtual void DebugMessage(string message)
    {
        var handler = DebugMessageEvent;
        if (handler != null)
        {
            var args = new BoardEventArgs
            {
                Message = message
            };
            handler(null, args);
        }
    }
    // --- done Events ---

    private IBoard theBoard;
    public IBoard TheBoard
    {
        get { return theBoard; }
    }

    public int CompletedRounds { get; private set; }

    public BoardEngine(IBoard board)
    {
        theBoard = board;
    }
    // --- done Constructor ---

    private MoveType? simpleMoveQueue; // for now, one item max in the queue
    public bool AddToMoveQueue(MoveType moveType)
    {
        if (simpleMoveQueue == null)
        {
            simpleMoveQueue = moveType;
            return true;
        }
        else
        {
            return false; // queue was full; move not added
        }
    }
    private MoveType? RemoveNextMoveFromQueue()
    {
        if (simpleMoveQueue == null)
            return null;

        var nextMove = simpleMoveQueue.Value;
        simpleMoveQueue = null;
        return nextMove;
    }
    // -- done MoveQueue ---

    // Used to instantiate the GameObjects in the Scene
    public IEnumerable<PositionedThing> GetPositionedThings()
    {
        return theBoard.GetAllPositionedThings();
    }

    public IEnumerator Run()
    {
        //TODO: logic to wait a max of X seconds (2?) before a round is played and the robots move
        //TODO: logic to break out when the game ends or is quit
        while (1 == 1)
        {
            MoveType? playerMove;
            while ((playerMove = RemoveNextMoveFromQueue()) == null)
            {
                yield return new WaitForSeconds(.1f);
            }
            DesiredMove playerDesiredMove = new DesiredMove
            {
                PrimaryMove = new Move(playerMove.Value),
                SecondaryMove = null
            };

            //?Change this to GetCharacterIds() or GetThingIds(ThingType.Character)?
            foreach (var character in theBoard.GetCharacters())
            {
                DoASingleTurn(character, playerDesiredMove);
                yield return new WaitForSeconds(.1f); // ?change to a regular yield return
            }
            //?WaitForSeconds here?

            CompletedRounds++;
            DebugMessage(CompletedRounds + " rounds complete");

            CheckSpecialSquares();
        }
    }

    private void CheckSpecialSquares()
    {
        var positions = theBoard.GetSpecialSquarePositions(SquareType.Goal);
        foreach (var goalPos in positions)
        {
            var square = theBoard.SquareAt(goalPos);
            if (square != null && square.ThingOnMe != null)
            {
                if (square.ThingOnMe is Block && ((Block)square.ThingOnMe).IsGameBlock)
                {
                    DebugMessage("You win!");
                }
            }
        }
    }

    private void DoASingleTurn(Character character, DesiredMove playerDesiredMove)
    {
        DesiredMove desiredMove;
        if (character is Player) // we assume there's one Player on the board
        {
            desiredMove = playerDesiredMove;
        }
        else // is a Robot ?
        {
            var surroundings = theBoard.GetSurroundings(character);
            desiredMove = character.GetDesiredMove(surroundings);
        }
        if (desiredMove.PrimaryMove != null)
        {
            IEnumerable<PositionedThing> posThingsThatMoved;
            bool success = theBoard.TryMove(character.IdOnBoard, desiredMove.PrimaryMove, out posThingsThatMoved);
            if (!success && desiredMove.SecondaryMove != null)
            {
                success = theBoard.TryMove(character.IdOnBoard, desiredMove.SecondaryMove, out posThingsThatMoved);
            }
            if (success)
            {
                foreach (var posThing in posThingsThatMoved)
                {
                    OnThingMoved(posThing);
                }
            }
        }
    }
}
