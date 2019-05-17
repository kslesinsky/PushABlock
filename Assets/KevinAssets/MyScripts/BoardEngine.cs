using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEventArgs : EventArgs
{
    public int ThingId { get; set; }
    public PosFace NewPosFace { get; set; }
    public String Message { get; set; }
    public int RoundsCompleted { get; set; }
    public bool Won { get; set; }
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

    public event EventHandler<BoardEventArgs> RoundCompleted;
    protected virtual void OnRoundCompleted()
    {
        var handler = RoundCompleted;
        if (handler != null)
        {
            var args = new BoardEventArgs
            {
                RoundsCompleted = this.CompletedRounds
            };
            handler(null, args);
        }
    }

    public event EventHandler<BoardEventArgs> GameEnded;
    protected virtual void OnGameEnded()
    {
        var handler = GameEnded;
        if (handler != null)
        {
            var args = new BoardEventArgs
            {
                Won = this.Won
            };
            handler(null, args);
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
    public bool UseBoard(IBoard board)
    {
        if (InPlay)
        {
            DebugMessage("Tried to set the BoardEngine's Board while in play.");
            return false;
        }
        theBoard = board;
        return true;
    }

    public bool InPlay { get; private set; }
    public int CompletedRounds { get; private set; }
    private bool Won { get; set; }
    public bool Quit { get; set; }

    public void RemoveTheBoardAndReset()
    {
        // ? check if InPlay ?
        theBoard = null;
        CompletedRounds = 0;
        Won = false;
        Quit = false;
    }

    // --- MoveQueue ---

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
    private void ClearMoveQueue()
    {
        simpleMoveQueue = null;
    }
    // --- done MoveQueue ---

    public IEnumerator Run()
    {
        //TODO: logic to wait a max of X seconds (2?) before a round is played and the robots move
        InPlay = true;
        while (InPlay)
        {
            MoveType? playerMove = null;
            while (!Quit && (playerMove = RemoveNextMoveFromQueue()) == null)
            {
                yield return new WaitForSeconds(.1f);
            }
            if (Quit)
            {
                InPlay = false;
                continue;
            }
            DesiredMove playerDesiredMove = new DesiredMove
            {
                PrimaryMove = new Move(playerMove.Value),
                SecondaryMove = null
            };

            //?Change this to GetCharacterIds() or GetThingIds(ThingType.Character)?
            foreach (var player in theBoard.GetThings<Player>())
            {
                DoSingleTurn(player, playerDesiredMove);
                yield return new WaitForSeconds(.1f); // ?change to a regular yield return
            }
            foreach (var robot in theBoard.GetThings<Robot>())
            {
                DoSingleTurn(robot);
                yield return new WaitForSeconds(.1f); // ?change to a regular yield return
            }
            //?WaitForSeconds here?

            CompletedRounds++;
            OnRoundCompleted();

            CheckSpecialSquares();
        }
        //Not InPlay anymore

        ClearMoveQueue();
        OnGameEnded();
    }

    private void CheckSpecialSquares()
    {
        var positions = theBoard.GetSpecialSquarePositions(SquareDesignator.Goal);
        foreach (var goalPos in positions)
        {
            var square = theBoard.SquareAt(goalPos);
            if (square != null && square.ThingOnMe != null)
            {
                if (square.ThingOnMe is Block && ((Block)square.ThingOnMe).IsGameBlock)
                {
                    Won = true;
                    InPlay = false;
                }
            }
        }
    }

    private void DoSingleTurn(Character character)
    {
        var surroundings = theBoard.GetSurroundings(character);
        var desiredMove = character.GetDesiredMove(surroundings);
        DoSingleTurn(character, desiredMove);
    }

    private void DoSingleTurn(Character character, DesiredMove desiredMove)
    {
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
