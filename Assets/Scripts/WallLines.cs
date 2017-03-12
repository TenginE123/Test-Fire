using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLines : MonoBehaviour {

    public float width; //should this be a globally set value?
    float wallPosL, wallPosR;
    int lineTop = 0, lineBottom = -800; //top and bottom values, in local space coordinates

    static List<Vector2> pointsL = new List<Vector2>();
    static List<Vector2> pointsR = new List<Vector2>();

    LineRenderer lrL;
    LineRenderer lrR;

    EdgeCollider2D ecL;
    EdgeCollider2D ecR;

    int speed;
    [SerializeField]
    bool transitionIn;
    [SerializeField]
    bool transitionOut;

    private void OnEnable()
    {
        //Use this space for game start?
        //subscribe to game mode functions
    }

    void Start () {
        width = GameManager.Instance.Width;
        //create wallposition values based on a given width
        wallPosL = width / 2 * -1;
        wallPosR = width / 2;
        //find the Left and Right Line Renderer Components
        lrL = GameObject.Find("Left Wall").GetComponent<LineRenderer>();
        lrR = GameObject.Find("Right Wall").GetComponent<LineRenderer>();
        ecL = GameObject.Find("Left Wall").GetComponent<EdgeCollider2D>();
        ecR = GameObject.Find("Right Wall").GetComponent<EdgeCollider2D>();
        //Initialise the two wall lines for use
        

        //transitionIn = true;
	}
	
    //replace update function with "Game running" event subscription function?
	void Update () {
        speed = GameManager.Instance.ScrollSpeed; //allow for gamespeed updates at runtime
        var state = GameManager.Instance.Modes;
        var mode = GameManager.Instance.GameMode;
        switch (mode)
        {
            case GameManager.GameState.Start:
                //run enter corridor here?*
                if (!transitionIn)
                {
                    Initialise(ref pointsL, ref lrL, ref ecL, wallPosL); //init Left
                    Initialise(ref pointsR, ref lrR, ref ecR, wallPosR); //init Right
                    transitionIn = true;
                }
                break;
            case GameManager.GameState.Normal:
                //*or here? AND here?
                if (transitionOut)
                {
                    transitionOut = false;
                    Initialise(ref pointsL, ref lrL, ref ecL, wallPosL); //init Left
                    Initialise(ref pointsR, ref lrR, ref ecR, wallPosR); //init Right
                    transitionIn = true;
                }

                UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
                UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
                break;
            case GameManager.GameState.Defense:
                //run exit corridor here
                //if(!transitionOut) set transitionOut to true
                if (!transitionOut)
                {
                    transitionOut = true;
                }
                UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
                UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
                break;
            case GameManager.GameState.GameOver:
                break;
            default:
                Debug.Log("Error: Unknowm Game State.");
                break;
        }

        /*if(state == GameManager.Mode.Game)
        {
            UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
            UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
        }*/

        //Update the wall values
        //UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
        //UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
        //width old? For switching to defense mode, moving the lines out of the screen
	}

    //Function to receive the incoming point arrays for the line renderer
    public static void AddPoints(List<Vector2> points)
    {
        pointsL.InsertRange(1, points); //addPoints Left
        pointsR.InsertRange(1, points); //addPoints Right
    } //this method suggests the need to perform all point calculations BEFORE they reach this script

    void Initialise(ref List<Vector2> line, ref LineRenderer lr, ref EdgeCollider2D ec, float wallPos)
    {
        //Point List initialisation
        line = new List<Vector2>();                             //create a new List of type Vector 2
        //line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineTop));                //set the first value to match the top line renderer coordinate
        line.Add(new Vector2(wallPos, lineTop));
        line.Add(new Vector2(wallPos, lineTop));             //set the bottom value to match the bottom coordinates
        line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineTop));
        line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineBottom));
        //LineRenderer initialisation
        lr.numPositions = line.Count;

        lr.SetPosition(0, line[0]);     //set top point coordinate
        lr.SetPosition(1, line[1]);
        lr.SetPosition(2, line[2]);     //set bottom point coordinate
        lr.SetPosition(3, line[3]);

        ec.points = line.ToArray();

        //transitionIn = true;

        //while loop
        //while there are no positions below the bottom line value, run this code (Update wall copy)
    }

    void UpdateWall(ref List<Vector2> line, ref LineRenderer lr, ref EdgeCollider2D ec, float wallPos)
    {
        //int minCount;   //minimum point count to start moving points downward
        //int p;          //starting i value
        //int q;          //loop end modifier

        //if (transitionIn) //enter corridor
        //{
        //    //set count and i values
        //    minCount = 2;
        //    p = 2;
        //    q = 0;
        //}
        //else if (transitionOut) //exitcorridor
        //{
        //    minCount = 2;
        //    p = 0;
        //    q = 2;
        //}
        //else
        //{
        //    minCount = 4;
        //    p = 2;
        //    q = 2;
        //}

        if (line.Count > 2) //only start scrolling positions if points have been added
        {
            lr.numPositions = line.Count;
            //ec.points = line.ToArray();
            float sign = Mathf.Sign(wallPos);

            for (int i = 2; i < lr.numPositions - 2; i++)
            {
                //move the point down, checking the sign of the wall position to set the X component correctly
                line[i] = new Vector2(Mathf.Sign(line[i].x) == sign ? line[i].x : line[i].x * sign, line[i].y - (speed * Time.deltaTime));
                ec.points[i] = line[i];

                if (line[i].y <= lineBottom) //remove points once they reach the bottom of the screen
                {
                    //if either transition bools are true, they need to be disabled once the first point 
                    //reaches the bottom of the screen
                        //if transitionin is true, set the new width value to 97
                        //if transitionout is true, set the new width value to 200
                    line.Remove(line[i]);
                    continue;
                }

                lr.SetPosition(i, (Vector3)line[i]); //set the updated position
                
            }

            lr.SetPosition(lr.numPositions - 2, new Vector2(wallPos, lineBottom));
            lr.SetPosition(lr.numPositions - 1, new Vector2(Mathf.Sign(wallPos) * 200, lineBottom));
        }

        /*if (line.Count > 4) //only start scrolling positions if points have been added
        {
            lr.numPositions = line.Count;
            float sign = Mathf.Sign(wallPos);

            for (int i = 2; i < lr.numPositions - 2; i++)
            {
                //move the point down, checking the sign of the wall position to set the X component correctly
                line[i] = new Vector2(Mathf.Sign(line[i].x) == sign ? line[i].x : line[i].x * sign, line[i].y - (speed * Time.deltaTime));

                if (line[i].y <= lineBottom) //remove points once they reach the bottom of the screen
                {
                    line.Remove(line[i]);
                    continue;
                }

                lr.SetPosition(i, (Vector3)line[i]); //set the updated position
                ec.points = line.ToArray();
            }

            lr.SetPosition(lr.numPositions - 2, new Vector2(wallPos, lineBottom));
            lr.SetPosition(lr.numPositions - 1, new Vector2(Mathf.Sign(wallPos) * 200, lineBottom));
        }*/
    }

    void EnterCorridor()
    {
        //add coordinates for "entrance"
        //pass to wall update method to scroll past bottom of screen
        //set mode to normal?
    }

    void ExitCorridor()
    {
        //move top points down. Keep top line point out of view?
    }
}
