using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpaceFill{
    None, Impassable, Crowd
}

public class GridPoint{
    public Vector3 Position;
    public GridPoint[] Neighbors;
    public SpaceFill Fill;

    public GridPoint(Vector3 position, SpaceFill fill){
        Position = position;
        Neighbors = new GridPoint[4];
        Fill = fill;
    }

    public GridPoint(Vector3 position){
        Position = position;
        Neighbors = new GridPoint[4];
        Fill = SpaceFill.None;
    }
}
