using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleType {

    //variable for grouping? e.g. ID numer
    //individual movedir variable? (for gates)
    public string name;     //Obstacle Type
    public bool moves;      //Does the object move?
    public int dir;
    public Vector2 pos;     //Starting Position
    public Quaternion rot;  //Starting Rotation

    //constructor
    public ObstacleType(string typeName, bool isMoving, Vector2 startPos, Quaternion startRot)
    {
        name = typeName;
        moves = isMoving;
        pos = startPos;
        rot = startRot;
    }
    //initial direction overload
    public ObstacleType(string typeName, bool isMoving, int initDir, Vector2 startPos, Quaternion startRot)
    {
        name = typeName;
        moves = isMoving;
        dir = initDir;
        pos = startPos;
        rot = startRot;
    }
}
