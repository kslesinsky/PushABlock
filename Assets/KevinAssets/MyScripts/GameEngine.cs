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
        boardEngine.GameEnded += GameEndedHandler;
        GV.SetEventHandlers(boardEngine);

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
        StartCoroutine(boardEngine.Run());
    }

    void GameEndedHandler(object sender, BoardEventArgs args)
    {
        //TODO: wait for move-animations to complete
        StartCoroutine(WaitForRestart());
    }

    IEnumerator WaitForRestart()
    {
        print("Press space to play again.");
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
    }

    // Update is called once per frame
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
