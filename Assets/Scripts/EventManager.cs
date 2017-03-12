using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager> {

    public delegate void Scrollables();
    public static event Scrollables ScrollObjects;      //static declaration may not be necessary using "Instance" path

    public delegate void Changeables();
    public static event Changeables ChangeDirection;    //static declaration may not be necessary using "Instance" path

    public delegate void Restartables();
    public static event Restartables Restart;

    //OnDeath event for reset/highscores/and playing adverts?

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if(GameManager.Instance.Modes == GameManager.Mode.Game)
        {
            if (ScrollObjects != null) //error will be caused if an empty delegate/event is called/invoked
            {
                ScrollObjects();
            }
        }
        
        //Direction change event
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //change direction of objects subscribed to the "Change Direction" event
            if (ChangeDirection != null)
            {
                ChangeDirection();
            }
        }
        //END Direction change event
    }

    public void OnTouch()
    {
        if(ChangeDirection!= null)
        {
            ChangeDirection();
        }
    } 

    //Call restart event. Event will call all subscribed restart functions
    public void CallRestart()
    {
        if(Restart != null)
        {
            Restart();
        }
    }
}
