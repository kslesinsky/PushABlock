using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public GameVisuals GV;

    private BoardEngine boardEngine;

    // Start is called before the first frame update
    void Start()
    {
        var board = new Board();
        board.SetupForTesting();
        GV.InstantiateGameObjectsFromThings(board);
        GV.InstantiateOtherGameObjects(board);

        boardEngine = new BoardEngine(board);
        GV.SetEventHandlers(boardEngine);

        StartCoroutine(boardEngine.Run());
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
