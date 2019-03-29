using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConvert
{
    public static Vector3 Vector3From(Pos pos)
    {
        return new Vector3(pos.X - (BoardCore.MAX_X / 2), 0, pos.Y - (BoardCore.MAX_Y / 2));
    }

    public static Quaternion QuaternionFrom(Facing facing)
    {
        switch(facing)
        {
            case Facing.North:
                return Quaternion.LookRotation(Vector3.forward);
            case Facing.West:
                return Quaternion.LookRotation(Vector3.left);
            case Facing.East:
                return Quaternion.LookRotation(Vector3.right);
            case Facing.South:
                return Quaternion.LookRotation(Vector3.back);
            default:
                return Quaternion.identity;
        }
    }
}

public class GameEngine : MonoBehaviour
{
    public Transform playerPrefab;
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
        InstantiateGameObjectsFromThingsOnBoard(board);

        boardEngine = new BoardEngine(board);
        boardEngine.ThingMoved += ThingMovedHandler;

        StartCoroutine(boardEngine.Run());
    }

    void InstantiateGameObjectsFromThingsOnBoard(IBoard board)
    {
        foreach(var posThing in board.GetAllPositionedThings())
        {
            var thing = posThing.Thing;
            var posFace = posThing.PosFace;
            Transform transform = null;
            if (thing is Player)
            {
                transform = Instantiate(playerPrefab, GameConvert.Vector3From(posFace), GameConvert.QuaternionFrom(posFace.Facing));

            }
            else if (thing is Robot)
            {
                transform = Instantiate(playerPrefab, GameConvert.Vector3From(posFace), GameConvert.QuaternionFrom(posFace.Facing));
            }
            if (transform != null)
            {
                thingTransforms[thing.IdOnBoard] = transform;
            }
        }
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
