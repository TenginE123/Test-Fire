using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallLinesEdit : MonoBehaviour {

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

    public string lastState;


    void Awake()
    {
        //width = GameManager.Instance.Width;
        width = 400;
        //create wallposition values based on a given width
        wallPosL = width / 2 * -1;
        wallPosR = width / 2;
        //find the Left and Right Line Renderer Components
        lrL = GameObject.Find("Left Wall").GetComponent<LineRenderer>();
        lrR = GameObject.Find("Right Wall").GetComponent<LineRenderer>();
        ecL = GameObject.Find("Left Wall").GetComponent<EdgeCollider2D>();
        ecR = GameObject.Find("Right Wall").GetComponent<EdgeCollider2D>();
        //Initialise the two wall lines for use

        EventManager.Restart += Restart;
        //transitionIn = true;
    }

    private void OnEnable()
    {
        //Use this space for game start?
        //subscribe to game mode functions

        width = 400;
        transitionIn = false;
        transitionOut = false;

        ResetLines(pointsL, lrL, ecL, wallPosL);
        ResetLines(pointsR, lrR, ecR, wallPosR);
        lastState = null;
    }

    private void OnDisable()
    {

    }

	
    //replace update function with "Game running" event subscription function?
	void Update () {
        wallPosL = width / 2 * -1;
        wallPosR = width / 2;

        speed = GameManager.Instance.ScrollSpeed;   //allow for gamespeed updates at runtime

        var state = GameManager.Instance.Modes;
        var mode = GameManager.Instance.GameMode;

        if(state == GameManager.Mode.Game)          //if in-game
        {
            switch (mode)
            {
                case GameManager.GameState.Start:
                    //Reset the linerenderers and all necessary variables 
                    if (lastState != null && lastState != "Start") //if the last state is null, the game is being run for the first time, so nothing need be reset
                    {
                        width = 400;
                        transitionIn = false;
                        transitionOut = false;
                        
                        ResetLines(pointsL, lrL, ecL, wallPosL);
                        ResetLines(pointsR, lrR, ecR, wallPosR);
                        lastState = null;
                    }
                    //END Reset the linerenderers and all necessary variables                
                    break;
                case GameManager.GameState.Normal:
                    //*or here? AND here?
                    if (mode.ToString() != lastState || lastState == null)   //Check to see if the state has only just changed
                    {
                        if (!transitionIn)             //if it HAS, and the transitionOut bool is false, the transition needs to happen
                        {
                            transitionIn = true;
                            Initialise(ref pointsL, ref lrL, ref ecL, wallPosL); //init Left
                            Initialise(ref pointsR, ref lrR, ref ecR, wallPosR); //init Right
                        }
                    }
                    //if (transitionOut) 
                    //{
                    //    transitionOut = false;
                    //    transitionIn = true;
                    //    Initialise(ref pointsL, ref lrL, ref ecL, wallPosL); //init Left
                    //    Initialise(ref pointsR, ref lrR, ref ecR, wallPosR); //init Right
                    //}

                    UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
                    UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
                    break;
                case GameManager.GameState.Defense:
                    if (mode.ToString() != lastState)   //Check to see if the state has only just changed
                    {
                        if (!transitionOut)             //if it HAS, and the transitionOut bool is false, the transition needs to happen
                        {
                            transitionOut = true;
                            Initialise(ref pointsL, ref lrL, ref ecL, wallPosL); //init Left
                            Initialise(ref pointsR, ref lrR, ref ecR, wallPosR); //init Right
                        }
                    }
                    //run exit corridor here
                    //if(!transitionOut) set transitionOut to true
                    //if (!transitionOut)
                    //{
                    //    transitionOut = true;
                    //    Initialise(ref pointsL, ref lrL, ref ecL, wallPosL); //init Left
                    //    Initialise(ref pointsR, ref lrR, ref ecR, wallPosR); //init Right
                    //}

                    UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
                    UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
                    break;
                case GameManager.GameState.GameOver:
                    UpdateWall(ref pointsL, ref lrL, ref ecL, wallPosL); //update Left
                    UpdateWall(ref pointsR, ref lrR, ref ecR, wallPosR); //update Right 
                    break;
                default:
                    Debug.Log("Error: Unknowm Game State.");
                    break;
            }
        }

        //Last State check. Used to check if the state has changed and enable transition activity if their HAS been a change
        lastState = mode.ToString(); //Set the last state value for each update call

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
        //line.Add(new Vector2(wallPos, lineTop));                        //set first point to wallpos and walltop
        //line.Add(new Vector2(wallPos, lineTop));                        //set second point to the same
        //line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineTop));      //set third to draw horizontal line off of screen
        //line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineBottom));   //set fourth to bottom of wall point (off screen)
        //LineRenderer initialisation
        if (transitionIn)
        {
            line.Add(new Vector2(Mathf.Sign(wallPos) * 48.5f, lineTop));                        //set first point to wallpos and walltop
            line.Add(new Vector2(Mathf.Sign(wallPos) * 48.5f, lineTop));                        //set second point to the same
            line.Add(new Vector2(wallPos, lineTop));      //set third to draw horizontal line off of screen
            line.Add(new Vector2(wallPos, lineBottom));   //set fourth to bottom of wall point (off screen)
        }
        if (transitionOut)
        {
            line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineTop));                        //set first point to wallpos and walltop
            line.Add(new Vector2(Mathf.Sign(wallPos) * 200, lineTop));                        //set second point to the same
            line.Add(new Vector2(Mathf.Sign(wallPos) * 48.5f, lineTop));      //set third to draw horizontal line off of screen
            line.Add(new Vector2(Mathf.Sign(wallPos) * 48.5f, lineBottom));   //set fourth to bottom of wall point (off screen)
        }

        lr.numPositions = line.Count;
        //assign line values to linerenderer positions
        lr.SetPosition(0, line[0]);     
        lr.SetPosition(1, line[1]);
        lr.SetPosition(2, line[2]);     
        //lr.SetPosition(3, line[3]);
        lr.SetPosition(lr.numPositions-1, line[3]);

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
            
            float sign = Mathf.Sign(wallPos);

            for (int i = 1; i < lr.numPositions - 1; i++) //loop through the list of positions, ignoring the first and last
            {
                //move the point down, checking the sign of the wall position to set the X component correctly
                line[i] = new Vector2(Mathf.Sign(line[i].x) == sign ? line[i].x : line[i].x * sign, line[i].y - (speed * Time.deltaTime));

                if (line[i].y <= lineBottom) //remove points once they reach the bottom of the screen
                {
                    //if either transition bools are true, they need to be disabled once the first point 
                    //reaches the bottom of the screen
                    if (transitionIn || transitionOut)
                    {
                        //if transitionin is true, set the new width value to 97
                        if (transitionIn)
                        {
                            width = 97;
                            transitionIn = false;
                        }
                        //if transitionout is true, set the new width value to 200
                        if (transitionOut)
                        {
                            width = 400;
                            transitionOut = false;
                        }
                    }

                    wallPos = sign * width /2; //update wallPos value
                    //BEFORE removing the offending point, ensure the bottom coordinate values are up to date
                    //this eliminates the flickering caused by removing the point before updating the new bottom coordinate values
                    //Added top coordinates
                    line[0] = new Vector2(wallPos, lineTop);                                //set top points
                    lr.SetPosition(0, new Vector2(wallPos, lineTop));                       

                    //Changes index to set the coordinates for the point about to be removed
                    //further ensuring that there will be nodiscrepancies
                    line[line.Count - 2] = new Vector2(wallPos, lineBottom);                //set bottom points
                    lr.SetPosition(lr.numPositions - 2, new Vector2(wallPos, lineBottom));  

                    line.Remove(line[i]);           //remove the point
                    lr.numPositions = line.Count;   //update the position count
                    //continue;
                    break;      
                    //using break seems to fix glitches
                    //this is likely because, as the offending point is at the Bottom of the screen, it is also the last item in
                    //the position list, making a 'continue' statement unnecessary
                }
                else
                {
                    lr.SetPosition(i, (Vector3)line[i]); //set the updated position
                }
                
            }

            if(!transitionIn && !transitionOut)
            {
                line[0] = new Vector2(wallPos, lineTop);
                
                lr.SetPosition(0, new Vector2(wallPos, lineTop));                       //set top points
                
            }
            line[line.Count - 1] = new Vector2(wallPos, lineBottom);
            lr.SetPosition(lr.numPositions - 1, new Vector2(wallPos, lineBottom));  //set bottom points
            //line[0] = new Vector2(wallPos, lineTop);
            //line[line.Count - 1] = new Vector2(wallPos, lineBottom);
            //lr.SetPosition(0, new Vector2(wallPos, lineTop));                       //set top points
            //lr.SetPosition(lr.numPositions - 1, new Vector2(wallPos, lineBottom));  //set bottom points

            //lr.SetPosition(lr.numPositions - 1, new Vector2(Mathf.Sign(wallPos) * 200, lineBottom));
            ec.points = line.ToArray();
            //ec.points.SetValue(new Vector2(wallPos, lineBottom), ec.pointCount - 1);
            //ec.points[ec.pointCount - 1] = new Vector2(wallPos, lineBottom);
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

    void ResetLines(List<Vector2> p, LineRenderer lr, EdgeCollider2D ec, float wallPos)
    {
        if(p != null)
        {
            p.Clear();
            p.Add(new Vector2(Mathf.Sign(wallPos) * 200, 0));
            p.Add(new Vector2(Mathf.Sign(wallPos) * 200, -800));
        }
        if(lr.numPositions > 1)
        {
            lr.numPositions = 2;
            lr.SetPosition(0, p[0]);
            lr.SetPosition(1, p[1]);
        }
        if(ec.pointCount > 2)
        {
            //ec.points = new Vector2[1];
            //ec.points[0].Set(0, 0);
            ec.Reset();
        }
    }

    void Restart()
    {
        width = 400;
        transitionIn = false;
        transitionOut = false;

        ResetLines(pointsL, lrL, ecL, wallPosL);
        ResetLines(pointsR, lrR, ecR, wallPosR);
        lastState = null;
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
