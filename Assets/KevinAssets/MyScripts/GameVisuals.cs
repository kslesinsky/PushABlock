using System.Collections.Generic;
using UnityEngine;

public class GameVisuals : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform robotPrefab;
    public Transform blockPrefab;
    public Transform gameblockPrefab;
    public Transform goalSquarePrefab;

    private Dictionary<int, Transform> thingTransforms = new Dictionary<int, Transform>();

    Transform GetThingTransform(int id)
    {
        if (thingTransforms.ContainsKey(id))
            return thingTransforms[id];
        else
            return null;
    }

    public void InstantiateGameObjectsFromThings(IBoard board)
    {
        foreach (var posThing in board.GetAllPositionedThings())
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
                if (((Block)thing).IsGameBlock)
                    transform = Instantiate(gameblockPrefab, posFace);
                else
                    transform = Instantiate(blockPrefab, posFace);
            }
            if (transform != null)
            {
                thingTransforms[thing.IdOnBoard] = transform;
            }
        }
    }

    public void InstantiateOtherGameObjects(IBoard board)
    {
        var ssPositions = board.GetSpecialSquarePositions(SquareType.Goal);
        foreach (var pos in ssPositions)
        {
            Instantiate(goalSquarePrefab, pos);
        }
    }

    Transform Instantiate(Transform transform, Pos pos)
    {
        return Instantiate(transform, new PosFace(pos));
    }
    Transform Instantiate(Transform transform, PosFace posFace)
    {
        return Instantiate(transform, GameConvert.Vector3From(posFace), GameConvert.QuaternionFrom(posFace.Facing));
    }

    // --- Event Handlers ---

    public void SetEventHandlers(BoardEngine boardEngine)
    {
        boardEngine.ThingMoved += ThingMovedHandler;
        boardEngine.DebugMessageEvent += DebugMessageHandler;
    }

    void ThingMovedHandler(object sender, BoardEventArgs args)
    {
        var pf = args.NewPosFace;
        //var msg = string.Format("{0}: {1},{2} {3}", args.ThingId, pf.X, pf.Y, pf.Facing);
        //print(msg);

        var transform = GetThingTransform(args.ThingId);
        if (transform != null)
        {
            transform.position = GameConvert.Vector3From(pf);
            transform.rotation = GameConvert.QuaternionFrom(pf.Facing);
        }
    }

    void DebugMessageHandler(object sender, BoardEventArgs args)
    {
        Debug.Log(args.Message);
    }

}
