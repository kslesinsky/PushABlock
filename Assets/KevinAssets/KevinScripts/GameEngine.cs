using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    private BoardEngine boardEngine;

    // Start is called before the first frame update
    void Start()
    {
        var board = new Board();
        board.SetupForTesting();
        boardEngine = new BoardEngine(board);
        //TODO: instantiate GameObjects
        boardEngine.ThingMoved += ThingMovedHandler;

        StartCoroutine(boardEngine.Run());
    }

    void ThingMovedHandler(object sender, BoardEventArgs args)
    {
        var pf = args.NewPosFace;
        var msg = string.Format("{0}: {1},{2} {3}", args.ThingId, pf.X, pf.Y, pf.Facing);
        print(msg);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            boardEngine.AddToMoveQueue(MoveType.Forward);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            boardEngine.AddToMoveQueue(MoveType.Reverse);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            boardEngine.AddToMoveQueue(MoveType.RotLeft);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            boardEngine.AddToMoveQueue(MoveType.RotRight);
        }
    }
}
