using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    [SerializeField]
    private int segments = 8;   //detail setting. higher value = smoother curves //could create a quality setting that reduces this number?
    [SerializeField]
    private int radius = 100;     //sets overall size of curve
    [SerializeField]
    //private int hDist = 48; //distance from centre of screen to wall blocks //needs changing to universal width value
    private int hDist;

    private float timeToNextSpawn = 1.5f;
    string lastObsType; //the last obstacle that spawned

    public List<KeyValuePair<string, int>> chance = new List<KeyValuePair<string, int>>(); //CREATE THE <new> OBJECT FIRST!!

    public int numObstacles;
    public int numProjectiles;

    void Awake()
    {
        hDist = GameManager.Instance.Width / 2;

        EventManager.Restart += Restart;

        //move to initialise function
        chance = InitObstacleList(chance);
    }

    private void Start()
    {
        //InitObstacleList();

        //invoke first obstacle spawn here*
        numObstacles = 10;
        numProjectiles = 10;
    }

    private void Update()
    {
        //invoke next obstacle spawn when current actually spawns
        if (GameManager.Instance.Modes == GameManager.Mode.Game)
        {
            if (GameManager.Instance.GameMode == GameManager.GameState.Start)
            {
                numObstacles = 10;
                numProjectiles = 10;
            }
            else if (GameManager.Instance.GameMode == GameManager.GameState.Normal)
            {
                //*OR invoke first spawn here, after mode has been set to normal

                if (!IsInvoking("SpawnObject"))
                {
                    if (numObstacles > 0 && !IsInvoking("SetGameDefense"))
                    {
                        Invoke("SpawnObject", timeToNextSpawn); //timeToNextSpawn variable for invoke?
                        //Decrement action needs to be triggered when an obstacle reaches the bottom of the screen
                        numObstacles--;                      //decrement NumObstacles here?                                        
                                                             //multiply timeToNextSpawn by "Difficulty" multiplier?
                    }
                    else
                    {
                        if (!IsInvoking("SetGameDefense"))
                        {
                            Invoke("SetGameDefense", 3);                            

                            if (IsInvoking("SpawnObject"))
                            {
                                StopCoroutine("SpawnObject");   //Stop spawning obstacles
                                CancelInvoke("SpawnObject");    //Cancel any obstacles that are waiting to be spawned 
                            }

                            if (numObstacles == 0)  //reset numObstaacles variable
                            {
                                numObstacles = 10;
                            }
                        }
                        
                        
                    }
                }
                
            }
            else if (GameManager.Instance.GameMode == GameManager.GameState.Defense)
            {
                //Invoke SpawnProjectile
                if (!IsInvoking("SpawnProjectile"))
                {
                    if(numProjectiles > 0 && !IsInvoking("SetGameNormal"))
                    {
                        Invoke("SpawnProjectile", 1);

                        numProjectiles--;
                    }
                    else
                    {
                        if (!IsInvoking("SetGameNormal"))
                        {
                            Invoke("SetGameNormal", 3);                            

                            if (IsInvoking("SpawnProjectile"))
                            {
                                StopCoroutine("SpawnProjectile");   //Stop spawning obstacles
                                CancelInvoke("SpawnProjectile");    //Cancel any obstacles that are waiting to be spawned 
                            }

                            if (numProjectiles == 0)     //reset numProjectiles value
                            {
                                numProjectiles = 10;
                            }
                        }
                        
                    }
                }
            }
            else
            {
                if(IsInvoking("SetGameNormal") || IsInvoking("SetGameDefense"))
                {

                }
                if (IsInvoking("SpawnObject"))
                {
                    StopCoroutine("SpawnObject");   //Stop spawning obstacles
                    CancelInvoke("SpawnObject");    //Cancel any obstacles that are waiting to be spawned 
                }
                if (IsInvoking("SpawnProjectile"))
                {
                    StopCoroutine("SpawnProjectile");   //Stop spawning obstacles
                    CancelInvoke("SpawnProjectile");    //Cancel any obstacles that are waiting to be spawned 
                }
            }
        }
    }

    //Spawn Missile Function
    private void SpawnProjectile()
    {
        GameObject p = PoolManager.Instance.GetProjectile();
        //p.transform.position = new Vector2(Random.Range(-400, 400), -600);
    }

    private void SpawnObject() //Suscribe to a spawn event?
    {
        string obstacleType = Choose(chance);
        CreateObstacle(obstacleType);                   //Create the picked obstacle
    }

    List<KeyValuePair<string, int>> InitObstacleList(List<KeyValuePair<string, int>> list)
    {
        list.Add(new KeyValuePair<string, int>("Single",        5));
        list.Add(new KeyValuePair<string, int>("Gate",          4));
        list.Add(new KeyValuePair<string, int>("Door",          4));
        list.Add(new KeyValuePair<string, int>("RotateSingle",  3));
        list.Add(new KeyValuePair<string, int>("Rotate2",       3));
        list.Add(new KeyValuePair<string, int>("Rotate3",       1));
        list.Add(new KeyValuePair<string, int>("RotateTunnel",  2));

        return list;
    }

    //Build the chosen obstacle type
    void CreateObstacle(string obsType)
    {
        int maxPos = 0;

        int dir = Random.value > 0.5 ? 1 : -1;

        List<Vector2> points = new List<Vector2>(); //set points

        var subObjList = new List<ObstacleType>();

        //Get a parent object
        GameObject container = PoolManager.Instance.GetContainer(); //Call the getcontainer function

        //Set up wall coordinates and subobject array
        switch (obsType)
        {
            case "Single":
                maxPos = 130;
                
                subObjList.Add(new ObstacleType("RectPaddle", true, dir, new Vector2(0, 400), Quaternion.identity));

                points.Add(new Vector2(hDist + 0.5f , 10.5f));  //y value = (obsHeight / 2) + 1?
                points.Add(new Vector2(160 + 0.5f   , 10.5f));
                points.Add(new Vector2(160 + 0.5f   , -10.5f));
                points.Add(new Vector2(hDist + 0.5f , -10.5f));

                Debug.Log(obsType);
                break;
            case "Gate":
                maxPos = 80;
                subObjList.Add(new ObstacleType("RectPaddle", true, dir, new Vector2(-100, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RectPaddle", true, dir, new Vector2(100, 400), Quaternion.identity));

                //wall coordinates
                points.Add(new Vector2(hDist + 0.5f, 10.5f));  //y value = (obsHeight / 2) + 1?
                points.Add(new Vector2(160 + 0.5f, 10.5f));
                points.Add(new Vector2(160 + 0.5f, -10.5f));
                points.Add(new Vector2(hDist + 0.5f, -10.5f));
                Debug.Log(obsType);
                break;
            case "Door": //////USE WIDER PADDLES THAT MOVE OFFSCREEN//////
                maxPos = 80;
                subObjList.Add(new ObstacleType("RectPaddle", true, dir, new Vector2(-120, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RectPaddle", true, -dir, new Vector2(120, 400), Quaternion.identity));

                //wall coordinates
                points.Add(new Vector2(hDist + 0.5f, 10.5f));  //y value = (obsHeight / 2) + 1?
                points.Add(new Vector2(160 + 0.5f, 10.5f));
                points.Add(new Vector2(160 + 0.5f, -10.5f));
                points.Add(new Vector2(hDist + 0.5f, -10.5f));
                Debug.Log(obsType);
                break;
            case "RotateSingle":
                radius = 80;

                subObjList.Add(new ObstacleType("CurvePaddle", true, dir, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RotInner", false, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RotInner", false, new Vector2(0, 400), Quaternion.Euler(0,0,180)));

                points = GetCurvePoints(radius, segments, hDist); //get arc points
                Debug.Log(obsType);
                break;
            case "Rotate2":
                radius = 100;

                subObjList.Add(new ObstacleType("CurvePaddle", true, dir, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("CurvePaddle", true, dir, new Vector2(0, 400), Quaternion.Euler(0, 0, 180)));
                subObjList.Add(new ObstacleType("RotInner", false, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RotInner", false, new Vector2(0, 400), Quaternion.Euler(0, 0, 180)));

                points = GetCurvePoints(radius, segments, hDist); //get arc points
                Debug.Log(obsType);
                break;
            case "Rotate3":
                radius = 160;
                subObjList.Add(new ObstacleType("CurvePaddle", true, dir, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("CurvePaddle", true, dir, new Vector2(0, 400), Quaternion.Euler(0, 0, 120)));
                subObjList.Add(new ObstacleType("CurvePaddle", true, dir, new Vector2(0, 400), Quaternion.Euler(0, 0, 240)));
                subObjList.Add(new ObstacleType("RotInner", false, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RotInner", false, new Vector2(0, 400), Quaternion.Euler(0, 0, 180)));

                points = GetCurvePoints(radius, segments, hDist); //get arc points
                Debug.Log(obsType);
                break;
            case "RotateTunnel":
                radius = 100;
                subObjList.Add(new ObstacleType("RotTunnel", true, dir, new Vector2(0, 400), Quaternion.identity));
                subObjList.Add(new ObstacleType("RotTunnel", true, dir, new Vector2(0, 400), Quaternion.Euler(0, 0, 180)));

                points = GetCurvePoints(radius, segments, hDist); //get arc points
                Debug.Log(obsType);
                break;
        }
        //END Set up wall coordinates and subobject array

        CreateSubObjects(subObjList, container, dir, maxPos);
        WallLinesEdit.AddPoints(points);
    }
    //END Build the picked obstacle type

    //Create/enable the objects associated with the picked obstacle type
    void CreateSubObjects(List<ObstacleType> sots, GameObject cont, int dir, int max)
    {
        foreach(ObstacleType obj in sots)
        {
            //Get an obstacle object
            GameObject obstacleObj = PoolManager.Instance.GetObstacle();

            //Create the linerenderer coordinates
            List<Vector2> points = new List<Vector2>();

            switch (obj.name)
            {
                case "RectPaddle":
                    //RectPaddle coordinates
                    points.Add(new Vector2(0, 7.5f));   //y value = (obsHeight / 2) + 1?
                    points.Add(new Vector2(-40 + 0.5f, 7.5f));
                    points.Add(new Vector2(-40 + 0.5f, -7.5f));
                    points.Add(new Vector2(40 - 0.5f, -7.5f));
                    points.Add(new Vector2(40 - 0.5f, 7.5f));
                    points.Add(new Vector2(0, 7.5f));

                    //Debug.Log(obj.name);
                    break;
                case "CurvePaddle":
                    //points = GetCurvePoints(82, segments / 2, 76);
                    points = GetCurvePoints(radius - 18, segments / 2, radius - 24);
                    points.Reverse(); //reverse points
                                      //points.AddRange(GetCurvePoints(98, segments/2, 88));
                    points.AddRange(GetCurvePoints(radius - 2, segments / 2, radius - 12));
                    Vector2 p = points[0];  //get first point
                    points.Add(p);          //add it to the list to close the objects line //may need to add another

                    //Debug.Log(obj.name);
                    break;
                case "RotInner":
                    //spawn two and rotate the other one 180 degrees on initialisation
                    points = GetCurvePoints(radius - 20, segments, hDist);
                    Vector2 q = points[0];  //get first point
                    points.Add(q);          //add it to the list to close the objects line

                    //Debug.Log(obj.name);
                    break;
                case "RotTunnel":
                    //spawn two and rotate the other one 180 degrees on initialisation
                    points = GetCurvePoints(radius, segments, hDist);
                    Vector2 r = points[0];  //get first point
                    points.Add(r);          //add it to the list to close the objects line

                    //Debug.Log(obj.name);
                    break;
            }
            //ENDCreate the linerenderer coordinates

            obstacleObj.transform.parent = cont.transform; //assign parent object
            obstacleObj.GetComponent<Obstacle>().BuildObject(points, dir, max, obj);
        }
    }

    //Calculate the points array for a curve
    List<Vector2> GetCurvePoints(int radius, int segments, int xDist) 
    {
        float vDist = Mathf.Sqrt(((radius * radius) - (xDist * xDist))); //calculate the y component
        float startAngle = Mathf.Acos(vDist / radius); //use CAH to work out the start angle
        float endAngle = Mathf.Acos(-vDist / radius); //reverse the sign of the y component to get the opposite coordinate

        //Calculate Curve coordinates
        List<Vector2> arcPoints = new List<Vector2>();

        ////set up extra positions here? Use Switch statement?

        float angle = startAngle;
        float arcLength = endAngle - startAngle;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.RoundToInt(Mathf.Sin(angle) * radius);
            float y = Mathf.RoundToInt(Mathf.Cos(angle) * radius);


            arcPoints.Add(new Vector2(x + (x > 0 ? 0.5f : -0.5f), y - 0.5f)); //adjust x value based on whether it is to the Left or Right of center //adjust y value to match unit grid

            angle += (arcLength / segments);
        }
        //END Calculate Curve coordinates
        return arcPoints;
    }
    //END Calculate the points array for a curve


    //Randomly pick an obstacle to spawn
    string Choose(List<KeyValuePair<string, int>> probs)
    {
        float total = 0;
        //may need to sort list by value for every choose operation, especially if chance values are to be dynamics
        foreach (KeyValuePair<string, int> elem in probs)
        {
            total += elem.Value;
        }     

        float randomPoint = Random.value * total;

        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs[i].Value)
            {
                return probs[i].Key;
            }
            else
            {
                randomPoint -= probs[i].Value;
            }
        }
        return probs[probs.Count - 1].Key; //returns the last obstacle type (Lists used 0 based indexes, hence "Count - 1")
    }
    //END Randomly pick an obstacle to spawn

    void Restart()
    {
        if (IsInvoking("SpawnObject"))
        {
            StopCoroutine("SpawnObject");   //Stop spawning obstacles
            CancelInvoke("SpawnObject");    //Cancel any obstacles that are waiting to be spawned 
        }
        if (IsInvoking("SpawnProjectile"))
        {
            StopCoroutine("SpawnProjectile");   //Stop spawning obstacles
            CancelInvoke("SpawnProjectile");    //Cancel any obstacles that are waiting to be spawned 
        }

        numObstacles = 10;
        numProjectiles = 10;
    }

    void SetGameNormal()
    {
        GameManager.Instance.SetGameMode("Normal");
    }

    void SetGameDefense()
    {
        GameManager.Instance.SetGameMode("Defense");
    }
}
