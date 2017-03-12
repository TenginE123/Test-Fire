using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IScrollable {
   
    string  type;       //Obstacle Type
    bool    moves;      
    int     speed;      //Speed
    [SerializeField]
    int     dir;        //Instance Direction
    //int width = 40;
    int     max;  //maximum movement value, can be adjusted to match a width value for smaller H-Obs
    //int oldDir = 1;
    Vector2 startPos;

    //speed multiplier variable (pass from spawn manager)
  
    private void OnEnable()
    {
        transform.rotation = new Quaternion(0, 0, 0, 0);
        //call line drawing function here?
        //pass the obstacle type and its values
        speed = GameManager.Instance.ScrollSpeed;
        transform.position = ResetPos(transform.position); //reset the position
        EventManager.ScrollObjects += Scroll;
        EventManager.ChangeDirection += ChangeDir;
        EventManager.Restart += Restart;
    }

    private void OnDisable()
    {
        EventManager.ScrollObjects -= Scroll;
        EventManager.ChangeDirection -= ChangeDir;
        EventManager.Restart -= Restart;
        transform.position = ResetPos(transform.position); //reset the position //Possibly causing visual glitch
        transform.rotation = new Quaternion(0, 0, 0, 0);
        //set active to false
    }

    public void Scroll() //Scrolling can likely be applied to the parent object
    {
        //disable obstacle once it reaches the bottom of the game space
        if (transform.position.y < -400)
        {
            gameObject.SetActive(false);
        }

        speed = GameManager.Instance.ScrollSpeed; //allow for gamespeed updates at runtime

        //Check incoming obstacle info 
        if (moves)          //does it move?
        {
            switch (type)   //what type is it?
            {
                case "RectPaddle":
                    //Position Check
                    float t = transform.position.x; //Get the x posiition of the object
                    //added "- startPos.x" to enforce movement boundary based on spawn position
                    if(Mathf.Abs(t - startPos.x) > max)      //if the object is outside its max move value 
                    {
                        if (Mathf.Sign(dir) == Mathf.Sign(t - startPos.x))
                        {
                            ChangeDir();
                            AudioManager.Instance.PlayObsBounce();
                        }
                    }
                    //END Position Check

                    //Position Assignment
                    Vector2 _newPosition = transform.position;
                    _newPosition.x += dir * (speed * Time.deltaTime);
                    transform.position = _newPosition;
                    //End Position Assignment
                    break;
                case "CurvePaddle":
                    transform.Rotate(Vector3.forward * (dir * (0.66f * speed * Time.deltaTime))); //rotate
                    break;
                case "RotTunnel":
                    transform.Rotate(Vector3.forward * (dir * (0.5f * speed * Time.deltaTime)));
                    break;
            }
        }
    }

    void Restart()
    {
        if(moves) moves = false;
        gameObject.SetActive(false);
    }

    void ChangeDir() //removed "ref int dir" to allow for subscription to change direction event
    {
        dir = -dir;
    }

    Vector2 ResetPos(Vector2 pos)
    {
        pos = new Vector2(0, 400);
        return pos;
    }

    //Obstacle building function
    public void BuildObject(List<Vector2> points, int initDir, int maxPos, ObstacleType obst) //add width value?
    {
        //receive and construct points arrays for both linerenderer and polygon collider (if necssary)
        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        PolygonCollider2D pc = gameObject.GetComponent<PolygonCollider2D>();

        pc.points = points.ToArray();   //assign points to collider

        lr.numPositions = points.Count; //set the number of points in the linerenderer

        for (int i = 0; i < lr.numPositions; i++)
        {
            lr.SetPosition(i, (Vector3)points[i]);  //set linerenderer point
        }

        //Set direction, obstacle type and moving values
        dir = obst.dir;                 //Initial Direction
        type = obst.name;               //Obstacle Type
        moves = obst.moves;             //Is a moving obstacle
        transform.position = obst.pos;  //Initial Position
        transform.rotation = obst.rot;  //Initial Rotation
        
        startPos = obst.pos;            //Start Position
        max = maxPos;                   //Set Maximum position value
        //END Set direction, obstacle type and moving values
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject.Find("Missile").GetComponent<Missile>().Explode();
        }
    }


    //Create struct with all constructor variables?
    //Include Reset function?
    public struct ObstacleVars
    {
        string obsType;
        bool moves;
        int moveDir;
        int speed;
        Quaternion startRot;

        void Reset()
        {

        }
    }
}
