using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform robotPrefab;
    public Transform blockPrefab;

    private BoardEngine boardEngine;
    private Dictionary<int, Transform> thingTransforms = new Dictionary<int, Transform>();

    Transform GetThingTransform(int id)
    {
        if (thingTransforms.ContainsKey(id))
            return thingTransforms[id];
        else
            return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        var board = new Board();
        board.SetupForTesting();
        InstantiateGameObjectsFromThings(board);

        boardEngine = new BoardEngine(board);
        boardEngine.ThingMoved += ThingMovedHandler;

        StartCoroutine(boardEngine.Run());
    }

    void InstantiateGameObjectsFromThings(IBoard board)
    {
        foreach(var posThing in board.GetAllPositionedThings())
        {
            var thing = posThing.Thing;
            var posFace = posThing.PosFace;
            Transform transform = null;
            if (thing is Player)
            {
                transform = Instantiate(playerPrefab, posFace);
            }
            else if (thing is Robot)
            {
                transform = Instantiate(robotPrefab, posFace);
            }
            else if (thing is Block)
            {
                transform = Instantiate(blockPrefab, posFace);
            }
            if (transform != null)
            {
                thingTransforms[thing.IdOnBoard] = transform;
            }
        }
    }
    Transform Instantiate(Transform transform, PosFace posFace)
    {
        return Instantiate(transform, GameConvert.Vector3From(posFace), GameConvert.QuaternionFrom(posFace.Facing));
    }

    void ThingMovedHandler(object sender, BoardEventArgs args)
    {
        var pf = args.NewPosFace;
        var msg = string.Format("{0}: {1},{2} {3}", args.ThingId, pf.X, pf.Y, pf.Facing);
        print(msg);

        var transform = GetThingTransform(args.ThingId);
        if (transform != null)
        {
            transform.position = GameConvert.Vector3From(pf);
            transform.rotation = GameConvert.QuaternionFrom(pf.Facing);
        }
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
