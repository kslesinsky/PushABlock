using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameVisuals : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform robotPrefab;
    public Transform blockPrefab;
    public Transform gameblockPrefab;
    public Transform goalSquarePrefab;
    public Text statusText;
    public Text centerText;

    private Dictionary<int, Transform> thingTransforms = new Dictionary<int, Transform>();
    private List<Transform> otherTransforms = new List<Transform>();

    Transform GetThingTransform(int id)
    {
        if (thingTransforms.ContainsKey(id))
            return thingTransforms[id];
        else
            return null;
    }

    public void UpdateThing(int thingId, PosFace newPosFace)
    {
        var transform = GetThingTransform(thingId);
        if (transform != null)
        {
            transform.position = GameConvert.Vector3From(newPosFace);
            transform.rotation = GameConvert.QuaternionFrom(newPosFace.Facing);
        }
    }

    // --- Instantiation of GameObjects ---

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
        var ssPositions = board.GetSpecialSquarePositions(SquareDesignator.Goal);
        foreach (var pos in ssPositions)
        {
            var transform = Instantiate(goalSquarePrefab, pos);
            otherTransforms.Add(transform);
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

    public void ClearTheBoard()
    {
        foreach(var transform in thingTransforms.Values)
        {
            Destroy(transform.gameObject);
        }
        thingTransforms.Clear();

        foreach (var transform in otherTransforms)
        {
            Destroy(transform.gameObject);
        }
        otherTransforms.Clear();
        SetRoundsCompleted(0);
        SetCenterText(null);
    }

    public void SetRoundsCompleted(int numCompleted)
    {
        statusText.text = "Turns Completed: " + numCompleted;
    }

    public void SetCenterText(string message)
    {
        centerText.text = message;
    }
}
