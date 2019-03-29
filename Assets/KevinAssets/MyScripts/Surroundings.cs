using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Mainly used by a robot when deciding what move to make(attempt) e.g. if something in front, rotate right
// local to the character; positive Y is forward facing
public interface ISurroundings
{
    ISquare SquareAt(int x, int y);
}

public class Surroundings : ISurroundings
{
    public ISquare SquareAt(int x, int y)
    {
        throw new System.NotImplementedException();
    }
}
