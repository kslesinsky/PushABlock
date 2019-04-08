using System.Collections;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public GameVisuals GV;

    private BoardEngine boardEngine;

    // Start is called before the first frame update
    void Start()
    {
        boardEngine = new BoardEngine();
        boardEngine.ThingMoved += ThingMovedHandler;
        boardEngine.RoundCompleted += RoundCompletedHandler;
        boardEngine.GameEnded += GameEndedHandler;
        boardEngine.DebugMessageEvent += DebugMessageHandler;

        LoadTestLevelAndStart();
    }

    void LoadTestLevelAndStart(int level = 0)
    {
        var board = new Board();
        BoardTest.SetupForTesting(board, level);
        boardEngine.UseBoard(board);
        StartGame();
    }

    void StartGame()
    {
        // add Things & designators to the board
        var board = boardEngine.TheBoard;
        GV.InstantiateGameObjectsFromThings(board);
        GV.InstantiateOtherGameObjects(board);
        GV.SetRoundsCompleted(0);
        StartCoroutine(boardEngine.Run());
    }

    // --- Event Handlers ---

    void ThingMovedHandler(object sender, BoardEventArgs args)
    {
        GV.UpdateThing(args.ThingId, args.NewPosFace);
    }

    void RoundCompletedHandler(object sender, BoardEventArgs args)
    {
        GV.SetRoundsCompleted(args.RoundsCompleted);
    }

    void GameEndedHandler(object sender, BoardEventArgs args)
    {
        //TODO: wait for move-animations to complete
        StartCoroutine(WaitForRestart());
    }

    IEnumerator WaitForRestart()
    {
        GV.SetCenterText("Press SPACE to continue.");
        bool waiting = true;
        while (waiting)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                waiting = false;
            else
                yield return null;
        }
        GV.ClearTheBoard();
        boardEngine.RemoveTheBoardAndReset();
        LoadTestLevelAndStart(level: 1);
        // Any issues here, like having a loop in the call stack?
    }

    void DebugMessageHandler(object sender, BoardEventArgs args)
    {
        Debug.Log(args.Message);
    }

    // --- Update is called once per frame ---

    void Update()
    {
        if (boardEngine != null && boardEngine.InPlay)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                boardEngine.AddToMoveQueue(MoveType.Forward);
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                boardEngine.AddToMoveQueue(MoveType.Reverse);
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                boardEngine.AddToMoveQueue(MoveType.RotLeft);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                boardEngine.AddToMoveQueue(MoveType.RotRight);
        }
    }
}
