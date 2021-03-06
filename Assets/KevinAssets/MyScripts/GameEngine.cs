﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        var levelInfo = LevelInfo.CurrentLevelInfo;
        LoadLevelAndStart(levelInfo);
    }

    void LoadLevelAndStart(LevelInfo levelInfo)
    {
        var board = new Board();
        BoardLoader.LoadFromLevelInfo(board, levelInfo);
        boardEngine.UseBoard(board);

        //TODO: display level# onscreen.  ?store in BoardEngine or Board?

        StartGame();
    }

    void StartGame()
    {
        // add Things & designators to the board
        var board = boardEngine.TheBoard;
        GV.InstantiateGameObjects(board);
        GV.SetRoundsCompleted(0);

        StartCoroutine(boardEngine.Run());
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

            if (Input.GetKeyDown(KeyCode.Q))
                boardEngine.Quit = true;
        }
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
        string message = (args.Won ? "Congratulations! " : "") + "Press SPACE to continue.";
        GV.SetCenterText(message);
        StartCoroutine(WaitForRestart());
    }

    // ? Does the coroutine start in the follow frame ?

    IEnumerator WaitForRestart()
    {
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
        // Is the above necessary, or when you load another scene does everything get destroyed and GCed?

        SceneManager.LoadScene("MainMenu");
    }

    void DebugMessageHandler(object sender, BoardEventArgs args)
    {
        Debug.Log(args.Message);
    }
}
